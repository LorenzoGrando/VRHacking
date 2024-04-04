#pragma once

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct Attributes
{
    float4 positionOS   : POSITION;
    float2 uv : TEXCOORD0;
    float3 normalOS : NORMAL;

    UNITY_VERTEX_INPUT_INSTANCE_ID 
};

struct Varyings
{
    float4 positionCS  : SV_POSITION;
    float2 uv : TEXCOORD0;
    float2 uvPolar : TEXCOORD1;
    float3 positionWS : TEXCOORD2;
    float3 normalWS : TEXCOORD3;

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

TEXTURE2D(_BaseTex);
SAMPLER(sampler_BaseTex);

float3 _LightDirection;

CBUFFER_START(UnityPerMaterial)

float4 _BaseColor;
half4 _BaseTex_ST;


float _Smoothness;
float _EmissionStrength;
float4 _EmissionColor;
float _EmissionMulByBaseColor;

float _RadialScale;
float _LengthScale;
float _Falloff;

CBUFFER_END

float4 TransformShadowCasterPositionCS(float3 positionWS, float3 normalWS)
{
    float3 lightDirectionWS = _LightDirection;
    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
    #if UNITY_REVERSED_Z
        positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #else   
        positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #endif
    
    return positionCS;
}

float2 TransformToPolarCoordinates(float2 UV, float2 Center, float RadialScale, float LengthScale) {
    float2 delta = UV - Center;
    float radius = length(delta) * 2 * RadialScale;
    float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
    return float2(radius, angle);
}

float4 DisplaceVertex(float4 pos, float2 polarUV)
{
    float displacement = 0.25 * sin(pos.x + _Time.x * 7);
    float damping = exp(polarUV.r * _Falloff) * (1 - polarUV.r);
    displacement *= abs(max(0,1 - damping));
    displacement = max(0, displacement);

    pos.y += displacement;
    
    return pos;
}
            
Varyings vert(Attributes i)
{
    Varyings o;

    //GPU Instancing and Single Pass Stereo Rendering
    UNITY_SETUP_INSTANCE_ID(i);                
    UNITY_TRANSFER_INSTANCE_ID(i, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.uvPolar = TransformToPolarCoordinates(i.uv, float2(0.5, 0.5), _RadialScale, _LengthScale);
    o.uv = TRANSFORM_TEX(i.uv, _BaseTex);
    
    i.positionOS = DisplaceVertex(i.positionOS, o.uvPolar);
    
    VertexPositionInputs vertexPositions = GetVertexPositionInputs(i.positionOS.xyz);
    VertexNormalInputs normalInputs = GetVertexNormalInputs(i.normalOS);
    
    #ifdef _APPLY_SHADOW_BIAS
        o.positionCS = TransformShadowCasterPositionCS(vertexPositions.positionWS, normalInputs.normalWS);
    #else
        o.positionCS = vertexPositions.positionCS;
    #endif
    
    o.positionWS = vertexPositions.positionWS;
    o.normalWS = normalInputs.normalWS;

    return o;
}

InputData SetupLightingData(float3 positionWS, float3 normalWS)
{
    InputData lightingData = (InputData)0;
    lightingData.positionWS = positionWS;
    lightingData.normalWS = normalWS;
    lightingData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(positionWS);
    lightingData.shadowCoord = TransformWorldToShadowCoord(positionWS);

    return lightingData;
}

SurfaceData SetupSurfaceData(float3 albedo, float3 specular, float3 emission, half alpha)
{
    SurfaceData surfaceData = (SurfaceData)0;
    surfaceData.albedo = albedo * _BaseColor.rgb;
    surfaceData.alpha = alpha * _BaseColor.a;
    surfaceData.specular = specular;

    float3 emissionResult = lerp(emission * _EmissionColor.rgb, emission * _BaseColor.rgb, _EmissionMulByBaseColor);
    emissionResult *= _EmissionStrength;
    surfaceData.emission = emissionResult;
    surfaceData.smoothness = _Smoothness;

    return surfaceData;
}

float4 CalculateLighting(InputData lightingData, SurfaceData surfaceData)
{
    //Blinn Phong shading

    return UniversalFragmentBlinnPhong(lightingData, surfaceData);
}
            
float4 frag(Varyings i) : SV_Target
{
    //GPU Instancing and Single Pass Stereo Rendering
    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
    
    float4 baseTextCol = SAMPLE_TEXTURE2D(_BaseTex, sampler_BaseTex, i.uv);

    InputData lightData = SetupLightingData(i.positionWS, i.normalWS);
    SurfaceData surfData = SetupSurfaceData(baseTextCol.rgb, 1, baseTextCol.rgb, 1);
    
    float4 finalCol = CalculateLighting(lightData, surfData);
    return finalCol;
}

float4 fragShadow(Varyings i) : SV_Target
{
    return 0;
}
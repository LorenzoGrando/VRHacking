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
    float2 uvGrid : TEXCOORD1;
    float3 positionWS : TEXCOORD2;
    float3 normalWS : TEXCOORD3;

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

TEXTURE2D(_BaseTex);
SAMPLER(sampler_BaseTex);

TEXTURE2D(_GridTex);
SAMPLER(sampler_GridTex);

float3 _LightDirection;

CBUFFER_START(UnityPerMaterial)

float4 _BaseColor;
half4 _GridTex_ST;

float _ScrollSpeed;

float _Smoothness;
float _EmissionStrength;
float4 _EmissionColor;
float _EmissionMulByBaseColor;

CBUFFER_END

float4 TransformShadowCasterPositionCS(float3 positionWS, float3 normalWS)
{
    float3 lightDirectionWS = _LightDirection;
    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
    #if UNITY_REVERSED_Z
        positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #else   
        positionCS.z = max(positionCS.Z, UNITY_NEAR_CLIP_VALUE);
    #endif
    
    return positionCS;
}

            
Varyings vert(Attributes i)
{
    Varyings o;

    //GPU Instancing and Single Pass Stereo Rendering
    UNITY_SETUP_INSTANCE_ID(input);                
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
    VertexPositionInputs vertexPositions = GetVertexPositionInputs(i.positionOS.xyz);
    VertexNormalInputs normalInputs = GetVertexNormalInputs(i.normalOS);
    
    
    

    #ifdef _APPLY_SHADOW_BIAS
        o.positionCS = TransformShadowCasterPositionCS(vertexPositions.positionWS, normalInputs.normalWS);
    #else
        o.positionCS = vertexPositions.positionCS;
    #endif
    
    o.uv = i.uv;
    o.uvGrid = TRANSFORM_TEX(i.uv, _GridTex);
    o.uvGrid.y += _Time.x * _ScrollSpeed;
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

    float3 emissionResult = lerp(emission * _EmissionColor, emission * _BaseColor, _EmissionMulByBaseColor);
    emissionResult /= _EmissionStrength;
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
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
    float4 baseTextCol = SAMPLE_TEXTURE2D(_BaseTex, sampler_BaseTex, i.uv);
    float4 maskedCol = baseTextCol.r * abs(1- SAMPLE_TEXTURE2D(_GridTex, sampler_GridTex, i.uvGrid));
    float3 combinedCol = max(baseTextCol.bbb, maskedCol.rrr);
    //float3 shadedCol = (_BaseColor * combinedCol).xyz;

    InputData lightData = SetupLightingData(i.positionWS, i.normalWS);
    SurfaceData surfData = SetupSurfaceData(combinedCol.rgb, 1, combinedCol, 1);
    
    float4 finalCol = CalculateLighting(lightData, surfData);
    return finalCol;
}

float4 fragShadow(Varyings i) : SV_Target
{
    return 0;
}
#pragma once

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

struct Attributes
{
    float4 positionOS   : POSITION;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS  : SV_POSITION;
    float2 uv : TEXCOORD0;
    float2 uvGrid : TEXCOORD1;
};

TEXTURE2D(_BaseTex);
SAMPLER(sampler_BaseTex);

TEXTURE2D(_GridTex);
SAMPLER(sampler_GridTex);

CBUFFER_START(UnityPerMaterial)

float4 _BaseColor;
half4 _GridTex_ST;

float _ScrollSpeed;

CBUFFER_END

            
Varyings vert(Attributes i)
{
    Varyings o;
                
    VertexPositionInputs vertexPositions = GetVertexPositionInputs(i.positionOS.xyz);
    o.positionCS = vertexPositions.positionCS;
    o.uv = i.uv;
    o.uvGrid = TRANSFORM_TEX(i.uv, _GridTex);
    o.uvGrid.y += _Time * _ScrollSpeed;

    return o;
}
            
float4 frag(Varyings i) : SV_Target
{
    float4 baseTextCol = SAMPLE_TEXTURE2D(_BaseTex, sampler_BaseTex, i.uv);
    
    float4 maskedCol = baseTextCol.r * abs(1- SAMPLE_TEXTURE2D(_GridTex, sampler_GridTex, i.uvGrid));

    float3 combinedCol = max(baseTextCol.bbb, maskedCol.rrr);

    float3 shadedCol = (_BaseColor * combinedCol).xyz;
    return float4(shadedCol, 1);
}
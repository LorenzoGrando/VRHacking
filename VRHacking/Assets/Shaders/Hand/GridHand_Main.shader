Shader "Custom/GridHand"
{
    Properties
    { 
        [MainColor] _BaseColor ("Base Color", Color) = (1,1,1,1)
        [MainTexture][NoScaleOffset] _BaseTex ("Base Tex", 2D) = "white" {}
        _GridTex ("Grid Tex", 2D) = "white" {}
        _ScrollSpeed ("Grid Scroll Speed", Float) = 0.25
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "GridHand_Shade.hlsl"
            ENDHLSL
        }
    }
    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
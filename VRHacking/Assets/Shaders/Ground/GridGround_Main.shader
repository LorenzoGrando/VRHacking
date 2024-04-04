Shader "Custom/GridGround"
{
    Properties
    {
        [Header(Base Color)]
        [MainColor] _BaseColor ("Base Color", Color) = (1,1,1,1)
        [MainTexture] _BaseTex ("Base Tex", 2D) = "white" {}

        [Header(Lighting)]
        _Smoothness ("Smoothness", Range(0,1)) = 0
        _EmissionStrength ("Emission Strenght", Range(0.1, 1)) = 1
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        [Toggle]_EmissionMulByBaseColor ("  Use Base Color Instead", Range(0,1)) = 0
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

            // URP Keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
            
            #define _SPECULAR_COLOR

            #include "GridGround_Shade.hlsl"
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
            
            ZWrite On
            ZTest LEqual
            Colormask 0
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragShadow

            #define _APPLY_SHADOW_BIAS

            #include "GridGround_Shade.hlsl"
            ENDHLSL
        }
    
    }
    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
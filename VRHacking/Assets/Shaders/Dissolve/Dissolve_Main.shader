Shader "Custom/Dissolve"
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
        
        [Header(Voronoi Noise)]
        _CellDensity ("Cell Density", Float) = 1
        _AngleOffset ("Angle Offset", Float) = 1
        _NoiseStrength ("Noise Strength", Float) = 1
        _CutoffHeight ("Cutoff Height", Float) = 0
        _EdgeWidth ("Edge Width", Float) = 0.1
        
        [Header(Scrolling Mask)]
        [Toggle]_UseScroll ("Enable Scrolling Mask", Range(0,1)) = 0
        [NoScaleOffset]_ScrollMaskTex ("Mask Texture", 2D) = "white" {}
        _ScrollStrength ("Strength", Range(0,1)) = 1
        _ScrollSpeed ("Speed", Float) = 1
        _ScrollDirection ("DirectionVector", Vector) = (0,1,0,0)
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
                "Queue" = "AlphaTest"
            }
            
            Cull Off
            
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

            #include "Dissolve_Shade.hlsl"
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
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragShadow

            #define _APPLY_SHADOW_BIAS

            #include "Dissolve_Shade.hlsl"
            ENDHLSL
        }
    
    }
    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
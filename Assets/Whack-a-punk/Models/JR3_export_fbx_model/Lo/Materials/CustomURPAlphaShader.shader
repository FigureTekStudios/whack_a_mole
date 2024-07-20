Shader "Custom/URPAlphaMultiplyShader"
{
    Properties
    {
        _MainTex("Base Texture (RGB)", 2D) = "white" {}
        _AlphaTex("Alpha Texture (B/W)", 2D) = "white" {}
        _Color("Multiply Color", Color) = (1, 1, 1, 1)
        _UseAlphaTex("Use Alpha Texture", Float) = 0 // 0 means don't use, 1 means use
        _UseEmission("Use Emission", Float) = 0 // 0 means don't use, 1 means use
        _EmissionColor("Emission Color", Color) = (0, 0, 0, 1) // Default emission color
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 100

            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite On
                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                struct Attributes
                {
                    float4 position : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct Varyings
                {
                    float2 uv : TEXCOORD0;
                    float4 position : SV_POSITION;
                };

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                TEXTURE2D(_AlphaTex);
                SAMPLER(sampler_AlphaTex);
                float4 _MainTex_ST;
                float4 _Color;
                float _UseAlphaTex;
                float _UseEmission;
                float4 _EmissionColor;

                Varyings vert(Attributes v)
                {
                    Varyings o;
                    o.position = TransformObjectToHClip(v.position);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                half4 frag(Varyings i) : SV_Target
                {
                    // Sample the main texture
                    half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                    // Apply alpha texture if needed
                    if (_UseAlphaTex > 0.5)
                    {
                        half4 alpha = SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, i.uv);
                        col.a = alpha.r;
                    }
                    else
                    {
                        col.a = 1.0;
                    }

                    // Multiply the color by the specified color
                    col.rgb *= _Color.rgb;

                    // Apply emission if enabled
                    if (_UseEmission > 0.5)
                    {
                        col.rgb += _EmissionColor.rgb * col.rgb;
                    }

                    return col;
                }
                ENDHLSL
            }
        }
            FallBack "Diffuse"
}

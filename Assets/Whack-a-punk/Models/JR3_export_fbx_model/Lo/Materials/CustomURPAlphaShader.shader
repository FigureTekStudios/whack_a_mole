Shader "Custom/URPAlphaMultiplyShader"
{
    Properties
    {
        _MainTex("Base Texture (RGB)", 2D) = "white" {}
        _AlphaTex("Alpha Texture (B/W)", 2D) = "white" {}
        _Color("Multiply Color", Color) = (1, 1, 1, 1)
        _UseAlphaTex("Use Alpha Texture", Float) = 0 // 0 means don't use, 1 means use
        _UseEmission("Use Emission", Float) = 0 // 0 means don't use, 1 means use
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 100

            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite On
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                sampler2D _AlphaTex;
                float4 _MainTex_ST;
                float4 _Color;
                float _UseAlphaTex;
                float _UseEmission;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    UNITY_TRANSFER_FOG(o, o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // sample the main texture
                    fixed4 col = tex2D(_MainTex, i.uv);

                    if (_UseAlphaTex > 0.5)
                    {
                        // sample the alpha texture
                        fixed4 alpha = tex2D(_AlphaTex, i.uv);
                        // set the alpha value from the red channel of the alpha texture
                        col.a = alpha.r;
                    }
                    else
                    {
                        // no alpha texture, assume opaque
                        col.a = 1.0;
                    }

                    // multiply the color by the specified color
                    col.rgb *= _Color.rgb;

                    if (_UseEmission > 0.5)
                    {
                        // map the diffuse color to the emission channel
                        col.rgb += col.rgb;
                    }

                    // apply fog
                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
                ENDCG
            }
        }
}

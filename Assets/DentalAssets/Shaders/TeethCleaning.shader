Shader "Custom/TeethCleaning"
{
    Properties
    {
        _BaseMap ("Clean Texture", 2D) = "white" {}
        _DetailMap ("Plaque Texture", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "black" {}
        
        _CleanTint ("Clean Teeth Color Tint", Color) = (1,1,1,1)
        _PlaqueTint ("Plaque Color Tint", Color) = (1,1,1,1)
        _CleanBrightness ("Clean Brightness", Range(0.5, 2.0)) = 1.0
        _PlaqueBrightness ("Plaque Brightness", Range(0.5, 2.0)) = 1.0
        _Contrast ("Contrast", Range(0.0, 2.0)) = 1.0
        _Saturation ("Saturation", Range(0.0, 2.0)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // Added for instancing
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO // Added for stereo output
            };

            sampler2D _BaseMap;
            sampler2D _DetailMap;
            sampler2D _MaskTex;
            float4 _CleanTint;
            float4 _PlaqueTint;
            float _CleanBrightness;
            float _PlaqueBrightness;
            float _Contrast;
            float _Saturation;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v); // Added for instancing
                UNITY_INITIALIZE_OUTPUT(v2f, o); // Added to initialize output
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); // Added for stereo output

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            float3 AdjustContrast(float3 color, float contrast)
            {
                return (color - 0.5f) * contrast + 0.5f;
            }

            float3 AdjustSaturation(float3 color, float saturation)
            {
                float grey = dot(color, float3(0.3, 0.59, 0.11));
                return lerp(grey.xxx, color, saturation);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 clean = tex2D(_BaseMap, i.uv);
                fixed4 plaque = tex2D(_DetailMap, i.uv);
                fixed mask = tex2D(_MaskTex, i.uv).r;

                clean.rgb *= _CleanTint.rgb * _CleanBrightness;
                plaque.rgb *= _PlaqueTint.rgb * _PlaqueBrightness;

                float3 finalColor = lerp(plaque.rgb, clean.rgb, mask);

                finalColor = AdjustContrast(finalColor, _Contrast);
                finalColor = AdjustSaturation(finalColor, _Saturation);

                return float4(finalColor, 1);
            }
            ENDCG
        }
    }
}

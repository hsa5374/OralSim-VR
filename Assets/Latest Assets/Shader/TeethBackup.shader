Shader "Teeth/URP_DirtyBlendShader"
{
    Properties
    {
        _CleanTex ("Clean Texture", 2D) = "white" {}
        _CleanNormal ("Clean Normal", 2D) = "bump" {}
        _CleanAO ("Clean AO", 2D) = "white" {}
        
        _DirtyTex ("Dirty Texture", 2D) = "white" {}
        _DirtyNormal ("Dirty Normal", 2D) = "bump" {}
        _DirtyOpacity ("Dirty Opacity", Range(0,1)) = 1
        _DirtyTiling ("Dirty Tiling", Float) = 1
        
        _BlendMask ("Blend Mask", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "Queue"="Geometry" }
        Pass
        {
            Name "Lit"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _NORMALMAP
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };
            
            sampler2D _CleanTex, _CleanNormal, _CleanAO;
            sampler2D _DirtyTex, _DirtyNormal, _BlendMask;
            float _DirtyOpacity, _DirtyTiling;
            
            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                return o;
            }
            
            half4 frag (Varyings i) : SV_Target
            {
                float2 dirtyUV = i.uv * _DirtyTiling;
                
                half4 cleanCol = tex2D(_CleanTex, i.uv);
                half4 dirtyCol = tex2D(_DirtyTex, dirtyUV);
                half blendMask = tex2D(_BlendMask, i.uv).r;
                
                half3 cleanNormal = UnpackNormal(tex2D(_CleanNormal, i.uv));
                half3 dirtyNormal = UnpackNormal(tex2D(_DirtyNormal, dirtyUV));
                
                half3 finalNormal = normalize(lerp(cleanNormal, dirtyNormal, blendMask * _DirtyOpacity));
                
                half ao = tex2D(_CleanAO, i.uv).r;
                half3 albedo = lerp(cleanCol.rgb, dirtyCol.rgb, blendMask * _DirtyOpacity);
                
                return half4(albedo * ao, 1);
            }
            
            ENDHLSL
        }
    }
}
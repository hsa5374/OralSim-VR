Shader "2Sided/TwoSidedURPShader"
{
    Properties
    {
        _BaseMap ("Base Texture", 2D) = "white" {}
        _BaseNormalMap ("Base Normal Map", 2D) = "bump" {}
        _OcclusionMap ("Occlusion Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200
        Cull Off // Enables two-sided rendering
        
        Pass
        {
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 tangentWS : TEXCOORD2;
                float3 bitangentWS : TEXCOORD3;
                float4 positionHCS : SV_POSITION;
            };
            
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BaseNormalMap); SAMPLER(sampler_BaseNormalMap);
            TEXTURE2D(_OcclusionMap); SAMPLER(sampler_OcclusionMap);
            
            float _Glossiness;
            float _Metallic;
            
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.uv = IN.uv;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.tangentWS = TransformObjectToWorldDir(IN.tangentOS.xyz);
                OUT.bitangentWS = cross(OUT.normalWS, OUT.tangentWS) * IN.tangentOS.w;
                return OUT;
            }
            
            half4 frag (Varyings IN) : SV_Target
            {
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_BaseNormalMap, sampler_BaseNormalMap, IN.uv));
                half ao = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, IN.uv).r;
                
                // Construct the tangent to world matrix
                float3x3 tangentToWorld = float3x3(IN.tangentWS, IN.bitangentWS, IN.normalWS);
                float3 normalWS = mul(normalTS, tangentToWorld);
                
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = baseColor.rgb;
                surfaceData.normalTS = normalTS;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Glossiness;
                surfaceData.occlusion = ao;
                
                InputData inputData = (InputData)0;
                inputData.positionWS = GetCameraPositionWS(); // Corrected to use world space position
                inputData.normalWS = normalize(normalWS);
                inputData.viewDirectionWS = GetWorldSpaceViewDir(inputData.positionWS);
                inputData.tangentToWorld = tangentToWorld;
                
                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                return color;
            }
            ENDHLSL
        }
    }
}
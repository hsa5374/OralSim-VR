Shader "Tarter/TeethTextureShaderURP"
{
    Properties
    {
        _BaseMap ("Base Texture", 2D) = "white" {}
        _BaseNormalMap ("Base Normal Map", 2D) = "bump" {}
        _OcclusionMap ("Occlusion Map", 2D) = "white" {}
        _RustMap ("Rust Texture", 2D) = "white" {}
        _RustMask ("Rust Mask", 2D) = "white" {}
        _RustOpacity ("Rust Opacity", Range(0,1)) = 1.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200
        Cull Off // Enable two-sided rendering (optional)

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
                float4 tangentOS : TANGENT; // Added for normal mapping
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 tangentWS : TEXCOORD2; // Added for normal mapping
                float3 bitangentWS : TEXCOORD3; // Added for normal mapping
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD4; // Added for world space position
            };
            
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BaseNormalMap); SAMPLER(sampler_BaseNormalMap);
            TEXTURE2D(_OcclusionMap); SAMPLER(sampler_OcclusionMap);
            TEXTURE2D(_RustMap); SAMPLER(sampler_RustMap);
            TEXTURE2D(_RustMask); SAMPLER(sampler_RustMask);
            
            float _RustOpacity;
            float _Glossiness;
            float _Metallic;
            
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.uv = IN.uv;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz); // World space position
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.tangentWS = TransformObjectToWorldDir(IN.tangentOS.xyz); // Tangent in world space
                OUT.bitangentWS = cross(OUT.normalWS, OUT.tangentWS) * IN.tangentOS.w; // Bitangent in world space
                return OUT;
            }
            
            half4 frag (Varyings IN) : SV_Target
            {
                // Sample textures
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half4 rustColor = SAMPLE_TEXTURE2D(_RustMap, sampler_RustMap, IN.uv);
                half rustMask = SAMPLE_TEXTURE2D(_RustMask, sampler_RustMask, IN.uv).r;
                
                // Blend base color and rust color based on rust mask and opacity
                half rustFactor = rustMask * _RustOpacity;
                half3 finalColor = lerp(baseColor.rgb, rustColor.rgb, rustFactor);
                
                // Sample and unpack normal map
                half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_BaseNormalMap, sampler_BaseNormalMap, IN.uv));
                
                // Construct tangent-to-world matrix
                float3x3 tangentToWorld = float3x3(IN.tangentWS, IN.bitangentWS, IN.normalWS);
                float3 normalWS = normalize(mul(normalTS, tangentToWorld)); // Transform normal to world space
                
                // Sample occlusion map
                half ao = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, IN.uv).r;
                
                // Prepare SurfaceData for PBR lighting
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = finalColor;
                surfaceData.normalTS = normalTS;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Glossiness;
                surfaceData.occlusion = ao;
                
                // Prepare InputData for PBR lighting
                InputData inputData = (InputData)0;
                inputData.positionWS = IN.positionWS; // Use world space position
                inputData.normalWS = normalWS; // Use transformed normal
                inputData.viewDirectionWS = GetWorldSpaceViewDir(IN.positionWS); // View direction in world space
                inputData.tangentToWorld = tangentToWorld; // Tangent-to-world matrix
                
                // Calculate final color using URP's PBR lighting
                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                return color;
            }
            ENDHLSL
        }
    }
}
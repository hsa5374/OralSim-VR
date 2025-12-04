Shader "Teeth/URP_DirtyBlendShader"
{
    Properties
    {
        [MainTexture] _CleanTex ("Clean Texture", 2D) = "white" {}
        [Normal] _CleanNormal ("Clean Normal", 2D) = "bump" {}
        _CleanAO ("Clean AO", 2D) = "white" {}
        _DirtyTex ("Dirty Texture", 2D) = "white" {}
        [Normal] _DirtyNormal ("Dirty Normal", 2D) = "bump" {}
        _DirtyOpacity ("Dirty Opacity", Range(0,1)) = 1
        _DirtyTiling ("Dirty Tiling", Float) = 1
        _BlendMask ("Blend Mask", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // Greatly reduced keywords to minimize variants
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile_instancing
            
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 tangentWS : TEXCOORD2;
                float3 bitangentWS : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_CleanTex); SAMPLER(sampler_CleanTex);
            TEXTURE2D(_CleanNormal); SAMPLER(sampler_CleanNormal);
            TEXTURE2D(_CleanAO); SAMPLER(sampler_CleanAO);
            TEXTURE2D(_DirtyTex); SAMPLER(sampler_DirtyTex);
            TEXTURE2D(_DirtyNormal); SAMPLER(sampler_DirtyNormal);
            TEXTURE2D(_BlendMask); SAMPLER(sampler_BlendMask);

            CBUFFER_START(UnityPerMaterial)
                float4 _CleanTex_ST;
                float _DirtyOpacity;
                float _DirtyTiling;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionHCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _CleanTex);
                output.normalWS = normalInput.normalWS;
                output.tangentWS = normalInput.tangentWS;
                output.bitangentWS = normalInput.bitangentWS;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 dirtyUV = input.uv * _DirtyTiling;
                
                // Sample textures
                half4 cleanCol = SAMPLE_TEXTURE2D(_CleanTex, sampler_CleanTex, input.uv);
                half4 dirtyCol = SAMPLE_TEXTURE2D(_DirtyTex, sampler_DirtyTex, dirtyUV);
                half blendMask = SAMPLE_TEXTURE2D(_BlendMask, sampler_BlendMask, input.uv).r;
                
                // Normal mapping
                half3 cleanNormalTS = UnpackNormal(SAMPLE_TEXTURE2D(_CleanNormal, sampler_CleanNormal, input.uv));
                half3 dirtyNormalTS = UnpackNormal(SAMPLE_TEXTURE2D(_DirtyNormal, sampler_DirtyNormal, dirtyUV));
                
                // Blend normals in tangent space
                half3 blendedNormalTS = normalize(lerp(cleanNormalTS, dirtyNormalTS, blendMask * _DirtyOpacity));
                
                // Convert normal to world space
                float3x3 tangentToWorld = float3x3(
                    input.tangentWS,
                    input.bitangentWS,
                    input.normalWS
                );
                half3 normalWS = normalize(mul(blendedNormalTS, tangentToWorld));
                
                // Simplified lighting
                Light mainLight = GetMainLight();
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                
                // Sample AO
                half ao = SAMPLE_TEXTURE2D(_CleanAO, sampler_CleanAO, input.uv).r;
                
                // Final color with simplified lighting
                half3 albedo = lerp(cleanCol.rgb, dirtyCol.rgb, blendMask * _DirtyOpacity);
                half3 finalColor = albedo * ao * (NdotL * mainLight.color + 0.2); // Added ambient term
                
                return half4(finalColor, 1);
            }
            ENDHLSL
        }
    }
}
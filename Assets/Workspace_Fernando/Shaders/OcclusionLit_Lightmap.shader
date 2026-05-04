/*
 * Based on Meta's OcclusionLit.shader
 * Modified to add Lightmap (Baked GI) support
 */

Shader "Custom/OcclusionLit_Lightmap"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _EnvironmentDepthBias ("Environment Depth Bias", Float) = 0.0
    }

    SubShader
    {
        PackageRequirements {"com.unity.render-pipelines.universal"}
        Pass
        {
            Tags { "LightMode" = "UniversalForward"}
            Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha

            HLSLPROGRAM

            // Universal Render Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ EVALUATE_SH_MIXED
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP
            #pragma multi_compile _ HARD_OCCLUSION SOFT_OCCLUSION
            
            // Lightmap support - THIS IS THE KEY ADDITION
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.meta.xr.sdk.core/Shaders/EnvironmentDepth/URP/EnvironmentOcclusionURP.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                float2 staticLightmapUV : TEXCOORD1;  // Added for Lightmap support

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                // World position variable must have this name to work with EnvironmentDepth Macros
                float3 posWorld : TEXCOORD1;

                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
                float2 uv : TEXCOORD0;
                
                // Added for Lightmap support
                DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 4);

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float _Glossiness;
            float _Metallic;
            float _EnvironmentDepthBias;

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float4 _MainTex_ST;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionCS = vertexInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = normalInputs.normalWS;
                output.tangentWS = float4(normalInputs.tangentWS, input.tangentOS.w);
                output.posWorld = vertexInputs.positionWS;

                // Lightmap support - transfer lightmap UVs or calculate SH
                OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

                META_DEPTH_INITIALIZE_VERTEX_OUTPUT(output, input.positionOS.xyz);

                return output;
            }


            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float3 positionWS = input.posWorld;
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = GetWorldSpaceNormalizeViewDir(positionWS);

                float2 uv = input.uv;
                float4 colorSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * _Color;

                InputData lightingInput = (InputData) 0;
                lightingInput.positionWS = positionWS;
                lightingInput.normalWS = normalWS;
                lightingInput.viewDirectionWS = viewDirWS;
                lightingInput.shadowCoord = TransformWorldToShadowCoord(positionWS);
                
                // Lightmap support - sample baked GI
                lightingInput.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, normalWS);
                lightingInput.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);
                
                lightingInput.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

#if UNITY_VERSION >= 202120
                lightingInput.positionCS = input.positionCS;
#endif

                SurfaceData surfaceInput = (SurfaceData) 0;
                surfaceInput.albedo = colorSample.rgb;
                surfaceInput.alpha = colorSample.a;
                surfaceInput.metallic = _Metallic;
                surfaceInput.smoothness = _Glossiness;
                surfaceInput.emission = 0;
                surfaceInput.occlusion = 1.0;

                half4 finalOutputColor = UniversalFragmentPBR(lightingInput, surfaceInput);

                META_DEPTH_OCCLUDE_OUTPUT_PREMULTIPLY(input, finalOutputColor, _EnvironmentDepthBias);

                return finalOutputColor;
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        // Meta Lightmap pass - Required for lightmap baking
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaLit

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"

            ENDHLSL
        }
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Standard finalcolor:colorModifier fullforwardshadows keepalpha
        #pragma target 3.5

        #pragma multi_compile _ HARD_OCCLUSION SOFT_OCCLUSION
        #include "Packages/com.meta.xr.sdk.core/Shaders/EnvironmentDepth/BiRP/EnvironmentOcclusionBiRP.cginc"

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _EnvironmentDepthBias;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }

        void colorModifier(Input IN, SurfaceOutputStandard o, inout fixed4 color)
        {
            META_DEPTH_OCCLUDE_OUTPUT_PREMULTIPLY_WORLDPOS(IN.worldPos, color, _EnvironmentDepthBias)
        }
        ENDCG
    }
    FallBack "Diffuse"
}

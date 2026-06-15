Shader "StickyFingers/FogOfWar"
{
    // World-space fog-of-war plane. Lay it flat over the play area at ground height. Reads the reveal
    // mask + coverage rect set by FogOfWarController and darkens wherever the player is NOT currently
    // looking. Real-time: the mask is cleared every frame, so fog returns the moment you look away.
    Properties
    {
        _FogColor ("Fog Color", Color) = (0, 0, 0, 1)
        _FogStrength ("Fog Strength", Range(0, 1)) = 0.9
        _Feather ("Edge Feather", Range(0.001, 0.5)) = 0.12
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_FogRevealMask);
            SAMPLER(sampler_FogRevealMask);
            float4 _FogRevealBounds; // xy = world center XZ, zw = half extents XZ
            float4 _FogColor;
            float _FogStrength;
            float _Feather;

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float3 positionWS : TEXCOORD0; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs p = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = p.positionCS;
                OUT.positionWS  = p.positionWS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // World XZ -> mask UV using the reveal camera's coverage rectangle.
                float2 uv = (IN.positionWS.xz - _FogRevealBounds.xy) / (2.0 * _FogRevealBounds.zw) + 0.5;

                // Outside the reveal camera's coverage -> fully fogged.
                float inside = step(0.0, uv.x) * step(uv.x, 1.0) * step(0.0, uv.y) * step(uv.y, 1.0);
                float reveal = inside * SAMPLE_TEXTURE2D(_FogRevealMask, sampler_FogRevealMask, uv).r;

                reveal = smoothstep(0.0, _Feather, reveal);
                float alpha = (1.0 - reveal) * _FogStrength;
                return half4(_FogColor.rgb, alpha);
            }
            ENDHLSL
        }
    }
}

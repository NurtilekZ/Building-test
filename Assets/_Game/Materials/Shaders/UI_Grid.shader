Shader "UI/GridOccupancy"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (0,0,0,0.15)
        _LineColor("Line Color", Color) = (0,1,1,1)
        _OccupiedColor("Occupied Color", Color) = (1,0,0,0.55)

        _CellCount("Grid Cells XY", Vector) = (10,10,0,0)
        _LineThickness("Line Thickness", Range(0.001,0.2)) = 0.02

        _OccupancyTex("Occupancy Texture", 2D) = "black" {}
        
        _Stencil("Stencil Reference", Float) = 0
        _StencilComp("Stencil Comparison", Float) = 8
        _StencilOp("Stencil Operation", Float) = 0
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            
            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            ColorMask [_ColorMask]
            
            Name "UIGrid"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_OccupancyTex);
            SAMPLER(sampler_OccupancyTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _LineColor;
                half4 _OccupiedColor;
                float4 _CellCount;
                float _LineThickness;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float GridAA(float2 uv)
            {
                float2 cellUV = uv * _CellCount.xy;

                float2 distToLine = abs(frac(cellUV) - 0.5) * 2.0;
                float2 aa = fwidth(cellUV);

                float lineX = smoothstep(1.0 - _LineThickness - aa.x,
                                               1.0 - _LineThickness + aa.x,
                                               distToLine.x);

                float lineY = smoothstep(1.0 - _LineThickness - aa.y,
                                               1.0 - _LineThickness + aa.y,
                                               distToLine.y);

                return max(lineX, lineY);
            }

            float OccupancyMask(float2 uv)
            {
                float2 cellIndex = floor(uv * _CellCount.xy);
                float2 sampleUV = (cellIndex + 0.5) / _CellCount.xy;

                return SAMPLE_TEXTURE2D(_OccupancyTex, sampler_OccupancyTex, sampleUV).r;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float lineMask = GridAA(i.uv);
                float occupied = OccupancyMask(i.uv);

                half4 col = _BaseColor;
                col = lerp(col, _OccupiedColor, occupied);
                col = lerp(col, _LineColor, lineMask);

                return col;
            }
            ENDHLSL
        }
    }
}
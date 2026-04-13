Shader "UI/URP/GridShader"
{
    Properties
    {
        [Header(Base)]
        _MainTex("Base Map", 2D) = "white" {}
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        
        [Header(Grid Settings)]
        _GridWidth("Grid Width", Int) = 20
        _GridHeight("Grid Height", Int) = 20
        
        [Header(Cell Colors)]
        _CellOccupancyTex("Cell Occupancy Texture", 2D) = "black" {}
        _EmptyColor("Empty Cell Color", Color) = (0.2, 0.2, 0.2, 0.5)
        _OccupiedColor("Occupied Cell Color", Color) = (0.4, 0.6, 0.4, 0.8)
        _SelectedColor("Selected Cell Color", Color) = (0.2, 0.6, 0.9, 0.9)
        
        [Header(Grid Lines)]
        _GridLineColor("Grid Line Color", Color) = (0.1, 0.1, 0.1, 1)
        _GridLineWidth("Grid Line Width", Float) = 0.02
        
        [Header(Selection)]
        _SelectedCellX("Selected Cell X", Int) = -1
        _SelectedCellY("Selected Cell Y", Int) = -1
        _UseTexture("Use Texture", Float) = 1
        
        [Header(Blending)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend", Float) = 10
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcAlphaBlend("Src Alpha Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstAlphaBlend("Dst Alpha Blend", Float) = 10
        
        [Header(Stencil)]
        _Stencil("Stencil ID", Float) = 0
        _StencilComp("Stencil Comparison", Float) = 8
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        
        [Header(Other)]
        [Toggle(_CLIP)] _UseClip("Use Clip", Float) = 0
        _ClipRect("Clip Rect", Vector) = (0, 0, 1, 1)
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }
        
        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
        
        Pass
        {
            Name "Grid"
            Tags { "LightMode" = "Universal2D" }
            
            Blend[_SrcBlend][_DstBlend], [_SrcAlphaBlend][_DstAlphaBlend]
            Cull Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            
            HLSLPROGRAM
            #pragma vertex GridVert
            #pragma fragment GridFrag
            #pragma multi_compile _ _CLIP
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _MainTex;
                int _GridWidth;
                int _GridHeight;
                float4 _EmptyColor;
                float4 _OccupiedColor;
                float4 _SelectedColor;
                float4 _GridLineColor;
                float _GridLineWidth;
                int _SelectedCellX;
                int _SelectedCellY;
                float _UseTexture;
                float4 _ClipRect;
            CBUFFER_END
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_CellOccupancyTex);
            SAMPLER(sampler_CellOccupancyTex);
            
            Varyings GridVert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.worldPosition = input.positionOS;
                output.color = input.color * _BaseColor;
                return output;
            }
            
            half4 GridFrag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float2 uv = input.uv;
                
                float invWidth = 1.0 / max(1, _GridWidth);
                float invHeight = 1.0 / max(1, _GridHeight);
                
                float cellX = floor(uv.x * _GridWidth);
                float cellY = floor(uv.y * _GridHeight);
                
                float2 cellCenterUV = float2(
                    (cellX + 0.5) * invWidth,
                    (cellY + 0.5) * invHeight
                );
                
                float isOccupied = 0.0;
                if (_UseTexture > 0.5)
                {
                    isOccupied = SAMPLE_TEXTURE2D(_CellOccupancyTex, sampler_CellOccupancyTex, cellCenterUV).r;
                }
                
                bool isSelected = (cellX == _SelectedCellX && cellY == _SelectedCellY && _SelectedCellX >= 0 && _SelectedCellY >= 0);
                
                half4 cellColor;
                if (isSelected)
                {
                    cellColor = _SelectedColor;
                }
                else if (isOccupied > 0.5)
                {
                    cellColor = _OccupiedColor;
                }
                else
                {
                    cellColor = _EmptyColor;
                }
                
                float2 cellFrac = frac(uv * float2(_GridWidth, _GridHeight));
                
                float lineWidth = _GridLineWidth * 0.5;
                float gridX = smoothstep(1.0 - lineWidth, 1.0, cellFrac.x);
                float gridY = smoothstep(1.0 - lineWidth, 1.0, cellFrac.y);
                float gridLine = max(gridX, gridY);
                
                float cellWidth = invWidth;
                float cellHeight = invHeight;
                float borderLeft = step(uv.x, cellWidth * 0.5);
                float borderBottom = step(uv.y, cellHeight * 0.5);
                float borderRight = step(1.0 - cellWidth * 0.5, uv.x);
                float borderTop = step(1.0 - cellHeight * 0.5, uv.y);
                float border = max(max(borderLeft, borderBottom), max(borderRight, borderTop));
                
                gridLine = max(gridLine, border);
                
                half4 finalColor = lerp(cellColor, _GridLineColor, gridLine);
                
                #if _CLIP
                    float2 position = input.worldPosition.xy;
                    float2 clipEdge = step(_ClipRect.xy, position) * step(position, _ClipRect.zw);
                    float clipAlpha = clipEdge.x * clipEdge.y;
                    finalColor.a *= clipAlpha;
                #endif
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "UI/Default"
}

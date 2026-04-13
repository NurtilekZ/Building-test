Shader "Game/GridWorld"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0, 0, 0, 0)
        _LineColor ("Line Color", Color) = (0, 1, 1, 0.8)
        _CellSize ("Cell Size", Float) = 1
        _LineThickness ("Line Thickness", Range(0.001, 0.2)) = 0.03
        _GridOrigin ("Grid Origin", Vector) = (0, 0, 0, 0)
        _GridSize ("Grid Size (World XZ)", Vector) = (10, 10, 0, 0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            fixed4 _BaseColor;
            fixed4 _LineColor;
            float _CellSize;
            float _LineThickness;
            float4 _GridOrigin;
            float4 _GridSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 localXZ = i.worldPos.xz - _GridOrigin.xz;
                float2 halfThickness = max(_LineThickness * 0.5, 0.0005);
                float2 gridSize = max(_GridSize.xy, 0.0001);

                if (localXZ.x < 0 || localXZ.y < 0 || localXZ.x > gridSize.x || localXZ.y > gridSize.y)
                {
                    return fixed4(0, 0, 0, 0);
                }

                float2 cellCoords = localXZ / max(_CellSize, 0.0001);
                float2 lineDist = abs(frac(cellCoords) - 0.5) * 2.0;

                float xLine = step(1.0 - halfThickness.x,lineDist.x);
                float yLine = step(1.0 - halfThickness.y,lineDist.y);
                float lineMask = (max(xLine, yLine));

                fixed4 color = lerp(_BaseColor, _LineColor, lineMask);
                return color;
            }
            ENDCG
        }
    }
}

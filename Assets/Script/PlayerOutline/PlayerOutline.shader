Shader "Hidden/PlayerOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Range(0.0, 10.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "Outline"
            Tags { "LightMode"="UniversalForward" }

            ZTest Always Cull Front ZWrite On
            Stencil
            {
                Ref 1
                Comp FrontEqual
                Pass Replace
                Fail Keep
                ZFail Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 vertexColor : COLOR;
            };

            fixed4 _OutlineColor;
            float _OutlineThickness;

            v2f vert (appdata v)
            {
                // Calculate the outline offset
                float3 norm = mul((float3x3)unity_ObjectToWorld, v.normal);
                v.vertex.xyz += norm * _OutlineThickness;

                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.vertexColor = _OutlineColor;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return i.vertexColor;
            }
            ENDCG
        }
    }
}

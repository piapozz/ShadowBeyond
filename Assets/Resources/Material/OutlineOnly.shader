Shader "Custom/OutlineOnly"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,1,1,1)
        _OutlineWidth ("Outline Width", Range(0,0.05)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent+10" }

        Cull Front
        ZWrite Off
        Blend SrcAlpha One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _OutlineColor;
            float _OutlineWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float3 offset = normalize(v.normal) * _OutlineWidth;
                v.vertex.xyz += offset;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}

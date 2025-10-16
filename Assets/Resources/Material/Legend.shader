Shader "Custum/Legend"
{
    Properties
    {
        _HueBlendRate("Hue Blend Rate", Range(0.1, 5)) = 2
        _OverrideRate("Override Rate", Range(0, 1)) = 1
        _HueOffset("Hue Offset", float) = 0

        [Space]
        [KeywordEnum(ToShiny, ToDark)] _HUETYPE("HueType", Int) = 0
        [KeywordEnum(Circle1, Circle2, Vertical, Horizontal)] _GRADATIONTYPE("GradationType", Int) = 0

        [Space]
        _MetallicIntensity("Metallic Intensity", Range(0, 2)) = 1.2
        _SpecularPower("Specular Power", Range(1, 128)) = 48
        _SpecularIntensity("Specular Intensity", Range(0, 2)) = 1.5
        _Saturation("Saturation", Range(0, 2)) = 0.7

        [Space]
        _HueRotation("Hue Rotation", Range(0,1)) = 0    // 手動オフセット (0〜1 = 0〜360°)
        _HueRotationSpeed("Hue Rotation Speed", float) = 0.1 // 1秒あたりの回転速度
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _BaseBlend("Base Blend", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGINCLUDE

        #pragma vertex vert
        #pragma fragment frag

        struct appdata
        {
            float4 vertex : POSITION;
            float4 color : COLOR;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            fixed4 color : COLOR;
            float2 uv : TEXCOORD0;
        };

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;

        fixed _HueBlendRate;
        fixed _OverrideRate;
        fixed _HueOffset;

        float _MetallicIntensity;
        float _SpecularPower;
        float _SpecularIntensity;
        float _Saturation;

        float _HueRotation;
        float _HueRotationSpeed;   // 追加
        float4 _BaseColor;
        float _BaseBlend;

        #pragma multi_compile _HUETYPE_TOSHINY _HUETYPE_TODARK
        #pragma multi_compile _GRADATIONTYPE_CIRCLE1 _GRADATIONTYPE_CIRCLE2 _GRADATIONTYPE_VERTICAL _GRADATIONTYPE_HORIZONTAL

        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            o.color = v.color;
            return o;
        }

        // HUE → RGB変換
        fixed3 HUEtoRGB(in float H)
        {
            H = frac(H);
            float R = abs(H * 6 - 3) - 1;
            float G = 2 - abs(H * 6 - 2);
            float B = 2 - abs(H * 6 - 4);
            return saturate(float3(R, G, B));
        }

        // 彩度調整
        fixed3 AdjustSaturation(fixed3 color, float sat)
        {
            float grey = dot(color, float3(0.299, 0.587, 0.114));
            return lerp(grey.xxx, color, sat);
        }

        ENDCG

        Pass
        {
            CGPROGRAM
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= i.color;
                return col;
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, IN.uv) * IN.color;

                // 時間ベースの回転を加算
                float timeRotation = frac(_HueRotation + _Time.y * _HueRotationSpeed);

                fixed hueOffset = max(0, _HueOffset);
                fixed3 hueColor;

#ifdef _GRADATIONTYPE_CIRCLE1
                float pi2 = 6.283184;
                fixed fixX = IN.uv.x - 0.5;
                fixed fixY = IN.uv.y - 0.5;
                float rad = atan2(fixY, fixX);
                rad += pi2;
                rad += hueOffset;
                rad %= pi2;
                rad /= pi2;
                hueColor = (HUEtoRGB(rad + timeRotation) / _HueBlendRate);

#elif _GRADATIONTYPE_CIRCLE2
                fixed fixX = IN.uv.x - 0.5;
                fixed fixY = IN.uv.y - 0.5;             
                fixed minScreen = min(_MainTex_TexelSize.z, _MainTex_TexelSize.w);
                fixX *= (minScreen / _MainTex_TexelSize.w);
                fixY *= (minScreen / _MainTex_TexelSize.z);
                fixed len = sqrt(fixX * fixX + fixY * fixY);
                hueColor = (HUEtoRGB((len + hueOffset + timeRotation) % 1) / _HueBlendRate);

#elif _GRADATIONTYPE_VERTICAL
                hueColor = (HUEtoRGB((IN.uv.y + hueOffset + timeRotation) % 1) / _HueBlendRate);

#elif _GRADATIONTYPE_HORIZONTAL
                hueColor = (HUEtoRGB((IN.uv.x + hueOffset + timeRotation) % 1) / _HueBlendRate);
#endif

#ifdef _HUETYPE_TOSHINY
                color.rgb *= 1 / hueColor;
#elif _HUETYPE_TODARK
                color.rgb *= hueColor;
#endif

                // 彩度調整
                hueColor = AdjustSaturation(hueColor, _Saturation);

                // 指定色をブレンド
                hueColor = lerp(hueColor, _BaseColor.rgb, _BaseBlend);

                // 中心からの距離でリムライト
                float2 centerUV = IN.uv - 0.5;
                float dist = length(centerUV);
                float rim = pow(1.0 - saturate(dist * 2), _SpecularPower);

                float3 specular = rim * _SpecularIntensity;

                // 金属的に混合
                color.rgb = lerp(color.rgb, color.rgb * hueColor + specular, _MetallicIntensity);

                color.a *= _OverrideRate;
                return color;
            }
            ENDCG
        }
    }
}

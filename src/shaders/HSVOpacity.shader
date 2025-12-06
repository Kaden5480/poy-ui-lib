Shader "UILib/HSVOpacity" {
    Properties {
        _MainText ("Base Texture", 2D) = "white" {}
        _Hue ("Hue", Range(0.0, 360.0)) = 0.0
        _Saturation ("Saturation", Range(0.0, 100.0)) = 0.0
        _Value ("Value", Range(0.0, 100.0)) = 0.0
    }

    SubShader {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Pass {
            Tags { "LightMode"="Always" }

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _Hue;
            float _Saturation;
            float _Value;

            struct appdata {
                float4 pos: POSITION;
                float2 texcoord: TEXCOORD0;
            };

            struct v2f {
                float4 pos: POSITION;
                float2 texcoord: TEXCOORD0;
            };

            float hsvf(float h, float s, float v, int n) {
                float k = (n + h/60.0) % 6.0;

                float value = saturate(v - v * s * max(0,
                    min(min(k, 4.0-k), 1.0)
                ));

                return value;
            }

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.texcoord = v.texcoord;
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                float s = _Saturation/100.0;
                float v = _Value/100.0;

                float r = hsvf(_Hue, s, v, 5);
                float g = hsvf(_Hue, s, v, 3);
                float b = hsvf(_Hue, s, v, 1);

                return float4(r, g, b, i.texcoord.y);
            }

            ENDCG
        }
    }

    FallBack "UI/Default"
}

Shader "UILib/HSVRect" {
    Properties {
        _MainText ("Base Texture", 2D) = "white" {}
        _Hue ("Hue", Range(0.0, 360.0)) = 0.0
    }

    SubShader {
        Tags { "RenderType" = "Opaque" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _Hue;

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
                float x = i.texcoord.x;
                float y = i.texcoord.y;

                float r = hsvf(_Hue, x, y, 5);
                float g = hsvf(_Hue, x, y, 3);
                float b = hsvf(_Hue, x, y, 1);

                return float4(r, g, b, 1.0);
            }

            ENDCG
        }
    }

    FallBack "Diffuse"
}

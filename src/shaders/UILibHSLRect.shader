Shader "UILib/HSLRect" {
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

            float hslf(float h, float s, float l, int n) {
                float a = s * min(l, 1.0-l);
                float k = (n + h/30.0) % 12.0;

                float value = saturate(l - a * max(-1.0,
                    min(min(k-3.0, 9.0-k), 1.0)
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
                float y = i.texcoord.y/2;

                float r = hslf(_Hue, x, y, 0);
                float g = hslf(_Hue, x, y, 8);
                float b = hslf(_Hue, x, y, 4);

                return float4(r, g, b, 1.0);
            }

            ENDCG
        }
    }

    FallBack "Diffuse"
}

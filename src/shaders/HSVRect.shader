Shader "UILib/HSVRect" {
    Properties {
        _Hue ("Hue", Range(0.0, 360.0)) = 0.0
        [HideInInspector] _MainTex ("Base Texture", 2D) = "white" {}
    }

    SubShader {
        Tags {
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            float4 _ClipRect;
            float _Hue;

            struct appdata {
                float4 pos: POSITION;
                float2 texcoord: TEXCOORD0;
            };

            struct v2f {
                float4 pos: SV_POSITION;
                float2 texcoord: TEXCOORD0;
                float4 worldPos : TEXCOORD1;
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
                o.worldPos = v.pos;
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                float s = i.texcoord.x;
                float v = i.texcoord.y;

                float r = hsvf(_Hue, s, v, 5);
                float g = hsvf(_Hue, s, v, 3);
                float b = hsvf(_Hue, s, v, 1);

                #ifdef UNITY_UI_CLIP_RECT
                if (any(i.worldPos.xy < _ClipRect.xy)
                    || any(i.worldPos.xy > _ClipRect.zw)
                ) {
                    discard;
                }
                #endif

                return pow(float4(r, g, b, 1.0), 2.2);
            }

            ENDCG
        }
    }

    FallBack "UI/Default"
}

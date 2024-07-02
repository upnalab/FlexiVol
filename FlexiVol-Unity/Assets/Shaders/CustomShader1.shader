Shader "Custom/CustomShader1"
{
   SubShader {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {

            Stencil {
                Ref 2
                Comp equal
                Pass IncrWrap
                ZFail keep
            }

            ZTest less
            Cull Front
            ZWrite OFF

            CGPROGRAM
            #pragma vertex vert 
            #pragma fragment frag alpha
            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
            };
            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            half4 frag(v2f i) : SV_Target {
                return half4(0,0,1,0);
            }
            ENDCG
        }
   
        Pass {

            Stencil {
                Ref 2
                Comp equal
                Pass keep
                ZFail keep
            }

            ZTest less
            Cull Back
            ZWrite OFF

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
            };
            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            half4 frag(v2f i) : SV_Target {
                return half4(1,0.2,0.5,1);
            }
            ENDCG
        }
    } 
}
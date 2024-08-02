Shader "Unlit/PassThroughBlender"
{
    Properties
    {
        _RealWorldTex ("Texture", 2D) = "white" {}
        _PassThroughMaskTex ("VERenderTexture", 2D) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uvVE : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _RealWorldTex;
            float4 _RealWorldTex_ST;

            sampler2D _PassThroughMaskTex;
            float4 _PassThroughMaskTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _RealWorldTex);
                o.uv.x = 1.0 - o.uv.x;
                o.uvVE = TRANSFORM_TEX(v.uv, _PassThroughMaskTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_RealWorldTex, i.uv).bgra;
                fixed4 colVE = tex2D(_PassThroughMaskTex, i.uvVE);
                UNITY_APPLY_FOG(i.fogCoord, col);

                if (colVE.a == 0.0f){
                    discard;
                }
                
                return col;
            }
            ENDCG
        }
    }
}

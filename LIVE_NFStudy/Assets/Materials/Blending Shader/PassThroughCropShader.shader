Shader "Unlit/PassThroughCropShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VERenderTex ("VERenderTexture", 2D) = "" {}
        // _FullRenderTex ("FullRenderTexture", 2D) = "" {}
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
            // make fog work
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
                // float2 uvFull : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _VERenderTex;
            float4 _VERenderTex_ST;

            // sampler2D _FullRenderTex;
            // float4 _FullRenderTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.x = 1.0 - o.uv.x;
                o.uvVE = TRANSFORM_TEX(v.uv, _VERenderTex);
                // o.uvFull = TRANSFORM_TEX(v.uv, _FullRenderTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv).bgra;
                fixed4 colVE = tex2D(_VERenderTex, i.uvVE);
                // fixed4 colFull = tex2D(_FullRenderTex, i.uvFull);

                // apply fog
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

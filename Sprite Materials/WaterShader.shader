Shader "MMM/WaterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _W1 ("Water Texture 1", 2D) = "white" {}
        _W2 ("Water Texture 2", 2D) = "white" {}
        _ox("Offsets", vector) = (0, 0, 0, 0)
    }
    SubShader
    {
      Tags{
        "RenderType" = "Transparent"
        "Queue" = "Transparent"
      }

      Blend SrcAlpha OneMinusSrcAlpha
      ZWrite off
      Cull off
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _W1, _W2;
            float4 _MainTex_ST, _ox;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 uv1 = i.uv;
                uv1.x += _ox.x;
                uv1.y += _ox.y;
                float2 uv2 = i.uv;
                uv2.x += _ox.z;
                uv2.y += _ox.w;
                fixed4 colw1 = tex2D(_W1, uv1);
                fixed4 colw2 = tex2D(_W2, uv2);


                UNITY_APPLY_FOG(i.fogCoord, col);

                fixed4 res = (colw1 + colw2) * .5;
                res.a = col.a;

                return res;
            }
            ENDCG
        }
    }
}

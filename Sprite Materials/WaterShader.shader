Shader "MMM/WaterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _W1 ("Water Texture 1", 2D) = "white" {}
        _W2 ("Water Texture 2", 2D) = "white" {}
        _ox("Offsets", float) = 0
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
            float4 _MainTex_ST;
            float _ox;

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

				half2 uvWave = half2(.5 *  _MainTex_ST.x, .0025 *  _MainTex_ST.y) - i.uv;
				uvWave.x *= _ScreenParams.x / _ScreenParams.y;
				half angWave = (sqrt(dot(uvWave, uvWave)) * 7) - ((_Time.y *  2.5) % 360);
				i.uv.x += normalize(uvWave).x * sin(angWave) * .000275;
				i.uv.y += normalize(uvWave).y * sin(angWave) * .005;

                float str = .5;
                float spd = 3.3;

                float sx = sin(_Time.y * spd);
                float sy = sin(_Time.x * spd * .12345 + .12345) * cos(_Time.x * spd * 12.345);
                float cx = sin(_Time.y * _Time.y * spd);
                float cy = sin(-_Time.x * spd * 1.2345 - .12345) * cos(_Time.x * spd * 1.345);


                fixed4 col1 = fixed4(.4,.7,.9,.7);
                float2 uv1 = i.uv;
                float2 uv2 = i.uv;
                // How far are we from the edges?
                float dx = -4 * uv1.x * (uv1.x - 1);
                float dy = -4 * uv1.y * (uv1.y - 1);
                dy = sqrt(dy);//*dy*dy;

                uv1.x += str * sx * .01 * dx;
                uv1.y += str * sy * .05 * dy;
                uv2.x += str * cx * .08 * dx * dx;
                uv2.y += str * cy * .02 * dy * dy;

                fixed4 col2 = (tex2D(_W1, uv1) + tex2D(_W2, uv2)) * .5;

                fixed4 res = (col1 + col2) * .5;
                res.a = col1.a * frac(1-dx*dy*.2);

                return res;
            }
            ENDCG
        }
    }
}

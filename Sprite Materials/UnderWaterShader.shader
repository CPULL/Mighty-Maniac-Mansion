Shader "MMM/UnderWaterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)		//1
		_WaveAmount("Wave Amount", Range(0, 25)) = 7 //94
		_WaveSpeed("Wave Speed", Range(0, 25)) = 10 //95
		_WaveStrength("Wave Strength", Range(0, 25)) = 7.5 //96
		_WaveX("Wave X Axis", Range(0, 1)) = 0 //97
		_WaveY("Wave Y Axis", Range(0, 1)) = 0.5 //98
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

            sampler2D _MainTex;
            float4 _MainTex_ST, _Color;

			half _WaveAmount, _WaveSpeed, _WaveStrength, _WaveX, _WaveY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				half2 uvWave = half2(_WaveX *  _MainTex_ST.x, _WaveY *  _MainTex_ST.y) - i.uv;
				uvWave.x *= _ScreenParams.x / _ScreenParams.y;
				half angWave = (sqrt(dot(uvWave, uvWave)) * _WaveAmount) - ((_Time.y *  _WaveSpeed) % 360);
				i.uv = i.uv + normalize(uvWave) * sin(angWave) * (_WaveStrength / 1000);
				half4 col = tex2D(_MainTex, i.uv) * _Color;
				half originalAlpha = col.a;
				col.rgb *= col.a;
                return col;
            }
            ENDCG
        }
    }
}

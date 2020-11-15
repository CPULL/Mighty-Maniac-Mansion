Shader "MMM/C64Shader"
{

	Properties{
		[HideInInspector] _Color("Tint", Color) = (0, 0, 0, 1)
		[HideInInspector] _MainTex("Texture", 2D) = "white" {}

		[Header(Pixel size and scanlines)]
		_Res("Screen Resolution (320 for C64)", Float) = 320
		[Toggle()] _CRT("CRT scanlines", float) = 0
		[HideInInspector] _UVCenter("_UVCenter", Vector) = (0,0,0,0)

		[Header(C64 Colors)]
		_ColorBlk("C64 Black", Color) = (0.0000,0.0000,0.0000,1)
		_ColorWht("C64 White", Color) = (1,1,1,1)
		_ColorRed("C64 Red", Color) =    (0.5333, 0.0000, 0)
		_ColorCyn("C64 Cyan", Color) =   (0.6666, 1.0000, 0.9333)
		_ColorVio("C64 Violet", Color) = (0.8000, 0.2666, 0.8000)
		_ColorGrn("C64 Green", Color) =  (0.0000, 0.8000, 0.3330)
		_ColorBlu("C64 Blue", Color) =   (0.0000, 0.0000, 0.6666)
		_ColorYel("C64 Yellow", Color) = (0.9333, 0.9333, 0.4666)
		_ColorOrn("C64 Orange", Color) = (0.8666, 0.5333, 0.3333)
		_ColorBrn("C64 Brown", Color) =  (0.4000, 0.2666, 0.0000)
		_ColorLrd("C64 LRed", Color) =   (1.0000, 0.4666, 0.4666)
		_ColorGr1("C64 Grey1", Color) =  (0.2000, 0.2000, 0.2000)
		_ColorGr2("C64 Grey2", Color) =  (0.4666, 0.4666, 0.4666)
		_ColorLGr("C64 LGreen", Color) = (0.6666, 1.0000, 0.4000)
		_ColorLBl("C64 LBlue", Color) =  (0.0000, 0.5333, 1.0000)
		_ColorGr3("C64 Grey3", Color) =  (0.7333, 0.7333, 0.7333)
	}

	SubShader{
		Tags{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha

		ZWrite off
		Cull off

		Pass{
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST, _MainTex_TexelSize;

			fixed4 _Color;
			fixed4 _ColorBlk, _ColorWht, _ColorRed, _ColorCyn;
			fixed4 _ColorVio, _ColorGrn, _ColorBlu, _ColorYel;
			fixed4 _ColorOrn, _ColorBrn, _ColorLrd, _ColorGr1;
			fixed4 _ColorGr2, _ColorLGr, _ColorLBl, _ColorGr3;

			uniform half _Res, _TextQual;
			uniform half4 _UVCenter;
			float _CRT;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			float2 quant(float2 q, float2 v) {
				return floor(q / v) * v;
			}

			float cdist(fixed4 a, fixed4 b) {
				return 
					(a.r - b.r) * (a.r - b.r) +
					(a.g - b.g) * (a.g - b.g) +
					(a.b - b.b) * (a.b - b.b);
			}

			v2f vert(appdata v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}

			float2 quantToWorld(float2 value, float size) {
				float q = 1920 * _MainTex_TexelSize.x / size;
				float2 wp = mul(unity_ObjectToWorld, float4(value, 0, 0));
				wp = quant(wp, q);
				return mul(unity_WorldToObject, float4(wp, 0, 0));
			}

			fixed4 frag(v2f i) : SV_TARGET{
				float2 uv = quantToWorld(i.uv - _UVCenter.xy,  _Res) + _UVCenter.xy;
				fixed4 colOrg = tex2D(_MainTex, uv);

				fixed4 col = _ColorBlk;
				float minDist = cdist(colOrg, _ColorBlk);

				float dist = cdist(colOrg, _ColorWht);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorWht;
				}

				dist = cdist(colOrg, _ColorRed);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorRed;
				}

				dist = cdist(colOrg, _ColorCyn);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorCyn;
				}

				dist = cdist(colOrg, _ColorVio);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorVio;
				}

				dist = cdist(colOrg, _ColorGrn);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorGrn;
				}

				dist = cdist(colOrg, _ColorBlu);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorBlu;
				}

				dist = cdist(colOrg, _ColorYel);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorYel;
				}

				dist = cdist(colOrg, _ColorOrn);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorOrn;
				}

				dist = cdist(colOrg, _ColorBrn);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorBrn;
				}

				dist = cdist(colOrg, _ColorLrd);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorLrd;
				}

				dist = cdist(colOrg, _ColorGr1);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorGr1;
				}

				dist = cdist(colOrg, _ColorGr2);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorGr2;
				}

				dist = cdist(colOrg, _ColorLGr);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorLGr;
				}

				dist = cdist(colOrg, _ColorLBl);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorLBl;
				}

				dist = cdist(colOrg, _ColorGr3);
				if (dist < minDist) {
					minDist = dist;
					col = _ColorGr3;
				}

				col = colOrg;

				if (_CRT != 0) {
					int y = (int)(i.position.y * 0.256);
					if (y % 2 == 1) {
						col.r *= .8;
						col.g *= .8;
						col.b *= .8;
					}
				}

				col.a = colOrg.a;
				col *= _Color;
				col *= i.color;
				return col;
			}

			ENDCG
		}
	}
}

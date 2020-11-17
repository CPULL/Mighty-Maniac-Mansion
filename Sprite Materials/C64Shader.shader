Shader "MMM/C64Shader"
{

	Properties{
		[HideInInspector] _Color("Tint", Color) = (0, 0, 0, 1)
		[HideInInspector] _MainTex("Texture", 2D) = "white" {}
		[HideInInspector] _UVCenter("_UVCenter", Vector) = (0,0,0,0)

		[Header(Pixelize)]
		_Res("Screen Resolution in pixels\n(320 for true C64, 0 to disable)", Float) = 320

		[Header(C64 Colors)]
		[Toggle()] _UseC64Cols("Use C64 colors", float) = 1
		[Toggle()] _UseExC64Cols("Use Extended C64 colors", float) = 0

		[Header(C64 Color values)]
		_ColorBlk("C64 Black", Color) = (0.0000,0.0000,0.0000,1)
		_ColorWht("C64 White", Color) = (1,1,1,1)
		_ColorRed("C64 Red", Color) = (0.5333, 0.0000, 0)
		_ColorCyn("C64 Cyan", Color) = (0.6666, 1.0000, 0.9333)
		_ColorVio("C64 Violet", Color) = (0.8000, 0.2666, 0.8000)
		_ColorGrn("C64 Green", Color) = (0.0000, 0.8000, 0.3330)
		_ColorBlu("C64 Blue", Color) = (0.0000, 0.0000, 0.6666)
		_ColorYel("C64 Yellow", Color) = (0.9333, 0.9333, 0.4666)
		_ColorOrn("C64 Orange", Color) = (0.8666, 0.5333, 0.3333)
		_ColorBrn("C64 Brown", Color) = (0.4000, 0.2666, 0.0000)
		_ColorLrd("C64 LRed", Color) = (1.0000, 0.4666, 0.4666)
		_ColorGr1("C64 Grey1", Color) = (0.2000, 0.2000, 0.2000)
		_ColorGr2("C64 Grey2", Color) = (0.4666, 0.4666, 0.4666)
		_ColorLGr("C64 LGreen", Color) = (0.6666, 1.0000, 0.4000)
		_ColorLBl("C64 LBlue", Color) = (0.0000, 0.5333, 1.0000)
		_ColorGr3("C64 Grey3", Color) = (0.7333, 0.7333, 0.7333)

		[Header(Outlines)]
		[Toggle()] _UseOutline("Use Outlines", float) = 0
		_OutlineSize("Outline size", Range(0, 8)) = 1
		_OutlineStrenght("Outline Strenght", Range(0, 1)) = 1

		[Header(Scanlines)]
		[Toggle()] _CRT("Use CRT scanlines", float) = 1
		_CRTFreq("CRT scanlines frequency", Range(0, .5)) = 0
		[Toggle()] _CRTDir("Scanlines go down", float) = 1
		_CRTSpeed("Scanlines Speed", Range(0, 4)) = 0
		_CRTNoise("CRT noise", Range(0, 10)) = 0
		_CRTStrenght("Scanlines strenght", Range(0, 1)) = .8
		[Toggle()] _CRTInternalce("Interlace (Seizure Warning!)", float) = 0

		[Header(Luminosity)]
		[Toggle()] _Selected("Selected", float) = 0
		[Toggle()] _FlashLight("Flashlight", float) = 0
		[Toggle()] _NoLights("No lights", float) = 0
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
			float _UseC64Cols, _UseExC64Cols;
			float _UseOutline, _OutlineSize, _OutlineStrenght;
			float _CRT, _CRTFreq, _CRTDir, _CRTSpeed, _CRTNoise, _CRTStrenght, _CRTInternalce;
			float _Selected, _FlashLight, _NoLights;


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
				fixed4 col; // The final color to use
				float2 uv; // The uv to use


				// Handle quantization -------------------------------------------------------------------------------------------------------------------------------------
				if (_Res == 0 || _Res == 1920) {
					uv = i.uv; 
				}
				else {
					uv = quantToWorld(i.uv - _UVCenter.xy, _Res) + _UVCenter.xy;
				}


				// Handle colors -------------------------------------------------------------------------------------------------------------------------------------
				fixed4 colOrg = tex2D(_MainTex, uv);
				if (_UseC64Cols == 0) {
					col = colOrg;
				}
				else {
					fixed4 colors[16];
					colors[0] = _ColorBlk;
					colors[1] = _ColorWht;
					colors[2] = _ColorRed;
					colors[3] = _ColorCyn;
					colors[4] = _ColorVio;
					colors[5] = _ColorGrn;
					colors[6] = _ColorBlu;
					colors[7] = _ColorYel;
					colors[8] = _ColorOrn;
					colors[9] = _ColorBrn;
					colors[10] = _ColorLrd;
					colors[11] = _ColorGr1;
					colors[12] = _ColorGr2;
					colors[13] = _ColorLGr;
					colors[14] = _ColorLBl;
					colors[15] = _ColorGr3;

					float minDist = 999999;
					for (uint pos = 0; pos < 16; pos++) {
						float dist = cdist(colOrg, colors[pos]);
						if (dist < minDist) {
							minDist = dist;
							col = colors[pos];
						}
					}

					if (_UseExC64Cols != 0) { // Second pass with half colors
						for (uint pos = 0; pos < 16; pos++) {
							float dist = cdist(colOrg, colors[pos] * .5);
							if (dist < minDist) {
								minDist = dist;
								col = colors[pos] * .5;
							}
						}
					}

				}

				// Handle outline -------------------------------------------------------------------------------------------------------------------------------------
				if (_UseOutline != 0) {
					half4 outlineC = col * _OutlineStrenght;
					outlineC.a = col.a;

					fixed ua = tex2D(_MainTex, uv + fixed2(0, _MainTex_TexelSize.y) * _OutlineSize).a;
					fixed da = tex2D(_MainTex, uv - fixed2(0, _MainTex_TexelSize.y) * _OutlineSize).a;
					fixed la = tex2D(_MainTex, uv - fixed2(_MainTex_TexelSize.x, 0) * _OutlineSize).a;
					fixed ra = tex2D(_MainTex, uv + fixed2(_MainTex_TexelSize.x, 0) * _OutlineSize).a;

					half4 resc = lerp(outlineC, col, ceil(ua * da * la * ra));
					resc = (resc + col) * .5;
					resc.a = col.a;

					col = resc;
				}


				// Handle scanlines -------------------------------------------------------------------------------------------------------------------------------------
				if (_CRT != 0) {
					int y = 0;

					float noise = clamp(_CRTNoise * (-_Time.z + 3 * _SinTime.y - 4 * _CosTime.z), 0, 1);

					if (_CRTDir == 0) { // Up
						y = (int)((i.position.y + (_Time.w + noise) * _CRTSpeed) * _CRTFreq);

					}
					else { // Down
						y = (int)((i.position.y - (_Time.w + noise) * _CRTSpeed) * _CRTFreq);
					}

					if (_CRTInternalce == 0) {
						if (y & 1 == 1) {
							col.rgb *= _CRTStrenght;
						}
					}
					else {
						if (_Time.x % 0.00025 < 0.000125) {
							if (y & 1 == 1) {
								col.rgb *= _CRTStrenght;
							}
						}
						else {
							if (y & 1 != 1) {
								col.rgb *= _CRTStrenght;
							}
						}

					}
				}


				// _Selected -------------------------------------------------------------------------------------------------------------------------------------

				if (_Selected != 0) {
					// Outline and glow
					col = col + _ColorCyn;
					col *= .5;

					fixed ua = tex2D(_MainTex, uv + fixed2(0, _MainTex_TexelSize.y) * 5).a;
					fixed da = tex2D(_MainTex, uv - fixed2(0, _MainTex_TexelSize.y) * 5).a;
					fixed la = tex2D(_MainTex, uv - fixed2(_MainTex_TexelSize.x, 0) * 5).a;
					fixed ra = tex2D(_MainTex, uv + fixed2(_MainTex_TexelSize.x, 0) * 5).a;
					half4 resc = lerp(_ColorCyn, col, ceil(ua * da * la * ra));
					resc = (resc + col) * .5;
					resc.a = col.a;
					col = resc;
				}

				// _FlashLight -------------------------------------------------------------------------------------------------------------------------------------

				if (_FlashLight != 0 && _NoLights == 0) {
					// Lower saturation, lower luminosity, increase outline but with gray border

					float mean = (col.r + col.g + col.b) / 3;
					fixed4 mc;
					mc.r = mean;
					mc.g = mean;
					mc.b = mean;
					mc.a = col.a;

					float size = 3;
					if (_Selected == 0) {
						col = col + mc;
						col *= .35;
					}
					else {
						col = col + col + mc + _ColorCyn;
						col *= .17;
						size = 4;
					}

					half4 outlineC = _ColorWht;
					outlineC.a = col.a;
					fixed ua = tex2D(_MainTex, uv + fixed2(0, _MainTex_TexelSize.y) * size).a;
					fixed da = tex2D(_MainTex, uv - fixed2(0, _MainTex_TexelSize.y) * size).a;
					fixed la = tex2D(_MainTex, uv - fixed2(_MainTex_TexelSize.x, 0) * size).a;
					fixed ra = tex2D(_MainTex, uv + fixed2(_MainTex_TexelSize.x, 0) * size).a;
					half4 resc = lerp(outlineC, col, ceil(ua * da * la * ra));
					resc = (resc + col) * .5;
					resc.a = col.a;
					col = resc;
				}

				// _NoLights -------------------------------------------------------------------------------------------------------------------------------------

				if (_NoLights != 0) {
					// Lower saturation, low luminosity, increase outline but with gray border

					float mean = (col.r + col.g + col.b) / 3;
					fixed4 mc;
					mc.r = mean;
					mc.g = mean;
					mc.b = mean;
					mc.a = col.a;

					float size = 2;
					if (_Selected == 0) {
						col = col + mc + mc + mc;
						col *= .013;
					}
					else {
						col = col + mc + _ColorCyn;
						col *= .05;
						size = 4;
					}

					half4 outlineC = mc + _ColorWht;
					outlineC *= .125;
					outlineC.a = col.a;
					fixed ua = tex2D(_MainTex, uv + fixed2(0, _MainTex_TexelSize.y) * size).a;
					fixed da = tex2D(_MainTex, uv - fixed2(0, _MainTex_TexelSize.y) * size).a;
					fixed la = tex2D(_MainTex, uv - fixed2(_MainTex_TexelSize.x, 0) * size).a;
					fixed ra = tex2D(_MainTex, uv + fixed2(_MainTex_TexelSize.x, 0) * size).a;
					half4 resc = lerp(outlineC, col, ceil(ua * da * la * ra));
					if (_Selected == 0) {
						resc = (resc + col) * .5;
					}
					else {
						resc = (resc + resc + resc + col) * .25;
					}
					resc.a = col.a;
					col = resc;
				}


				// Finalize
				col.a = colOrg.a;
				col *= _Color;
				col *= i.color;

				return col;
			}

			ENDCG
		}
	}
}

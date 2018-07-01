Shader "Gabo7/Enviroment/DemonTree" {
	Properties {
		_ColorTint ("Color Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpScale("Normal Intensity", Range(0.01,7)) = 1.0
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
		_PulseSpeed ("Pulse Speed",Float) = 1
		_OutlineTexture("Outline Texture", 2D) = "white" {}
		_OutlineTint("Outline Tint", Color) = (1,1,1)
		_OutlineWidth("Outline Width", Range(0.0,5.0)) = 1.00
		_Transparency("Transparency", Range(0.0, 1.0)) = 1.0

		/*_GridSize("GridSize", float) = 1.0
		_X("Seed X", float) = 1.0
		_Y("Seed Y", float) = 1.0*/
	}

		CGINCLUDE
#include "UnityCG.cginc"
		struct Input {
			float4 color : COLOR;
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};
		struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};
		struct v2f {
			float4 pos : POSITION;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0; // texture coordinate
		};

		float4 _ColorTint;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _OutlineTexture;
		float4 _RimColor;
		float _RimPower;
		float _BumpScale;
		float _PulseSpeed;
		float _OutlineWidth;
		float4 _OutlineTint;
		float _Transparency;

		v2f vert(appdata v) {
			v.vertex.xyz *= _OutlineWidth;
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv+(_Time.x/7);
			return o;
		}
		
			
		ENDCG


	SubShader {

		Pass
		{
			
				ZWrite Off
				//Blend One One
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				//#pragma vertex vert_img
				#pragma fragment frag
			

				/*float4 permute(float4 x)
			{
				return fmod(34.0 * pow(x, 2) + x, 289.0);
			}

			float2 fade(float2 t) {
				return 6.0 * pow(t, 5.0) - 15.0 * pow(t, 4.0) + 10.0 * pow(t, 3.0);
			}

			float4 taylorInvSqrt(float4 r) {
				return 1.79284291400159 - 0.85373472095314 * r;
			}

			#define DIV_289 0.00346020761245674740484429065744f

			float mod289(float x) {
				return x - floor(x * DIV_289) * 289.0;
			}


				float PerlinNoise2D(float2 P)
			{
				float4 Pi = floor(P.xyxy) + float4(0.0, 0.0, 1.0, 1.0);
				float4 Pf = frac(P.xyxy) - float4(0.0, 0.0, 1.0, 1.0);

				float4 ix = Pi.xzxz;
				float4 iy = Pi.yyww;
				float4 fx = Pf.xzxz;
				float4 fy = Pf.yyww;

				float4 i = permute(permute(ix) + iy);

				float4 gx = frac(i / 41.0) * 2.0 - 1.0;
				float4 gy = abs(gx) - 0.5;
				float4 tx = floor(gx + 0.5);
				gx = gx - tx;

				float2 g00 = float2(gx.x,gy.x);
				float2 g10 = float2(gx.y,gy.y);
				float2 g01 = float2(gx.z,gy.z);
				float2 g11 = float2(gx.w,gy.w);

				float4 norm = taylorInvSqrt(float4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
				g00 *= norm.x;
				g01 *= norm.y;
				g10 *= norm.z;
				g11 *= norm.w;

				float n00 = dot(g00, float2(fx.x, fy.x));
				float n10 = dot(g10, float2(fx.y, fy.y));
				float n01 = dot(g01, float2(fx.z, fy.z));
				float n11 = dot(g11, float2(fx.w, fy.w));

				float2 fade_xy = fade(Pf.xy);
				float2 n_x = lerp(float2(n00, n01), float2(n10, n11), fade_xy.x);
				float n_xy = lerp(n_x.x, n_x.y, fade_xy.y);
				return 2.3 * n_xy;
			}*/

				fixed4 frag(v2f i) : SV_Target         //OLD
			{
				// sample texture and return it
				fixed4 col = tex2D(_OutlineTexture, i.uv) * _OutlineTint;
				//col.a = _Transparency;
				//col.a *= abs(_SinTime.z);
				
				return col;
			}

			/*float _GridSize;          //Perlin noise
			float _X;
			float _Y;

			fixed4 frag(v2f_img i) : SV_Target
			{
				i.uv *= _GridSize;
				i.uv += float2(_X, _Y);
				float ns = PerlinNoise2D(i.uv) / 2 + 0.5f;
				return float4(ns, ns, ns, 1.0f);
			}*/

				ENDCG
		}
		

		ZWrite On
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert



		void surf(Input IN, inout SurfaceOutput o) {
			half rim;
			half test;

			IN.color = _ColorTint;
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * IN.color;
			fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			normal.z = normal.z / _BumpScale;
			o.Normal = normalize(normal);
			test = _PulseSpeed * abs(_SinTime.y);
			rim = (1.0 - saturate(dot(normalize(IN.viewDir), o.Normal))) * test;
			//rim = (1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)) ) * abs(_SinTime.y);
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}
		ENDCG
		
	}
	FallBack "Diffuse"
}

Shader "Custom/QuadBendShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_UpperScale ("Upper Scale", Range(0, 1)) = 1
		_LowerScale ("Lower Scale", Range(0, 1)) = 0.6
		_Color("Color", Color) = (27, 212, 56, 0.5)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Transparent" }
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float _UpperScale;
			float _LowerScale;
			float4 _Color;

			v2f vert (appdata v)
			{
				v2f o;

				float avg = (_UpperScale + _LowerScale)/2.0;

				v.vertex.x *=  avg + (v.vertex.y * (_UpperScale - avg)*100);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//fixed4 col = tex2D(_MainTex, i.uv);

				fixed4 c = tex2D (_MainTex, i.uv) * _Color;
				//o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				//o.Metallic = _Metallic;
				//o.Smoothness = _Glossiness;
				//o.Alpha = c.a;
				//o.Alpha = 0.5;
			
				// just invert the colors
				//col.rgb = _Color;

				return _Color;
				//return float4(i.uv.x % 2, 10, 10, 0.5);

				//return (c * 0.90);
				//if (fmod(i.uv.x, 2) ==0)
				//	return c;
				//else
				//	return 1 - c;
			
				//return _Color;
			}
			ENDCG
		}
	}
}

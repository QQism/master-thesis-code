﻿Shader "Custom/QuadDistortionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_UpperScale ("Upper Scale", Range(0, 5)) = 1
		_LowerScale ("Lower Scale", Range(0, 5)) = 0.6
		_CustomColor("Custom Color", Color) = (.34, .85, .92, 0.5)
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
			float4 _CustomColor; 

			v2f vert (appdata v)
			{
				v2f o;

				float avg = (_UpperScale + _LowerScale)/2.0;

				v.vertex.x *=  avg + (v.vertex.z * (_UpperScale - avg));

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return _CustomColor;
			}
			ENDCG
		}
	}
}

Shader "Custom/ConeBarShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Transparency("Transparency", Range(0, 1)) = 0.5
		_OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineWidth ("Outline Width", Range(1.0, 10.0)) = 1.1
		_OutlineOn("Outline On", Int) = 0
	}

	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Transparent+1" }

		Pass 
		{
			Name "Outline"
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
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
			float4 _OutlineColor;
			float _OutlineWidth;
			int _OutlineOn;

			v2f vert (appdata v)
			{
				if (_OutlineOn)
					v.vertex.xzy *= _OutlineWidth;

				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i): SV_TARGET
			{
				float4 c;

				if (_OutlineOn)
				{
					c = tex2D(_MainTex, i.uv);
					c *= _OutlineColor;// float4(1, 1, 1, 1);
				}
				else
				{
					c = float4(1, 1, 1, 0);
				}

				return c;
			}
			ENDCG
		}

		Cull Front

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _Transparency;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			//o.Alpha = c.a;
			o.Alpha = _Transparency;
			//o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

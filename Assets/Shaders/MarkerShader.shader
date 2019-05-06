Shader "Custom/MarkerShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ColorMain ("Color Main", Color) = (1,1,1,1)
		_ColorTint ("Color Tint", Color) = (1,1,1,1)
		_ColorOutline ("Color Outline", Color) = (1,1,1,1)
		_OutlineWidth ("Outline Width", Range(1.0, 10.0)) = 1.1
		_OutlineOn("Outline On", Int) = 0
		_OutlineAlpha("Outline Transparency", Range(0, 1)) = 0.3
	}
	CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		//float4 _MainTex_ST;
		float4 _ColorMain;
		float4 _ColorTint;
		float4 _ColorOutline;
		float _OutlineWidth;
		int _OutlineOn;
		float _OutlineAlpha;
			
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

		v2f vert (appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;//TRANSFORM_TEX(v.uv, _MainTex);
			UNITY_TRANSFER_FOG(o,o.vertex);
			return o;
		}
		
		fixed4 frag1 (v2f i) : SV_Target
		{
			// sample the texture
			fixed4 col = tex2D(_MainTex, i.uv) * _ColorMain;
			//col.a = lerp(1, 0, i.uv.y * _Time);
			col.a = lerp(0.8, 0.5, i.uv.y);
			//col.a = 0.5;
			// apply fog
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
		}

		fixed4 frag2 (v2f i) : SV_Target
		{
			// sample the texture
			fixed4 col = tex2D(_MainTex, i.uv) * _ColorTint;
			//col.a = lerp(1, 0, i.uv.y * _Time);
			col.a = lerp(0.1, 0.25, i.uv.y/2 * _CosTime.w);
			//col.a = 0.5;
			// apply fog
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
		}


		v2f vertOutline (appdata v)
		{
			if (_OutlineOn)
				v.vertex.xzy *= _OutlineWidth;

			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		fixed4 fragOutline(v2f i): SV_TARGET
		{
			float4 c;

			if (_OutlineOn)
			{
				c = tex2D(_MainTex, i.uv);
				c *= _ColorOutline;
				c.a = _OutlineAlpha;
			}
			else
			{
				c = tex2D(_MainTex, i.uv);
				c.a = 0;
			}

			return c;
		}

	ENDCG
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Transparent" }


		Pass
		{
			Name "One"
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 100
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag1	
			ENDCG
		}

		Pass
		{
			Name "Two"
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 100
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag2			
			ENDCG
		}
		Pass
		{
			Name "Outline"
			Blend SrcAlpha OneMinusSrcAlpha
			//Blend One One
			//Blend OneMinusDstColor One
			LOD 100
			CGPROGRAM
			#pragma vertex vertOutline
			#pragma fragment fragOutline
			ENDCG
		}
	}
}

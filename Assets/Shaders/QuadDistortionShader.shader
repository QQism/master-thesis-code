﻿Shader "Custom/QuadDistortionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_UpperScale ("Upper Scale", Range(0, 5)) = 1
		_LowerScale ("Lower Scale", Range(0, 5)) = 0.6
		_CustomColor("Custom Color", Color) = (.34, .85, .92, 1)
		_Transparency("Transparency", Range(0, 1)) = 0.8
		_TickColor("Tick Color", Color) = (.75, .79, 0.8, 1)
		_TickTransparency("Tick Transparency", Range(0, 1)) = 1
		_TickThickness("Tick Thickness", Range(0, 1)) = 0.05
		_LevelScale("Scale Level", Range(0, 1)) = 0.5
		_TickStep("Tick Step", Range(0, 1)) = 0.25
		_OnTop("On Top?", Int) = 0
		_OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineWidth ("Outline Width", Range(1.0, 10.0)) = 1.02
		_OutlineOn("Outline On", Int) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		// No culling or depth
		Cull Off
		ZWrite Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

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
			float _UpperScale;
			float _LowerScale;

			v2f vert (appdata v)
			{

				// To avoid warning "not completely initialized"
				v2f o = (v2f)0;

				// Distorting the quad to become a traperzoid
				float avg = (_UpperScale + _LowerScale)/2.0;
				v.vertex.x *=  avg + (v.vertex.z * (_UpperScale - avg));

				if (_OutlineOn)
					v.vertex.x *= _OutlineWidth;

				//v2f o;
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
					//c *= _OutlineColor;// float4(1, 1, 1, 1);
					c *= float4(1, 1, 1, 1);
				}
				else
				{
					c = float4(1, 1, 1, 0);
				}

				return c;
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 og_vertex : TEXCOORD1;
			};

			sampler2D _MainTex;
			float _UpperScale;
			float _LowerScale;
			float4 _CustomColor; 
			float _Transparency;
			float4 _TickColor;
			float _TickTransparency;
			float _TickThickness;
			float _LevelScale;
			float _TickStep;
			int _OnTop;

			v2f vert (appdata v)
			{
				// To avoid warning "not completely initialized"
				v2f o = (v2f)0;

				// Distorting the quad to become a traperzoid
				float avg = (_UpperScale + _LowerScale)/2.0;
				v.vertex.x *=  avg + (v.vertex.z * (_UpperScale - avg));

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.og_vertex = v.vertex;
				return o;
			}

			float when_lt(float x, float y)
			{
				return max(sign(y - x), 0.0);
			}

			float when_gt(float x, float y)
			{
				return max(sign(x - y), 0.0);
			}

			float when_ge(float x, float y)
			{
				return 1.0 - when_lt(x, y);
			}

			float when_le(float x, float y)
			{
				return 1.0 - when_gt(x, y);
			}

			float and(float x, float y)
			{
				return x * y;
			}

			float or(float x, float y)
			{
				return min(x + y, 1.0);
			}

			fixed4 frag (v2f i) : SV_Target
			{ 
				fixed4 col;

				float offset, full_range, step, bottom_end, tick_pos, topOffset, bottomOffset;

				offset = 2 * (1 - _LevelScale) / _LevelScale;
				full_range = offset + 2;
				step = _TickStep * full_range;

				bottom_end = offset * _OnTop + 1;
				tick_pos = (i.og_vertex.z + bottom_end) % step;
				topOffset = i.og_vertex.z + bottom_end + _TickThickness;
				bottomOffset = i.og_vertex.z + bottom_end - _TickThickness;
	
				// 1 if is tick, 0 if not 
				float condition = and(and(
										when_lt(topOffset, full_range),
										when_gt(bottomOffset, 0)),
									or(
										when_ge(tick_pos, step - _TickThickness),
										when_le(tick_pos, _TickThickness)));
				float4 color = (_TickColor * condition + _CustomColor * !condition);
				float transparency = (_TickTransparency * condition + _Transparency * !condition);

				//col = tex2D(_MainTex, i.uv) * _CustomColor;
				col = tex2D(_MainTex, i.uv) * color;
				col.a = transparency;

				/*
				if ((topOffset < full_range && bottomOffset > 0) &&
					(tick_pos >= (step - _TickThickness) || tick_pos <= _TickThickness))
				{
					col = tex2D(_MainTex, i.uv) * _TickColor;
					col.a = _TickTransparency;
				} else
				{
					col = tex2D(_MainTex, i.uv) * _CustomColor;
					col.a = _Transparency;
				}
				*/		

				//col = tex2D(_MainTex, i.uv) * _CustomColor;
				//col.a = _Transparency;
				return col;
			}
			ENDCG
		}

	}
}

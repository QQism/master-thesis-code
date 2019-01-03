﻿Shader "Custom/QuadDistortionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_UpperScale ("Upper Scale", Range(0, 5)) = 1
		_LowerScale ("Lower Scale", Range(0, 5)) = 0.6
		_CustomColor("Custom Color", Color) = (.34, .85, .92, 1)
		_Transparency("Transparency", Range(0, 1)) = 0.8
		_TickColor("Tick Color", Color) = (0, 0, 0, 1)
		_TickThickness("Tick Thickness", Range(0, 1)) = 0.05
		_LevelScale("Scale Level", Range(0, 1)) = 0.5
		_TickStep("Tick Step", Range(0, 1)) = 0.25
		_OnTop("On Top?", Int) = 0
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
			float _TickThickness;
			float _LevelScale;
			int _OnTop;
			float _TickStep;

			v2f vert (appdata v)
			{
				v2f o;

				float avg = (_UpperScale + _LowerScale)/2.0;

				v.vertex.x *=  avg + (v.vertex.z * (_UpperScale - avg));

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.og_vertex = v.vertex;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{ 
				float thickness = 0.05;
				//fixed4 col = tex2D(_MainTex, i.uv) * _TickColor;

				int top = 0;
				float level = 0.3;
				float og_step = 0.25;
				float4 color = _CustomColor;

				float full_range, step, temp, topOffset, bottomOffset;

				top = _OnTop;
				level = _LevelScale;
				og_step = _TickStep;
				thickness = _TickThickness;

				/*	
				step = (og_step * 2);
				temp = (i.og_vertex.z + 1) % step;
				topOffset = i.og_vertex.z + thickness;
				bottomOffset = i.og_vertex.z - thickness;
				

				if ((topOffset < 1 && bottomOffset > -1)
					 && (temp >= (step - thickness) || temp <= thickness))
					color = _TickColor;
				*/

				// Bottom
				/*
				float full_range = 1 + 2 * (1 - level) / level;
				float step = og_step * full_range;
				float temp = (i.og_vertex.z + 1) % step;
				float bottomOffset = i.og_vertex.z - thickness;

				if ((bottomOffset > -1)
					&& (temp >= (step - thickness) || temp <= thickness))
					color = _TickColor;
				*/

				// Top
				/*
				float bottom_end = (1 + 2 * level / (1 - level));
				full_range = bottom_end + 1;
				step = og_step * full_range;
				temp = (i.og_vertex.z + bottom_end) % step;
				topOffset = i.og_vertex.z + thickness;

				if ((topOffset < 1) &&
					(temp >= (step - thickness) || temp <= thickness))
					color = _TickColor;
				*/

				float bottom_end;
				
				if (top)
				{
					//bottom_end = (1 + 2 * level / (1 - level));
					bottom_end = 1 + 2 * (1 - level) / level;
					full_range = bottom_end + 1;
					step = og_step * full_range;
					temp = (i.og_vertex.z + bottom_end) % step;
					topOffset = i.og_vertex.z + bottom_end + thickness;
					bottomOffset = i.og_vertex.z + bottom_end - thickness;

					if ((topOffset < full_range && bottomOffset > 0) &&
						(temp >= (step - thickness) || temp <= thickness))
						color = _TickColor;

				} else
				{
					bottom_end = 1 + 2 * (1 - level) / level;
					full_range = bottom_end + 1;
					step = og_step * full_range;
					temp = (i.og_vertex.z + 1) % step;
					topOffset = i.og_vertex.z + 1 + thickness;
					bottomOffset = i.og_vertex.z + 1 - thickness;

					if ((topOffset < full_range && bottomOffset > 0)
						&& (temp >= (step - thickness) || temp <= thickness))
						color = _TickColor;
				}
			

				fixed4 col = tex2D(_MainTex, i.uv) * color;

				col.a = _Transparency;
				return col;
			}
			ENDCG
		}
	}
}

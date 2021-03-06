﻿Shader "Custom/MapTextureShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_UpperScale ("Upper Scale", Range(0, 5)) = 1
		_LowerScale ("Lower Scale", Range(0, 5)) = 0.6
		_Transparency("Transparency", Range(0, 1)) = 0.8
		_RotationAngle("Rotation Angle Degree", Range(0, 360)) = 0
		_MiterAngle("Miter Angle Degree", Range(0, 360)) = 90
		_QuadsCount("Quads Count", Int) = 4
		_RotationAngle("Rotation Angle Degree", Range(0, 360)) = 0
		_MiterAngle("Miter Angle Degree", Range(0, 360)) = 90
		_QuadsCount("Quads Count", Int) = 4
		_TickColor("Tick Color", Color) = (.75, .79, 0.8, 1)
		_TickTransparency("Tick Transparency", Range(0, 1)) = 0.8
		_TickThickness("Tick Thickness", Range(0, 1)) = 0.005
		_TickStep("Tick Step", Range(0, 1)) = 0.2
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		// No culling or depth
		Cull Off
		ZWrite Off
		//ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

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
				float4 og_vertex: TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _UpperScale;
			float _LowerScale;
			float _Transparency;
			float _RotationAngle;
			float _MiterAngle;
			uint _QuadsCount;
			float4 _TickColor;
			float _TickTransparency;
			float _TickThickness;
			float _TickStep;
			
			v2f vert (appdata v)
			{
				// To avoid warning "not completely initialized"
				v2f o = (v2f)0;

				// compute angle of vertex relative to center of texture
				float quadAngle = radians(360/_QuadsCount);
				float vx, vy;
				vx = v.vertex.x;
				vy = v.vertex.z;

				float alpha = v.vertex.x * quadAngle;

				float rotationRad = -(alpha + radians(_RotationAngle - 90));				
				float s = sin(rotationRad);
				float c = cos(rotationRad);					

				// Distorting the quad to become a traperzoid
				float avg = (_UpperScale + _LowerScale)/2.0;
				v.vertex.x *=  avg + (v.vertex.z * (_UpperScale - avg));

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.og_vertex = v.vertex;

				float dx = v.vertex.x;
				// quad coordinates are relative to center of quad
				// move y coordinate origin to lower border of the quad
				// scale y coordinate to the range between 0 and 1
				float dy = 0.5 * v.vertex.z + 0.5;
				// radius in polar coordinate system
				// scale radius to range between 0 and 0.5
				float r = sqrt(dx * dx + dy * dy) * 0.5;
				o.uv.x = 0.5 + r * c;
				o.uv.y = 0.5 + r * s;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{ 
				fixed4 col;

				float offset, full_range, step, bottom_end, tick_pos, topOffset, bottomOffset;

				offset = 0;
				full_range = offset + 2;
				step = _TickStep * full_range;

				//bottom_end = offset * _OnTop + 1;
				bottom_end = 1;
				tick_pos = (i.og_vertex.z + bottom_end) % step;
				topOffset = i.og_vertex.z + bottom_end + _TickThickness;
				bottomOffset = i.og_vertex.z + bottom_end - _TickThickness;

				// Turn off tick
				if (0 && ((topOffset < full_range && bottomOffset > 0) && tick_pos >= (step - _TickThickness) || (tick_pos <= _TickThickness)))
				{
					col = _TickColor;
					col.a = _TickTransparency;
				} else
				{
					col = tex2D(_MainTex, i.uv);
					col.a = _Transparency;
				}

				float diffx = abs(i.uv.x - 0.5);
				float diffy = abs(i.uv.y - 0.5);

				//float radius = 0.1;
				//if (diffx < radius && diffy < radius) 
				//	col = fixed4(1, 0, 0, 1);

				return col;
			}
			ENDCG
		}
	}
}

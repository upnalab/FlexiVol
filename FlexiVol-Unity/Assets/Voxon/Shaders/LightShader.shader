Shader "Voxon/LightShader"{
	//show values to edit in inspector
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}

	}
		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0

			#include "UnityCG.cginc"

			struct POLTEX {
				float3 vertex;
				float2 uv;
				uint col;
			};

			struct DEPTH
			{
				float value;         // Depth value at this point
				int index;      // Data index where depth related value is stored
			};

			uniform RWStructuredBuffer<POLTEX> _Data : register(u1);
			uniform RWStructuredBuffer<DEPTH> _Depth : register(u2);
			RWStructuredBuffer<uint> _Index : register(u3);

			float4x4 _ActiveCamera : register(u4);
			uint _Resolution : register(u5);

			// note: no SV_POSITION in this struct
			struct v2f {
				float4 vertex : COLOR; // vertex position input
				float2 uv : TEXCOORD0;
			};

			v2f vert(
				float4 vertex : POSITION, // vertex position input
				float2 uv : TEXCOORD0, // texture coordinate input
				out float4 outpos : SV_POSITION // clip space position output
				)
			{
				v2f o;
				o.uv = uv;
				outpos = UnityObjectToClipPos(vertex);
				o.vertex = mul(_ActiveCamera, mul(unity_ObjectToWorld, vertex));
				return o;
			}

			float4 _Color;
			sampler2D _MainTex;

			fixed4 frag(v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{ 
				
				fixed4 c;
				c = tex2D(_MainTex, i.uv) * _Color;
			
				// Default action

				// screenPos.xy will contain pixel integer coordinates.
				// use them to implement a checkerboard pattern that skips rendering
				// 4x4 blocks of pixels

				// checker value will be negative for 4x4 blocks of pixels
				// in a checkerboard pattern

				// Save higher values of depth
				if (unity_OrthoParams.w < 1) {
					return c; // Non Orth camera
				}

				if (_ScreenParams.x < 1 || _ScreenParams.y < 1 || screenPos.w < 1) {
					return fixed4(1, 0, 0, 0); // Red, Bad Screen Params
				}


				int ScreenX = floor((screenPos.x / _ScreenParams.x) * _Resolution); // X pixel
				int ScreenY = floor((screenPos.y / _ScreenParams.y) * _Resolution); // Y Pixel
					
				if (ScreenX < 0 || ScreenX > _Resolution || ScreenY < 0 || ScreenY > _Resolution) {
					return fixed4( 0,1,0,0 ); // Green, X / Y outside of range
				}

				int depth_index = ScreenY + ScreenX * _Resolution;
				int data_index = 0;

				int depth = int(screenPos.z / screenPos.w * 512);
				

				int max_rez = _Resolution * _Resolution;
				// Discard for out of range depth
				if (depth_index >= max_rez || depth_index < 0) {
					return fixed4(0, 0, 1, 0);
				}
				

				// Discard Check of new Depth and inside volume (depth requires direction check)

				if (_Depth[depth_index].index > -1 && _Depth[depth_index].value > depth) {
					float d = depth / _Depth[depth_index].value;
					return fixed4(0, 0, 0, 0);; // Values assigned value is higher
				}
				

				if(	abs(i.vertex.x) > 1 
					|| abs(i.vertex.y) > 1 
					|| abs(i.vertex.z) > 1) {
					return fixed4(1, 1, 0, 0); // Out of Cube
				}
				

				/* Update Depth Map*/

				// Get data index
				if (_Depth[depth_index].index < 0) { // New Pixel (index -1 out of range)
					// Need threadsafe index update
					InterlockedAdd(_Index[0], 1, data_index);

					// Don't let us assign an out of range index
					if (data_index < 0 || data_index >= max_rez) {
						return c;
					}

					_Depth[depth_index].index = data_index;

				}
				else { // Old Pixel
					// Maintain old index (we overwrite it)
					data_index = _Depth[depth_index].index;
				}
					
				_Depth[depth_index].value = depth;

				// Vertex is -0.5 -> +0.5, double it, invert x & z
				_Data[data_index].vertex = i.vertex.xzy * -2;
				_Data[data_index].vertex.x *= -1;


				float4 arrangedColor = tex2D(_MainTex, i.uv).rgba * _Color;
				
				int red = int(arrangedColor.r * 255) & 255;
				int green = int(arrangedColor.g * 255) & 255;
				int blue = int(arrangedColor.b * 255) & 255;
				uint col = (red << 16) | (green << 8) | blue;

				_Data[data_index].col = col;
				
				return c;
			}
			ENDCG
		}
	}
}
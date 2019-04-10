// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "InWoFumu/Mat"
{
	Properties
	{
		_Pattern("Pattern", 2D) = "white" {}
		_TouchIntensity("TouchIntensity", Range( 0 , 2)) = 1.274559
		_OverallBrightness("Overall Brightness", Range( 0 , 0.6)) = 0.6
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		

		Pass
		{
			Name "Unlit"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			uniform float _TouchIntensity;
			uniform sampler2D _Pattern;
			uniform float _OverallBrightness;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 finalColor;
				float2 uv2 = i.ase_texcoord.xy * float2( 1,1 ) + float2( 0.03125,0.0625 );
				float pixelWidth5 =  1.0f / 12.0;
				float pixelHeight5 = 1.0f / 8.0;
				half2 pixelateduv5 = half2((int)(uv2.x / pixelWidth5) * pixelWidth5, (int)(uv2.y / pixelHeight5) * pixelHeight5);
				
				
				finalColor = ( ( _TouchIntensity * tex2D( _Pattern, pixelateduv5 ) ) + _OverallBrightness );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	

	
}
/*ASEBEGIN
Version=16100
2400;428;1195;780;1461.704;818.519;1.745886;True;False
Node;AmplifyShaderEditor.Vector2Node;6;-1345.455,-478.5371;Float;False;Constant;_Vector0;Vector 0;0;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;17;-1355.203,-336.654;Float;False;Constant;_Vector1;Vector 1;2;0;Create;True;0;0;False;0;0.03125,0.0625;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;4;-1038.472,-90.00551;Float;False;Constant;_Float1;Float 1;0;0;Create;True;0;0;False;0;8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1050.472,-204.0055;Float;False;Constant;_Float0;Float 0;0;0;Create;True;0;0;False;0;12;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1124.369,-474.3054;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCPixelate;5;-770.631,-451.0695;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;7;-495.369,-474.5742;Float;True;Property;_Pattern;Pattern;0;0;Create;True;0;0;False;0;bf4ff4f5eb7448b4f8a8e0cdfa652391;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-474.4187,-588.7765;Float;False;Property;_TouchIntensity;TouchIntensity;1;0;Create;True;0;0;False;0;1.274559;0.6;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-169.5418,-542.8501;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;16;54.61616,-359.4447;Float;False;Property;_OverallBrightness;Overall Brightness;2;0;Create;True;0;0;False;0;0.6;0.6;0;0.6;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;287.6476,-481.8123;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;725.4974,-193.224;Float;False;True;2;Float;ASEMaterialInspector;0;1;InWoFumu/Mat;0770190933193b94aaa3065e307002fa;0;0;Unlit;2;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;False;0;;4;=;=;=;=;0;Standard;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;2;0;6;0
WireConnection;2;1;17;0
WireConnection;5;0;2;0
WireConnection;5;1;3;0
WireConnection;5;2;4;0
WireConnection;7;1;5;0
WireConnection;13;0;14;0
WireConnection;13;1;7;0
WireConnection;12;0;13;0
WireConnection;12;1;16;0
WireConnection;1;0;12;0
ASEEND*/
//CHKSM=FD80B22E02A8354F3A8FFA4CD0C8905A0BBBDF37
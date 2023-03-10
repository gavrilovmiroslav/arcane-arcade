// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Sprite Shaders Ultimate/Standard/Color/Outer Outline"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[KeywordEnum(Linear_Default,Linear_Scaled,Linear_FPS,Frequency,Frequency_FPS,Custom_Value)] _TimeSettings("Time Settings", Float) = 0
		_TimeScale("Time Scale", Float) = 1
		_TimeFrequency("Time Frequency", Float) = 2
		_TimeRange("Time Range", Float) = 0.5
		_TimeFPS("Time FPS", Float) = 5
		_TimeValue("Time Value", Float) = 0
		_OuterOutlineFade("Outer Outline: Fade", Range( 0 , 1)) = 1
		[HDR]_OuterOutlineColor("Outer Outline: Color", Color) = (0,0,0,1)
		_OuterOutlineWidth("Outer Outline: Width", Float) = 0.04
		[Toggle(_OUTEROUTLINEDISTORTIONTOGGLE_ON)] _OuterOutlineDistortionToggle("Outer Outline: Distortion Toggle", Float) = 0
		_OuterOutlineDistortionIntensity("Outer Outline: Distortion Intensity", Vector) = (0.01,0.01,0,0)
		_OuterOutlineNoiseScale("Outer Outline: Noise Scale", Vector) = (4,4,0,0)
		_OuterOutlineNoiseSpeed("Outer Outline: Noise Speed", Vector) = (0,0.1,0,0)
		[ASEEnd]_OuterOutlineNoiseTexture("Outer Outline: Noise Texture", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		
		Pass
		{
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _OUTEROUTLINEDISTORTIONTOGGLE_ON
			#pragma shader_feature _TIMESETTINGS_LINEAR_DEFAULT _TIMESETTINGS_LINEAR_SCALED _TIMESETTINGS_LINEAR_FPS _TIMESETTINGS_FREQUENCY _TIMESETTINGS_FREQUENCY_FPS _TIMESETTINGS_CUSTOM_VALUE


			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform float _EnableExternalAlpha;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float4 _MainTex_ST;
			uniform float4 _OuterOutlineColor;
			uniform float _OuterOutlineFade;
			uniform sampler2D _OuterOutlineNoiseTexture;
			uniform float _TimeScale;
			uniform float _TimeFPS;
			uniform float _TimeFrequency;
			uniform float _TimeRange;
			uniform float _TimeValue;
			uniform float2 _OuterOutlineNoiseSpeed;
			uniform float2 _OuterOutlineNoiseScale;
			uniform float2 _OuterOutlineDistortionIntensity;
			uniform float _OuterOutlineWidth;
			float4 _MainTex_TexelSize;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				
				
				IN.vertex.xyz +=  float3(0,0,0) ; 
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				fixed4 alpha = tex2D (_AlphaTex, uv);
				color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}
			
			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 temp_output_15_0_g500 = tex2D( _MainTex, uv_MainTex );
				float3 temp_output_82_0_g500 = (_OuterOutlineColor).rgb;
				float temp_output_182_0_g500 = ( ( 1.0 - temp_output_15_0_g500.a ) * min( ( _OuterOutlineFade * 3.0 ) , 1.0 ) );
				float3 lerpResult178_g500 = lerp( (temp_output_15_0_g500).rgb , temp_output_82_0_g500 , temp_output_182_0_g500);
				float3 lerpResult170_g500 = lerp( lerpResult178_g500 , temp_output_82_0_g500 , temp_output_182_0_g500);
				float mulTime5_g503 = _Time.y * _TimeScale;
				float mulTime7_g503 = _Time.y * _TimeFrequency;
				#if defined(_TIMESETTINGS_LINEAR_DEFAULT)
				float staticSwitch1_g503 = _Time.y;
				#elif defined(_TIMESETTINGS_LINEAR_SCALED)
				float staticSwitch1_g503 = mulTime5_g503;
				#elif defined(_TIMESETTINGS_LINEAR_FPS)
				float staticSwitch1_g503 = ( _TimeScale * ( floor( ( _Time.y * _TimeFPS ) ) / _TimeFPS ) );
				#elif defined(_TIMESETTINGS_FREQUENCY)
				float staticSwitch1_g503 = ( ( sin( mulTime7_g503 ) * _TimeRange ) + 100.0 );
				#elif defined(_TIMESETTINGS_FREQUENCY_FPS)
				float staticSwitch1_g503 = ( ( _TimeRange * sin( ( _TimeFrequency * ( floor( ( _TimeFPS * _Time.y ) ) / _TimeFPS ) ) ) ) + 100.0 );
				#elif defined(_TIMESETTINGS_CUSTOM_VALUE)
				float staticSwitch1_g503 = _TimeValue;
				#else
				float staticSwitch1_g503 = _Time.y;
				#endif
				float2 temp_output_7_0_g500 = IN.texcoord.xy;
				#ifdef _OUTEROUTLINEDISTORTIONTOGGLE_ON
				float2 staticSwitch157_g500 = ( ( tex2D( _OuterOutlineNoiseTexture, ( ( ( staticSwitch1_g503 * _OuterOutlineNoiseSpeed ) + temp_output_7_0_g500 ) * _OuterOutlineNoiseScale ) ).r - 0.5 ) * _OuterOutlineDistortionIntensity );
				#else
				float2 staticSwitch157_g500 = float2( 0,0 );
				#endif
				float2 temp_output_131_0_g500 = ( staticSwitch157_g500 + temp_output_7_0_g500 );
				float2 appendResult2_g502 = (float2(_MainTex_TexelSize.z , _MainTex_TexelSize.w));
				float2 temp_output_25_0_g500 = ( 100.0 / appendResult2_g502 );
				float lerpResult168_g500 = lerp( temp_output_15_0_g500.a , min( ( max( max( max( max( max( max( max( tex2D( _MainTex, ( temp_output_131_0_g500 + ( ( _OuterOutlineWidth * float2( 0,-1 ) ) * temp_output_25_0_g500 ) ) ).a , tex2D( _MainTex, ( temp_output_131_0_g500 + ( ( _OuterOutlineWidth * float2( 0,1 ) ) * temp_output_25_0_g500 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g500 + ( ( _OuterOutlineWidth * float2( -1,0 ) ) * temp_output_25_0_g500 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g500 + ( ( _OuterOutlineWidth * float2( 1,0 ) ) * temp_output_25_0_g500 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g500 + ( ( _OuterOutlineWidth * float2( 0.705,0.705 ) ) * temp_output_25_0_g500 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g500 + ( ( _OuterOutlineWidth * float2( -0.705,0.705 ) ) * temp_output_25_0_g500 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g500 + ( ( _OuterOutlineWidth * float2( 0.705,-0.705 ) ) * temp_output_25_0_g500 ) ) ).a ) , tex2D( _MainTex, ( temp_output_131_0_g500 + ( ( _OuterOutlineWidth * float2( -0.705,-0.705 ) ) * temp_output_25_0_g500 ) ) ).a ) * 3.0 ) , 1.0 ) , _OuterOutlineFade);
				float4 appendResult174_g500 = (float4(lerpResult170_g500 , lerpResult168_g500));
				
				fixed4 c = ( appendResult174_g500 * IN.color );
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
	CustomEditor "SpriteShadersUltimate.SingleShaderGUI"
	
	
}
/*ASEBEGIN
Version=18900
0;0;1920;1019;2728.721;939.7081;2.982082;True;True
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;72;-1452.552,-164.1167;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;25;-999.9279,-429.1396;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;33;-945.118,-196.6154;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;106;-832.1169,-522.2167;Inherit;False;ShaderTime;0;;503;06a15e67904f217499045f361bad56e7;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;105;-519.5405,18.53127;Inherit;False;_OuterOutline;7;;500;c63fd6271f1306b46aa2076a150659b5;0;5;186;FLOAT;0;False;15;COLOR;0,0,0,0;False;161;SAMPLER2D;;False;7;FLOAT2;0,0;False;4;SAMPLER2D;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;22;-186.1487,18.7237;Inherit;False;TintVertex;-1;;499;b0b94dd27c0f3da49a89feecae766dcc;0;1;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;14;172.2378,20.04645;Float;False;True;-1;2;SpriteShadersUltimate.SingleShaderGUI;0;8;Sprite Shaders Ultimate/Standard/Color/Outer Outline;0f8ba0101102bb14ebf021ddadce9b49;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;True;3;1;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;25;0;72;0
WireConnection;105;186;106;0
WireConnection;105;15;25;0
WireConnection;105;7;33;0
WireConnection;105;4;72;0
WireConnection;22;1;105;0
WireConnection;14;0;22;0
ASEEND*/
//CHKSM=FF05A79609950CD0D7E8E1E33AB2F6661E1FD00D
Shader "Custom/DepthSegmentationCustom"
{
	Properties
	{
		_MainTexture ("Texture", 2D) = "white" {}
		_DepthTex ("Depth texture", 2D) = "white" {}
		_SegmentationTex("Segmentation texture", 2D) = "white" {}
		_ScaleMult("Scale multiplier", Float) = 0.001
		_Greyness("Grayness", Float) = 0.5
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.2

		//_ScaleMultPixel("Pixel scale multiplier", Float) = 1

//		number properties:
//		name ("display name", Range (min, max)) = number
//		name ("display name", Float) = number
//		name ("display name", Int) = number

//		colors, vectors:
//		name ("display name", Color) = (number,number,number,number)
//		name ("display name", Vector) = (number,number,number,number)
	}

	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 200

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

		Pass
		{
			Tags {"LightMode" = "ForwardBase"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

			uniform float3 _Offsets[50]; //offsets of vertexes for used point mesh

			uniform float fX;
			uniform float fY;

			struct appdata
			{
				float4 vertex : POSITION;

				float2 uv : TEXCOORD0;
				float2 depthPos : TEXCOORD1; //pos on depthmap
				float2 index : TEXCOORD2; //x is used for pixel vertex index
				float3 normal: NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1; 
				float4 vertex : SV_POSITION;
				fixed4 diff : COLOR0;
			};

			float _ScaleMult;
			float _Greyness;
			sampler2D _DepthTex;
			sampler2D _SegmentationTex;
			sampler2D _MainTexture;
			fixed _Cutoff;

			v2f vert (appdata v)
			{
				v2f o;

				float3 off = _Offsets[round(v.index.x)]; //offset for current vertex
				float4 depthCol = tex2Dlod(_DepthTex, float4(v.depthPos, 0, 0));

				//real positions of pixel:
				float z = depthCol.r * _ScaleMult * 16384; // should be enough, as depth is ushort in C#
				float x =  z * (v.depthPos.x - 0.5) / fX;
				float y = -z * (v.depthPos.y - 0.5) / fY;

				float4 newVertexPos = float4(x, y, z, 1) + float4(off, 0) * z;

				half3 worldNormal  = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl * _LightColor0;
				o.diff.rgb += ShadeSH9(half4(worldNormal,1));
				o.diff.a = 1;

				float colorModRed = max(0, 1 - newVertexPos.z / 4);
				float colorModGreen = max(0, 1 - abs(newVertexPos.z - 3) / 1.5);
				float colorModBlue = max(0, (newVertexPos.z - 3) / 7);

				o.diff.rgb *= float3(_Greyness + (1 - _Greyness) * colorModRed, _Greyness + (1 - _Greyness) * colorModGreen, _Greyness + (1 - _Greyness) * colorModBlue);

				o.vertex = mul(UNITY_MATRIX_MVP, newVertexPos);
				o.uv2 = v.depthPos;
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTexture, i.uv) * tex2D(_SegmentationTex, i.uv2) * i.diff;
				clip(col.a - _Cutoff);
				return col;
			}
			ENDCG
		}
	}
}

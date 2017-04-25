﻿// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'



Shader "GDGeek/VoxelMesh3" {
    Properties {
   		_LightColor ("Light Color", Color) = (1,1,1,1)
   		_MainColor ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Atlas Texture", 2D) = "white" {}
        _ShadowAlpha ("Shadow Alpha", float) = 0
      
       
    }

    // ======================================================== 
    // cg 
    // ======================================================== 

    SubShader {
        Tags { 
           // "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }
        Cull Off 
        Lighting Off 
        Fog { Mode Off }

        Pass {
       		Name "BASE"
			CGPROGRAM 
			
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members shadow)
			//#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _LightColor;
			fixed4 _MainColor;
			float _ShadowAlpha;
			float _LightPower;
			
			struct vertexInput {
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				fixed3 color     : COLOR;
				float3 normal :NORMAL;
			};

			struct vertexOutput {
				float4 vertex        : POSITION;
				float2 texcoord0      : TEXCOORD0;
				float2 texcoord1     : TEXCOORD1;
				fixed3 color     : COLOR; 
			};

			vertexOutput vert ( vertexInput input ) {
				vertexOutput output;
				output.vertex = mul(UNITY_MATRIX_MVP, input.vertex);
				output.texcoord0 = TRANSFORM_TEX(input.texcoord, _MainTex);
				output.texcoord1 = TRANSFORM_TEX(input.texcoord1, _MainTex);
				
				float3 viewDir = normalize(WorldSpaceViewDir(input.vertex));
				float3 normalDir =  normalize(mul(float4(input.normal, 0.0), unity_WorldToObject).xyz);
				output.color = float3(1,1,1) * min(1.0, max(0.3, dot(normalDir, viewDir)) *1.5);
				output.color *= input.color;
				return output;
			}

			fixed4 frag ( vertexOutput input ) : COLOR {
			
				fixed4 output= fixed4(input.color.r, input.color.g, input.color.b,1);
				fixed4 light =  _LightColor - tex2D ( _MainTex, input.texcoord0);
				fixed4 shadow =  tex2D ( _MainTex, input.texcoord1);
				
				output *= _MainColor;
				output += max(light, fixed4(0,0,0,0));
				output *= shadow * _ShadowAlpha  + (1-_ShadowAlpha);
                return output; 
			}
			ENDCG
        }
    }

    // ======================================================== 
    // fallback 
    // ======================================================== 

    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }
        Cull Off 
        Lighting Off 
        ZWrite Off 
        Fog { Color (0,0,0,0) }
        Blend SrcAlpha OneMinusSrcAlpha

        BindChannels {
            Bind "Color", color
            Bind "Vertex", vertex
            Bind "TexCoord", texcoord
        }

        Pass {
            SetTexture [_MainTex] {
                combine texture * primary
            }
        }
    }
}

Shader "Custom/OutlineShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
    }
    
    SubShader
    {
        Tags { "RenderType"= "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        
        sampler2D _MainTex;
        
        struct Input
        {
            float2 uv_MainTex;
        };
        
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG

        Pass
        {
			Cull Front //表面をカリング（描画しない）

			CGPROGRAM

			struct appdata
	        {
	        	float4 vertex : POSITION;
	        	float2 uv     : TEXCOORD0;
	        	float3 normal : NORMAL;
	        };
            
	        struct v2f 
	        {
	        	float4 pos : SV_POSITION;
	        	float2 uv  : TEXCOORD0;
	        };

			uniform float _OutlineWidth;
	        uniform float4 _OutlineColor;

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex); //頂点をMVP行列変換

	            //モデル座標系の法線をビュー座標系に変換
				float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));

	            //ビュー座標系に変換した法線を投影座標系に変換
				float2 offset = TransformViewToProjection(norm.xy);

	            //法線方向に頂点位置を押し出し
	            o.pos.xy += offset * UNITY_Z_0_FAR_FROM_CLIPSPACE(o.pos.z) * _OutlineWidth;

				return o;
			}

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor; //プロパティに設定したアウトラインカラーを表示
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

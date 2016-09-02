Shader "_Bison/Ground" {
	Properties {
		_Color ("Grass Color", Color) = (1,1,1,1)
		_MainTex ("Grass (RGB)", 2D) = "white" {}
        _MainTex1 ("Mask (RGB)", 2D) = "white" {}

        _TeamColor ("Team Color", Color) = (1,1,1,1)

        _Stripe ("Stripe", Range(0,1)) = 0.5
        _Line ("Line", Range(0,1)) = 0.8

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
  	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MainTex1;

		struct Input {
			float2 uv_MainTex;
            float2 uv2_MainTex1;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        fixed4 _TeamColor;
        fixed _Stripe;
        fixed _Line;

		void surf (Input IN, inout SurfaceOutputStandard o) {

            fixed4 grass = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            fixed4 mask = tex2D (_MainTex1, IN.uv2_MainTex1);

            fixed4 team = _TeamColor * mask.r;
            fixed4 stripe = mask.g*(0.3*_Stripe);
            fixed4 wline = mask.b*(_Line);

            fixed4 color;

            color = lerp( grass - stripe + team*_TeamColor.a, fixed4(1,1,1,1), wline);

            //fixed2 uv = IN.uv2_MainTex1.xy;
            //uv.x /= 24.0;
            //uv.y /= 24.0;
             
            //fixed tex1 = tex2D(_MainTex1, uv).r;
			
            //..............................
			o.Albedo = color.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

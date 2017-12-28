/*Copyright(c) <2017> <Benoit Constantin ( France )>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

Shader "BC_Solution/Materialization" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MaterializationTex("Materialization", 2D) = "black"{}
		_MaterializationColor("Materialization color", Color) = (1,1,1,1)
		_MaterializationColorFactor("Materialization color factor", Range(0,100)) = 3
	    _MaterializationDistanceFactor("Materialization color factor", Range(0,1)) = 0.25
		_MaterializationAmount("Materialization amount", Range(0,2)) = 2.0
	}
	SubShader {
		Tags { "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 200
		cull off
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alphatest:_Cutoff fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MaterializationTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		float _MaterializationAmount;
		float _MaterializationColorFactor;
		float _MaterializationDistanceFactor;
		float4 _MaterializationTex_ST;

		fixed4 _Color;
		fixed4 _MaterializationColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		//Calcul the amount of progress of the materialization
	    //t is the actual value of materialization
		//Pixel value is the value in the materialization texture
		inline float materialization(float t, float pixelValue)
		{
			return saturate(t - pixelValue);
		}

		//Distance calcul
		inline float distance(float t, float pixelValue)
		{
			return abs(t - pixelValue);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			float2 materialPos = IN.uv_MainTex*_MaterializationTex_ST.xy + _MaterializationTex_ST.zw;
			float mat = materialization(_MaterializationAmount, tex2D(_MaterializationTex, materialPos).rgb);
			float dist = distance(_MaterializationAmount, tex2D(_MaterializationTex, materialPos).rgb);


			o.Albedo = c.rgb;
			o.Emission = _MaterializationColorFactor*saturate(_MaterializationDistanceFactor - dist)*_MaterializationColor;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = mat*c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

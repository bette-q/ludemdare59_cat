Shader "Custom/FishEyeVignette"
{
    Properties
    {
        _MainTex ("Source", 2D) = "white" {}

        _FisheyeStrength ("Strength", Range(0, 1.5)) = 0.45
        _FisheyePower ("Power", Range(1, 4)) = 2.2
        _ViewMagnify ("Zoom", Range(1, 3)) = 1.5

        _VignetteIntensity ("Intensity", Range(0, 1)) = 0.65
        _VignetteInner ("Inner", Range(0, 1.2)) = 0.42
        _VignetteOuter ("Outer", Range(0, 1.5)) = 1.05
        _EllipseX ("Stretch X", Range(0.6, 2)) = 1.25
        _EllipseY ("Stretch Y", Range(0.6, 2)) = 0.95

        _OvalBorderStrength ("Strength", Range(0, 1)) = 1
        _PeepholeRadius ("Radius", Range(0.55, 1.45)) = 1.02
        _PeepholeFeather ("Feather", Range(0.03, 0.45)) = 0.16

        _EdgeSoftBlur ("Blur", Range(0, 0.02)) = 0.004
        _UVRimFeather ("Rim", Range(0.01, 0.25)) = 0.07
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Always
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            half _FisheyeStrength;
            half _FisheyePower;
            half _ViewMagnify;

            half _VignetteIntensity;
            half _VignetteInner;
            half _VignetteOuter;
            half _EllipseX;
            half _EllipseY;

            half _OvalBorderStrength;
            half _PeepholeRadius;
            half _PeepholeFeather;

            half _EdgeSoftBlur;
            half _UVRimFeather;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 DistortUV(float2 uv)
            {
                float2 center = 0.5;
                float2 d = uv - center;

                float aspect = _ScreenParams.x / _ScreenParams.y;
                d.x *= aspect;

                float r = length(d);
                float rp = pow(saturate(r), _FisheyePower);
                float factor = 1.0 + (_FisheyeStrength * 2.0) * rp;

                d *= factor;
                d.x /= aspect;

                return d + center;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uvBase = 0.5 + (i.uv - 0.5) / _ViewMagnify;
                float2 uv = DistortUV(uvBase);

                float2 suv = saturate(uv);

                half b = _EdgeSoftBlur;
                fixed4 col =
                    tex2D(_MainTex, suv) * 0.52 +
                    tex2D(_MainTex, suv + float2(b, 0)) * 0.12 +
                    tex2D(_MainTex, suv - float2(b, 0)) * 0.12 +
                    tex2D(_MainTex, suv + float2(0, b)) * 0.12 +
                    tex2D(_MainTex, suv - float2(0, b)) * 0.12;

                float edge = min(min(uv.x, 1 - uv.x), min(uv.y, 1 - uv.y));
                half rim = smoothstep(-_UVRimFeather * 1.25, _UVRimFeather * 0.85, edge);
                col.rgb *= rim;

                float2 v = (i.uv - 0.5) * 2.0;
                v.x *= _EllipseX;
                v.y *= _EllipseY;
                float d = length(v);

                half vig = smoothstep(_VignetteInner, _VignetteOuter, d);
                col.rgb *= (1.0 - vig * _VignetteIntensity);

                float2 c = abs(i.uv - 0.5) * 2.0;
                float cn = (c.x + c.y) * 0.5;
                half corner = smoothstep(0.38, 0.95, cn) * _VignetteIntensity * 0.35;
                col.rgb *= (1.0 - corner);

                float norm = d / length(float2(_EllipseX, _EllipseY));
                float pr = norm / _PeepholeRadius;
                half mask = 1.0 - smoothstep(1.0 - _PeepholeFeather * 2.0, 1.0 + _PeepholeFeather * 0.55, pr);

                col.rgb *= lerp(1.0, mask, _OvalBorderStrength);

                return col;
            }
            ENDCG
        }
    }

    FallBack Off
}
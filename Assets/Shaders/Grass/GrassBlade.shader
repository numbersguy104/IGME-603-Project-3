Shader "BezierBlade"
{
    Properties
    {
        [Header(Shape)]
        _TaperAmount ("Taper Amount", Float) = 0
        _CurvedNormalAmount("Curved Normal Amount", Range(0, 5)) = 1
        _p1Offset ("p1Offset", Float) = 1
        _p2Offset ("p2Offset", Float) = 1

        [Header(Shading)]
        _TopColor ("Top Color", Color) = (.25, .5, .5, 1)
        _BottomColor ("Bottom Color", Color) = (.25, .5, .5, 1)
        _GrassAlbedo("Grass albedo", 2D) = "white" {}
        _GrassGloss("Grass gloss", 2D) = "white" {}
        
        [Header(Wind Animation)]
        _WaveAmplitude("Wave Amplitude", Float) = 1
        _WaveSpeed("Wave Speed", Float) = 1
        _SinOffsetRange("Phase Variation", Range(0, 10)) = 0.3
        _PushTipForward("Push Tip Forward", Range(0, 2)) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Simple Grass Blade"
            Tags { "LightMode" = "UniversalForward" }

            Cull Off

            HLSLPROGRAM
            // Required to compile gles3.0 on some platforms
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _SHADOWS_SOFT

            // Vertex Shader
            #pragma vertex vert
            // Fragment Shader
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "CubicBezier.hlsl"

            struct GrassBlade
            {
                float3 position;
                float rotAngle;
                float hash;
                float height;
                float width;
                float tilt;
                float bend;
                float3 surfaceNorm;
                float windForce;
                float sideBend;
            };

            StructuredBuffer<GrassBlade> _GrassBlades;
            StructuredBuffer<int> Triangles;
            StructuredBuffer<float4> Colors;
            StructuredBuffer<float2> Uvs;
            
            float _TaperAmount;
            float _CurvedNormalAmount;
            float _p1Offset;
            float _p2Offset;
            float4 _TopColor;
            float4 _BottomColor;

            float _WaveAmplitude;
            float _WaveSpeed;
            float _SinOffsetRange;
            float _PushTipForward;

            TEXTURE2D(_GrassAlbedo);
            SAMPLER(sampler_GrassAlbedo);
            TEXTURE2D(_GrassGloss);
            SAMPLER(sampler_GrassGloss);

            struct Attributes
            {
                uint vertexID : SV_VertexID;
                uint instanceID : SV_InstanceID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 curvedNorm : TEXCOORD1;
                float3 originalNorm : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float t : TEXCOORD4;
            };

            float3 GetP0()
            {
                return float3(0,0,0);
            }

            float3 GetP3(float height, float tilt)
            {
                float p3y = tilt * height;
                float p3x = sqrt(height * height - p3y * p3y);
                return float3(-p3x, p3y, 0);
            }

            void GetP1P2P3(float3 p0, inout float3 p3, float bend, float hash, float windForce, out float3 p1, out float3 p2)
            {
                p1 = lerp(p0, p3, 0.33);
                p2 = lerp(p0, p3, 0.66);

                float3 bladeDir = normalize(p3 - p0);
                float3 bezCtrlOffsetDir = normalize(cross(bladeDir, float3(0,0,1)));

                p1 += bezCtrlOffsetDir * bend * _p1Offset;
                p2 += bezCtrlOffsetDir * bend * _p2Offset;

                float p2WindEffect = sin((_Time.y + hash * 2 * PI) * _WaveSpeed + 0.66 * 2 * PI * _SinOffsetRange) * windForce;
                p2WindEffect *= 0.66 * _WaveAmplitude;

                float p3WindEffect = sin((_Time.y + hash * 2 * PI) * _WaveSpeed + 1.0 * 2 * PI * _SinOffsetRange) * windForce + _PushTipForward * (1 - bend);
                p3WindEffect *= _WaveAmplitude;

                p2 += bezCtrlOffsetDir * p2WindEffect;
                p3 += bezCtrlOffsetDir * p3WindEffect;
            }

            float3x3 RotAxis3x3(float angle, float3 axis)
            {
                axis = normalize(axis);
                
                float s, c;
                sincos(angle, s, c);
                
                // 1 - cos(angle)
                float t = 1.0 - c;
                
                // 轴的分量
                float x = axis.x;
                float y = axis.y;
                float z = axis.z;
                
                float xy = x * y;
                float xz = x * z;
                float yz = y * z;
                float xs = x * s;
                float ys = y * s;
                float zs = z * s;
                
                float m00 = t * x * x + c;
                float m01 = t * xy - zs;
                float m02 = t * xz + ys;
                
                float m10 = t * xy + zs;
                float m11 = t * y * y + c;
                float m12 = t * yz - xs;
                
                float m20 = t * xz - ys;
                float m21 = t * yz + xs;
                float m22 = t * z * z + c;
                
                return float3x3(
                    m00, m01, m02,
                    m10, m11, m12,
                    m20, m21, m22
                );
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                GrassBlade blade = _GrassBlades[IN.instanceID];
                float bend = blade.bend;
                float height = blade.height;
                float tilt = blade.tilt;
                float hash = blade.hash;
                float windForce = blade.windForce;

                float3 p0 = GetP0();

                float3 p3 = GetP3(height, tilt);

                float3 p1 = float3(0,0,0);
                float3 p2 = float3(0,0,0);
                GetP1P2P3(p0, p3, bend, hash, windForce, p1, p2);

                int positionIndex = Triangles[IN.vertexID];
                float4 vertColor = Colors[positionIndex];
                float2 uv = Uvs[positionIndex];


                
                float t = vertColor.r;
                float3 centerPos = CubicBezier(p0, p1, p2, p3, t);

                float width = blade.width * (1 - _TaperAmount * t);
                float side = vertColor.g * 2 - 1;
                float3 position = centerPos + float3(0, 0, side * width);

                float3 tangent = CubicBezierTangent(p0, p1, p2, p3, t);
                float3 normal = normalize(cross(tangent, float3(0,0,1)));

                float3 curvedNorm = normal;
                curvedNorm.z += side * _CurvedNormalAmount;
                curvedNorm = normalize(curvedNorm);

                float angle = blade.rotAngle;
                float sideBend = blade.sideBend;

                float3x3 rotMat = RotAxis3x3(-angle, float3(0,1,0));

                float3x3 sideRot = RotAxis3x3(sideBend, normalize(tangent));
                position = position - centerPos;
                normal = mul(sideRot, normal);
                curvedNorm = mul(sideRot, curvedNorm);
                position = mul(sideRot, position);

                position = position + centerPos;
                normal = mul(rotMat, normal);
                curvedNorm = mul(rotMat, curvedNorm);
                position = mul(rotMat, position);

                position += blade.position;

                OUT.positionCS = TransformWorldToHClip(position);
                OUT.curvedNorm = curvedNorm;
                OUT.originalNorm = normal;
                OUT.positionWS = position;
                OUT.uv = uv;
                OUT.t = t;

                return OUT;
            }

            half4 frag(Varyings i, bool isFrontFace : SV_IsFrontFace) : SV_Target
            {
                // Calculate normal
                float3 n = isFrontFace ? normalize(i.curvedNorm) : -reflect(-normalize(i.curvedNorm), normalize(i.originalNorm));

                Light mainLight = GetMainLight(TransformWorldToShadowCoord(i.positionWS));
                float3 v = normalize(GetCameraPositionWS() - i.positionWS);

                float3 grassAlbedo = saturate(_GrassAlbedo.Sample(sampler_GrassAlbedo, i.uv));

                float4 grassCol = lerp(_BottomColor, _TopColor, i.t);

                float3 albedo = grassCol.rgb * grassAlbedo;

                float gloss = (1 - _GrassGloss.Sample(sampler_GrassGloss, i.uv).r)* 0.35;

                half3 GI = SampleSH(n);

                BRDFData brdfData;
                half alpha = 1;

                InitializeBRDFData(albedo, 0, half3(1, 1, 1), gloss, alpha, brdfData);
                float3 directBRDF = DirectBRDF(brdfData, n, mainLight.direction, v) * mainLight.color;

                // Final color calculation
                float3 finalColor = GI * albedo + directBRDF * (mainLight.shadowAttenuation);

                float4 col;
                col = float4(finalColor, grassCol.a); // Alpha from grassCol

                return half4(col);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}
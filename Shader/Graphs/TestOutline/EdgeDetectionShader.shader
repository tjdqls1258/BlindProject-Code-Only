Shader"Hidden/Edge Detection"
{
    Properties
    {
        _OutlineThickness ("Outline Thickness", Float) = 1
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _maskTex ("MaskTex", 2D) = "" {}
        _maskData ("MaskData", 2D) = "" {}
        _numMasks ("numMasks", Float)= 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Opaque"
        }

ZWrite Off

Cull Off

Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
Name"EDGE DETECTION OUTLINE"
            
            HLSLPROGRAM
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl" // needed to sample scene depth
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl" // needed to sample scene normals
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl" // needed to sample scene color/luminance

float _OutlineThickness;
float4 _OutlineColor;
Texture2D _maskData;
float _numMasks;
Texture2D _maskTex;

SamplerState sampler_maskData;
SamplerState sampler_maskTex;
            #pragma vertex Vert // vertex shader is provided by the Blit.hlsl include
            #pragma fragment frag

            

            // Edge detection kernel that works by taking the sum of the squares of the differences between diagonally adjacent pixels (Roberts Cross).
float RobertsCross(float3 samples[4])
{
    const float3 difference_1 = samples[1] - samples[2];
    const float3 difference_2 = samples[0] - samples[3];
    return sqrt(dot(difference_1, difference_1) + dot(difference_2, difference_2));
}

                        // The same kernel logic as above, but for a single-value instead of a vector3.
float RobertsCross(float samples[4])
{
    const float difference_1 = samples[1] - samples[2];
    const float difference_2 = samples[0] - samples[3];
    return sqrt(difference_1 * difference_1 + difference_2 * difference_2);
}
            
                        // Helper function to sample scene normals remapped from [-1, 1] range to [0, 1].
float3 SampleSceneNormalsRemapped(float2 uv)
{
    return SampleSceneNormals(uv) * 0.5 + 0.5;
}

                        // Helper function to sample scene luminance.
float SampleSceneLuminance(float2 uv)
{
    float3 color = SampleSceneColor(uv);
    return color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
}

float2 getUV(int i, float num)
{
    return float2((i + 0.5) / num, 0.5); // °¡¿îµ¥ ÇÈ¼¿¿¡¼­ »ùÇÃ¸µ
                //return float2(clamp((i / num) + 0.001, 0.001, 0.999), 0.0);
}

float alpa(float maskRadius, float maskPower)
{
    float maskrange = maskPower * 0.7;

    return (maskRadius < maskrange) ? 1 : saturate(lerp(1, 0, (maskRadius - maskrange) / (maskPower * 0.3)));
}

float4 SphericalMaskDissolveV2(
                Texture2D maskTex, SamplerState samplerMaskTex,
                Texture2D scanData, SamplerState samplerScanData,
                float numMask, float3 objectWorldPos, float lineSize, out float4 maskArea)
{
    float4 texColor = float4(0.0, 0.0, 0.0, 1);
    float4 texColor2 = float4(0.0, 0.0, 0.0, 1);
    for (int i = 0; i < numMask; i++)
    {
        float2 maskUV = getUV(i, numMask);
        float4 maskData = maskTex.Sample(samplerMaskTex, maskUV);
        float4 maskData2 = scanData.Sample(samplerScanData, maskUV);

        float3 maskPosition = maskData.rgb;
        float3 maskColor = maskData2.rgb;
        float maskRadius = maskData.a;
        float maskPower = maskData2.a;
        
        float dist = distance(maskPosition, objectWorldPos);
        float sphere = step(dist, maskRadius);
        float scanLineSphere = 1 - step(dist, maskRadius - lineSize);
        float a = alpa(maskRadius, maskPower);

        texColor.rgb += float3(1,1,1) * sphere * a;
        texColor2.rgb += (maskColor) * sphere * scanLineSphere * a;
    }
    
                // texColor.a = max(texColor.a, 0.0);
                // texColor2.a = max(texColor2.a, 0.0);
    maskArea = texColor;
    texColor = texColor2;
    
    return texColor;
}

half4 frag(Varyings IN) : SV_TARGET
{
                            // Screen-space coordinates which we will use to sample.
    float2 uv = IN.texcoord;
    float2 texel_size = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                
                            // Generate 4 diagonally placed samples.
    const float half_width_f = floor(_OutlineThickness * 0.5);
    const float half_width_c = ceil(_OutlineThickness * 0.5);

    float2 uvs[4];
    uvs[0] = uv + texel_size * float2(half_width_f, half_width_c) * float2(-1, 1); // top left
    uvs[1] = uv + texel_size * float2(half_width_c, half_width_c) * float2(1, 1); // top right
    uvs[2] = uv + texel_size * float2(half_width_f, half_width_f) * float2(-1, -1); // bottom left
    uvs[3] = uv + texel_size * float2(half_width_c, half_width_f) * float2(1, -1); // bottom right
                
    float3 normal_samples[4];
    float depth_samples[4], luminance_samples[4];
                
    for (int i = 0; i < 4; i++)
    {
        depth_samples[i] = SampleSceneDepth(uvs[i]);
        normal_samples[i] = SampleSceneNormalsRemapped(uvs[i]);
        luminance_samples[i] = SampleSceneLuminance(uvs[i]);
    }
                
                            // Apply edge detection kernel on the samples to compute edges.
    float edge_depth = RobertsCross(depth_samples);
    float edge_normal = RobertsCross(normal_samples);
    float edge_luminance = RobertsCross(luminance_samples);
                            // Threshold the edges (discontinuity must be above certain threshold to be counted as an edge). The sensitivities are hardcoded here.
    float depth_threshold = 1 / 200.0f;
    edge_depth = edge_depth > depth_threshold ? 1 : 0;
                
    float normal_threshold = 1 / 4.0f;
    edge_normal = edge_normal > normal_threshold ? 1 : 0;
                
    float luminance_threshold = 1 / 0.5f;
    edge_luminance = edge_luminance > luminance_threshold ? 1 : 0;

                            // Combine the edges from depth/normals/luminance using the max operator.
    float edge = max(max(edge_depth, edge_normal), edge_luminance);

    float depth = SampleSceneDepth(uv);
    float3 worldPos = ComputeWorldSpacePosition(uv, depth, UNITY_MATRIX_I_VP);
    float4 mask = float4(0, 0, 0, 1);
    float4 maskColor = SphericalMaskDissolveV2(_maskTex, sampler_maskTex, _maskData, sampler_maskData, _numMasks, worldPos, 1, mask);
    float3 outline = _OutlineColor.rgb * maskColor.rgb;
    float4 colorLine = float4(outline.rgb, 1);
                // Color the edge with a custom color.
    return (edge * mask) + colorLine;
}
            ENDHLSL
        }
    }
}
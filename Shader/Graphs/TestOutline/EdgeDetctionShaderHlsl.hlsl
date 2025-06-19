#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl" // needed to sample scene depth
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl" // needed to sample scene normals
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl" // needed to sample scene color/luminance

float RobertsCross_f3(float3 samples[4])
{
    const float3 difference_1 = samples[1] - samples[2];
    const float3 difference_2 = samples[0] - samples[3];
    return sqrt(dot(difference_1, difference_1) + dot(difference_2, difference_2));
}

            // The same kernel logic as above, but for a single-value instead of a vector3.
float RobertsCross_f4(float samples[4])
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

void mainOutline_float(float2 uv, float thickness, float4 c, out float4 edge)
{
    // Screen-space coordinates which we will use to sample.
    float2 texel_size = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                
    // Generate 4 diagonally placed samples.
    const float half_width_f = floor(thickness * 0.5);
    const float half_width_c = ceil(thickness * 0.5);

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
    float edge_depth = RobertsCross_f4(depth_samples);
    float edge_normal = RobertsCross_f3(normal_samples);
    float edge_luminance = RobertsCross_f4(luminance_samples);
                
                // Threshold the edges (discontinuity must be above certain threshold to be counted as an edge). The sensitivities are hardcoded here.
    float depth_threshold = 1 / 200.0f;
    edge_depth = edge_depth > depth_threshold ? 1 : 0;
                
    float normal_threshold = 1 / 4.0f;
    edge_normal = edge_normal > normal_threshold ? 1 : 0;
                
    float luminance_threshold = 1 / 0.5f;
    edge_luminance = edge_luminance > luminance_threshold ? 1 : 0;
                
                // Combine the edges from depth/normals/luminance using the max operator.
    edge = max(edge_depth, max(edge_normal, edge_luminance)) * c;
}

void mainOutline_half(float2 uv, float thickness, float4 c, out float4 edge)
{
    // Screen-space coordinates which we will use to sample.
    float2 texel_size = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                
    // Generate 4 diagonally placed samples.
    const float half_width_f = floor(thickness * 0.5);
    const float half_width_c = ceil(thickness * 0.5);

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
    float edge_depth = RobertsCross_f4(depth_samples);
    float edge_normal = RobertsCross_f3(normal_samples);
    float edge_luminance = RobertsCross_f4(luminance_samples);
                
                // Threshold the edges (discontinuity must be above certain threshold to be counted as an edge). The sensitivities are hardcoded here.
    float depth_threshold = 1 / 200.0f;
    edge_depth = edge_depth > depth_threshold ? 1 : 0;
                
    float normal_threshold = 1 / 4.0f;
    edge_normal = edge_normal > normal_threshold ? 1 : 0;
                
    float luminance_threshold = 1 / 0.5f;
    edge_luminance = edge_luminance > luminance_threshold ? 1 : 0;
                
                // Combine the edges from depth/normals/luminance using the max operator.
    edge = max(edge_depth, max(edge_normal, edge_luminance)) * c;
}

#endif //MYHLSLINCLUDE_INCLUDED
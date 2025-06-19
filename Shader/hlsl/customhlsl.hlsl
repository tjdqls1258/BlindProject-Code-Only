#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

float2 getUV(int i, float num)
{
    //return float2(clamp((i / num)+0.001, 0.001, 0.999), 0.0);
    return float2((i + 0.5) / num, 0.5); // °¡¿îµ¥ ÇÈ¼¿¿¡¼­ »ùÇÃ¸µ
}

float alpa(float maskRadius, float maskPower)
{
    float maskrange = maskPower * 0.7;

    return (maskRadius < maskrange) ? 1 : saturate(lerp(1, 0, (maskRadius - maskrange) / (maskPower * 0.3)));
}

void SphericalMaskDissolveV2_float(UnityTexture2D maskTex, UnityTexture2D scanData, float numMasks, float3 objectWorldPos, out float4 texColor, out float4 texColor2)
{
    texColor = float4(0.0, 0.0, 0.0, 1);
    texColor2 = float4(0.0, 0.0, 0.0, 1);

    for (int i = 0; i < numMasks; i++)
    {
        float2 maskUV = getUV(i, numMasks);
        float4 maskData = tex2D(maskTex, maskUV);
        float4 maskData2 = tex2D(scanData, maskUV);

        float3 maskPosition = maskData.rgb; 
        float3 maskColor = maskData2.rgb;
        float maskRadius = maskData.a;
        float maskPower = maskData2.a;

        float dist = distance(maskPosition, objectWorldPos);
        float sphere = step(dist, maskRadius);
        float sphere2 = sphere * step(sphere, 0.001);
        float a = alpa(maskRadius, maskPower);

        texColor.rgb += (maskColor) * sphere * a;
        texColor2.rgb += (maskColor) * sphere2 * a;
    }

    texColor.a = max(texColor.a, 0.0);
    texColor2.a = max(texColor2.a, 0.0);
}

void SphericalMaskDissolveV2_half(UnityTexture2D maskTex, UnityTexture2D scanData, float numMasks, float3 objectWorldPos, out float4 texColor, out float4 texColor2)
{
    texColor = float4(0.0, 0.0, 0.0, 1);
    texColor2 = float4(0.0, 0.0, 0.0, 1);

    for (int i = 0; i < numMasks; i++)
    {
        float2 maskUV = getUV(i, numMasks); 
        float4 maskData = tex2D(maskTex, maskUV); 
        float4 maskData2 = tex2D(scanData, maskUV); 

        float3 maskPosition = maskData.rgb;
        float3 maskColor = maskData2.rgb;
        float maskRadius = maskData.a;
        float maskPower = maskData2.a;

        float dist = distance(maskPosition, objectWorldPos);
        float sphere = step(dist, maskRadius);
        
        float sphere2 = sphere * step(sphere, 0.001);
        float a = alpa(maskRadius, maskPower);

        texColor.rgb += (maskColor) * sphere * a;
        texColor2.rgb += (maskColor) * sphere2 * a;
    }

    texColor.a = max(texColor.a, 0.0);
    texColor2.a = max(texColor2.a, 0.0);
}

void ScanLine_float(UnityTexture2D maskTex, UnityTexture2D scanData, float numMasks, float3 objectWorldPos, float lineSize, out float4 texColor)
{
    texColor = float4(0.0, 0.0, 0.0, 1);

    for (int i = 0; i < numMasks; i++)
    {
        float2 maskUV = getUV(i, numMasks);
        float4 maskData = tex2D(maskTex, maskUV);
        float4 maskData2 = tex2D(scanData, maskUV);
        
        float3 maskPosition = maskData.rgb;
        float3 maskColor = maskData2.rgb;
        float maskRadius = maskData.a;
        float maskPower = maskData2.a;
        
        float dist = distance(maskPosition, objectWorldPos);
        float sphere = 1 - step(dist, maskRadius - lineSize);
        float sphere2 = step(maskRadius - 1.5, dist) * step(dist, maskRadius) * sphere;
        float a = alpa(maskRadius, maskPower);
        
        texColor.rgb += maskColor * sphere2 * a;
    }

    texColor.a = max(texColor.a, 0.0);
}

void ScanLine_half(UnityTexture2D maskTex, UnityTexture2D scanData, float numMasks, float3 objectWorldPos, float lineSize, out float4 texColor)
{
    texColor = float4(0.0, 0.0, 0.0, 1);

    for (int i = 0; i < numMasks; i++)
    {
        float2 maskUV = getUV(i, numMasks);
        float4 maskData = tex2D(maskTex, maskUV);
        float4 maskData2 = tex2D(scanData, maskUV);
        
        float3 maskPosition = maskData.rgb;
        float3 maskColor = maskData2.rgb;
        float maskRadius = maskData.a;
        float maskPower = maskData2.a;
        
        float dist = distance(maskPosition, objectWorldPos);
        float sphere = 1 - step(dist, maskRadius - lineSize);
        float sphere2 = step(maskRadius - 1.5, dist) * step(dist, maskRadius) * sphere;
        float a = alpa(maskRadius, maskPower);
        
        texColor.rgb += maskColor * sphere2 * a;
    }

    texColor.a = max(texColor.a, 0.0);
}

void test_float(UnityTexture2D maskTex, UnityTexture2D scanData, float numMasks, float3 objectWorldPos, float lineSize, out float4 texColor, out float al)
{
    texColor = float4(0.0, 0.0, 0.0, 1);
    
    texColor.rgb = (1, 1, 1);
    
    for (int i = 0; i < numMasks; i++)
    {
        float2 maskUV = getUV(i, numMasks);
        float4 maskData = tex2D(maskTex, maskUV);
        float4 maskData2 = tex2D(scanData, maskUV);
        
        float3 maskPosition = maskData.rgb;
        float3 maskColor = maskData2.rgb;
        float maskRadius = maskData.a;
        float maskPower = maskData2.a;
        
        float dist = distance(maskPosition, objectWorldPos);
        float sphere = 1 - step(dist, maskRadius - lineSize);
        float sphere2 = step(maskRadius - 2, dist) * step(dist, maskRadius) * sphere;

        texColor.a += alpa(maskRadius, maskPower) * sphere2;
        
        al += alpa(maskRadius, maskPower);
    }
    
    texColor.a = max(texColor.a, 0.0);
}

void test_half(UnityTexture2D maskTex, UnityTexture2D scanData, float numMasks, float3 objectWorldPos, float lineSize, out float4 texColor, out float al)
{
    texColor = float4(0.0, 0.0, 0.0, 1);
    
    texColor.rgb = (1, 1, 1);
    
    for (int i = 0; i < numMasks; i++)
    {
        float2 maskUV = getUV(i, numMasks);
        float4 maskData = tex2D(maskTex, maskUV);
        float4 maskData2 = tex2D(scanData, maskUV);
        
        float3 maskPosition = maskData.rgb;
        float3 maskColor = maskData2.rgb;
        float maskRadius = maskData.a;
        float maskPower = maskData2.a;
        
        float dist = distance(maskPosition, objectWorldPos);
        float sphere = 1 - step(dist, maskRadius - lineSize);
        float sphere2 = step(maskRadius - 1.5, dist) * step(dist, maskRadius) * sphere;

        texColor.a += alpa(maskRadius, maskPower) * sphere2;
        
        al += alpa(maskRadius, maskPower);
    }
    
    texColor.a = max(texColor.a, 0.0);
}
#endif //MYHLSLINCLUDE_INCLUDED
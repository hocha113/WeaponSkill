﻿sampler uImage0 : register(s0);
sampler uImage1 : register(s1); // Automatically Images/Misc/Perlin via Force Shader testing option
sampler uImage2 : register(s2); // Automatically Images/Misc/noise via Force Shader testing option
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pos = float2(0.5, 0.5);
    // offset 是中心到当前点的向量
    float2 offset = floor((coords - pos) * 50) / 50;
    float4 color = tex2D(uImage0, pos + offset);
    return color;
}

technique Technique1
{
    pass WoShiHuangLeiLong
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
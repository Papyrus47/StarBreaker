﻿sampler uImage0 : register(s0);

float4x4 uTransform;
float uTime;


struct VSInput
{
    float2 Pos : POSITION0;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

float4 PixelShaderFunction(PSInput input) : COLOR0
{
    float3 coord = input.Texcoord; //获取纹理坐标
    float4 color = tex2D(uImage0, float2(coord.x,coord.y));//获取对应在贴图上的颜色
    if (all(color))
    {
        return float4(1,1,1,1);
    }
    color.rbg = input.Color.rbg;
    return color;//返回其颜色

}
PSInput VertexShaderFunction(VSInput input)
{
    PSInput output;
    output.Color = input.Color;
    output.Texcoord = input.Texcoord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}
technique Technique1
{
    pass FrostFistHealMagic
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
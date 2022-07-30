sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

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

float3 hsv2rgb(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs((c.xxx + K.xyz - floor(c.xxx + K.xyz)) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float4 PixelShaderFunction(PSInput input) : COLOR0
{
    float3 coord = input.Texcoord;//获取纹理的坐标
    float4 c = tex2D(uImage0, float2(coord.x + uTime, coord.y));//主纹理
    c *= coord.z;//乘上透明度
    c = tex2D(uImage1, float2(c.r, 0.5));//获取颜色
    return c;

}

PSInput VertexShaderFunction(VSInput input)
{
    PSInput output;
    output.Color = input.Color;
    output.Texcoord = input.Texcoord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}


technique technique1
{
    pass TheDraw
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
sampler uImage0 : register(s0);
sampler uImage1 : register(s1); // 色图

float4x4 uTransform;
float m;
float n;
float4 uColor;

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

float4 ProjShader(PSInput input) : COLOR0
{
    float3 coords = input.Texcoord;
    float4 color = tex2D(uImage0, coords.xy);
    if (color.r < m && color.r > n)
    {
        return uColor;
    }
    else if (color.r <= n)
    {
        return float4(0,0,0,0);

    }
    float4 color1 = tex2D(uImage1, float2(color.r, 0.5));
    color1.a *= coords.z;
    return color1;
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
    pass ProjEffect
    {
        PixelShader = compile ps_2_0 ProjShader();
        VertexShader = compile vs_2_0 VertexShaderFunction();

    }
}
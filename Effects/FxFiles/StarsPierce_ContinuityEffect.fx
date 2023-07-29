sampler uImage0 : register(s0);

float2 uOrigin;

float4 PixelShaderFun(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float len = length(coords - uOrigin);
    color.a *= len;
    return color;
}


technique Technique1
{
    pass StarsPierce_ContinuityEffect
    {
        PixelShader = compile ps_2_0 PixelShaderFun();
    }
}

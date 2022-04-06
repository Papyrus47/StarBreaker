﻿sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float m;
float n;
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0, coords);
    float a = max(c.r, max(c.g, c.b));
    if (a > m)
    {
        float4 c1 = tex2D(uImage1, coords);
        return c1;
    }
    else if (abs(a - m) < n)
        return float4(0, 0, 0, 1);
    else
        return c * a;
}

technique Technique1
{
    pass Tentacle
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
   
}

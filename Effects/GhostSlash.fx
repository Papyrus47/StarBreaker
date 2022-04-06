sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float uOpacity;
float3 uSecondaryColor;
float uTime;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uImageOffset;
float uIntensity;
float uProgress;
float2 uDirection;
float2 uZoom;
float2 uImageSize0;
float2 uImageSize1;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if(uOpacity < 0.5)//在刚刚使用的时候
    {
        color.rbga -= float4(0.9, 0.9, 0.9, uOpacity);
    }
    else
    {
        color = float4(color.a - color.r, color.a - color.b, color.a - color.g, color.a);//让它反色
    }
    return color;
}
technique Technique1
{
    pass GhostSlash
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}


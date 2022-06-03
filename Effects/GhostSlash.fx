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
    float4 invColor = color;
    if (any(tex2D(uImage0, coords)))
    {
        invColor.rgb = 1 - invColor.rgb; //反色
        return color * (1 - uOpacity) + invColor * uOpacity;
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


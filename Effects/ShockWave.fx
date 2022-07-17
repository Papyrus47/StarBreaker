//此代码来源于 https://forums.terraria.org/index.php?threads/tutorial-shockwave-effect-for-tmodloader.81685/
//不是我写的

sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float uOpacity;
float3 uSecondaryColor;
float uTime;
float2 uScreenResolution;//分辨率
float2 uScreenPosition;
float2 uTargetPosition;
float2 uImageOffset;
float uIntensity;
float uProgress;
float2 uDirection;
float2 uZoom;
float2 uImageSize0;
float2 uImageSize1;

float4 PixelShaderFunction(float4 position : SV_POSITION, float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;//获取目标像素点的纹理坐标位置
    float2 centerCoords = (coords - targetCoords) * (uScreenResolution / uScreenResolution.y);//生成纵向映射
    float dotField = dot(centerCoords, centerCoords);//求点积
    float ripple = (dotField * uColor.y * 3.14) - (uProgress * uColor.z);//获取波纹(?)
    //啊啊啊啊啊我看不懂了
    //我摆烂不写注释了啊啊啊啊啊
    if (ripple < 0 && ripple > uColor.x * -2 * 3.14)
    {
        ripple = saturate(sin(ripple));
    }
    else
    {
        ripple = 0;
    }

    float2 sampleCoords = coords + ((ripple * uOpacity / uScreenResolution) * centerCoords);

    return tex2D(uImage0, sampleCoords);
}
technique Technique1
{
    pass ShockWave
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
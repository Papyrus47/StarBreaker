//本shader文件参考yiyang233的Bloom发光(指除开发光矩阵就全是抄的)
//yiyang,我的超人

sampler uImage0 : register(s0);
texture2D tex0;
sampler2D uImage1 = sampler_state
{
    Texture = <tex0>; //指定被采样的纹理 
    MinFilter = Linear; //纹理过滤方式 
    MagFilter = Linear;
    AddressU = Wrap; //纹理寻址模式 
    AddressV = Wrap;
};
float2 uScreenResolution;
float m;
float uIntensity;
float uRange;
float gauss[7] = { 0.025, 0.05, 0.25, 0.35, 0.25, 0.05, 0.025 }; //亮光矩阵

float4 CutLight(float2 coords : TEXCOORD0) : COLOR0 //提取亮度超过m的部分
{
    float4 c = tex2D(uImage0, coords);
    if (c.r * 0.4 + c.g * 0.4 + c.b * 0.2 > m)//这里亮度我取0.4r+0.4g+0.2b ，之所以不用“那个公式”是因为红色物体很多都是要发光的
        return c;
    else
        return float4(0, 0, 0, 0);//透明
}
float4 GlurH(float2 coords : TEXCOORD0) : COLOR0 //水平方向模糊
{
    float4 color = float4(0, 0, 0, 1);
    float dx = uRange / uScreenResolution.x;
    color = float4(0, 0, 0, 1);
    for (int i = -3; i <= 3; i++)
    {
        color.rgb += gauss[i + 3] * tex2D(uImage0, float2(coords.x + dx * i, coords.y)).rgb * uIntensity;
    }
    return color;
}
float4 GlurV(float2 coords : TEXCOORD0) : COLOR0 //竖直方向模糊
{
    float4 color = float4(0, 0, 0, 1);
    float dy = uRange / uScreenResolution.y;
    for (int i = -3; i <= 3; i++)
    {
        color.rgb += gauss[i + 3] * tex2D(uImage0, float2(coords.x, coords.y + dy * i)).rgb * uIntensity;
    }
    return color;
}

technique Technique1
{
    pass Bloom
    {
        PixelShader = compile ps_2_0 CutLight();
    }
    pass GlurH
    {
        PixelShader = compile ps_2_0 GlurH();
    }
    pass GlurV
    {
        PixelShader = compile ps_2_0 GlurV();
    }
}
uniform extern float4x4 WorldViewProj : WORLDVIEWPROJECTION;
uniform extern float4x4 WorldView : WORLDVIEW;
uniform extern texture DiffuseTexture;
uniform extern texture LightMapTexture;

struct VS_OUTPUT
{
    float4 position  : POSITION;
    float4 diffuse   : COLOR0;
    float4 textureCoordinate : TEXCOORD0;
    float4 lightMapCoordinate : TEXCOORD1;
};

sampler textureSampler = sampler_state
{
    Texture = <DiffuseTexture>;
    minfilter = LINEAR;
    magfilter = LINEAR;
    mipfilter = NONE; 
};

sampler lightmapSampler = sampler_state
{
    Texture = <LightMapTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = NONE; 
};
 
VS_OUTPUT TransformDiffuse(
    float4 Position  : POSITION, 
    float4 Normal    : NORMAL,
    float4 Diffuse   : COLOR0,
    float4 TextureCoordinate : TEXCOORD0, 
    float4 LightMapCoordinate : TEXCOORD1 )
{
    VS_OUTPUT Out = (VS_OUTPUT)0;

    Out.position = mul(Position, WorldViewProj);
    Out.textureCoordinate = TextureCoordinate;
    Out.lightMapCoordinate = LightMapCoordinate;
    Out.diffuse = Diffuse;
    return Out;
}


float4 ApplyDiffuseTexture(VS_OUTPUT vsout) : COLOR
{
    return tex2D(textureSampler, vsout.textureCoordinate).rgba * vsout.diffuse;
}

float4 ApplyDiffuseLightMapTexture(VS_OUTPUT vsout) : COLOR
{
    return tex2D(textureSampler, vsout.textureCoordinate).rgba *
           tex2D(lightmapSampler, vsout.lightMapCoordinate).rgba;
}

technique TransformAndTextureDiffuse
{
    pass P0
    {
        vertexShader = compile vs_4_0 TransformDiffuse();
        pixelShader  = compile ps_4_0 ApplyDiffuseTexture();
    }
}

technique TransformAndTextureDiffuseAndLightMap
{
    pass P0
    {
        vertexShader = compile vs_4_0 TransformDiffuse();
        pixelShader  = compile ps_4_0 ApplyDiffuseLightMapTexture();
    }
}

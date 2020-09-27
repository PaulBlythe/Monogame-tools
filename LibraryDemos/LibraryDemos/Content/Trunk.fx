/*
 * Copyright (c) 2007-2009 Asger Feldthaus
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

#define MAXBONES 20

float4x4 World;
float4x4 View;
float4x4 Projection;

// Should be:  InverseReferenceFrame * AbsoluteBoneTransform
float4x4 Bones[MAXBONES];

texture Texture;

float4 AmbientLight = float4(0.05,0.05,0.05,0);

sampler TextureSampler = sampler_state
{
	Texture = (Texture);
	AddressU = Wrap;
	AddressV = Wrap;
	AddressW = Wrap;
	MinFilter = Anisotropic;
	MagFilter = Anisotropic;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL;
    float2 TextureCoordinate : TEXCOORD0;
    int2 BoneIndex : TEXCOORD1;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TextureCoordinate : TEXCOORD0;    
    float2 Depth : TEXCOORD2;
    float3 Normal : TEXCOORD3;    
};


struct BBPixelToFrame
{
    half4 Color : COLOR0;
    half4 Normal : COLOR1;
    half4 Depth : COLOR2;
    half4 Extra1 : COLOR3;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	
	float4 localPosition = mul(input.Position, Bones[input.BoneIndex.x]);
    float4 worldPosition = mul(localPosition, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
    output.TextureCoordinate = input.TextureCoordinate;
    
    float3 normal = mul(input.Normal, Bones[input.BoneIndex.x]);
	normal = normalize(mul(normal, World));
	
	output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;    
    output.Normal = normal;
	
    return output;
}

BBPixelToFrame  PixelShaderFunction(VertexShaderOutput input)
{
	BBPixelToFrame output = (BBPixelToFrame)0;
    output.Color = tex2D(TextureSampler, input.TextureCoordinate) ;
    output.Color.a  = 0;
    output.Normal.rgb = 0.5f * (normalize(input.Normal) + 1.0f);              
    output.Normal.a = 0;                                        
    output.Extra1.rgba =  0;  
    output.Depth = input.Depth.x / input.Depth.y;
	output.Extra1.a =  0;		    
    return output;    
}

float4  PixelShaderFunctionForward(VertexShaderOutput input) : Color0
{	
    return tex2D(TextureSampler, input.TextureCoordinate);  
}

float4  PixelShaderFunctionDEPTH(VertexShaderOutput input) : Color0
{	
       return input.Depth.x / input.Depth.y;	
}



technique TechniqueDeferred
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}

technique TechniqueForward
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunctionForward();
    }
}

technique Depth
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunctionDEPTH();
    }
}
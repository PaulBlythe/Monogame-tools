/*
 * Copyright (c) 2007-2009 Asger Feldthaus
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */


#define MAXBONES 20

// 1 means we should only accept opaque pixels.
// -1 means only accept transparent pixels.
float AlphaTestDirection = 1;
float AlphaTestThreshold = 0.95;

float4 clippingPlane; 
bool isClip;
float4x4 WorldView;
float4x4 View;
float4x4 Projection;
texture Texture;
float4x4 Bones[MAXBONES];

float LeafScale = 1.0f;

float3 BillboardRight = float3(1,0,0);	// The billboard's right direction in view space
float3 BillboardUp = float3(0,1,0);		// The billboard's up direction in view space

sampler TextureSampler = sampler_state
{
	Texture = (Texture);
	MinFilter = Anisotropic;
	MagFilter = Anisotropic;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TextureCoordinate : TEXCOORD0;
    float2 Offset : TEXCOORD1;
    float4 Color : COLOR0;
    int2 BoneIndex : TEXCOORD2;
    float3 Normal : NORMAL;    
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TextureCoordinate : TEXCOORD0;    
    float2 Depth : TEXCOORD1;
    float3 Normal : TEXCOORD2;    
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
    float4 viewPosition = mul(localPosition, WorldView);
    
    viewPosition.xyz += (input.Offset.x * BillboardRight + input.Offset.y * BillboardUp) * LeafScale;
    
    output.Position = mul(viewPosition, Projection);
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;    

	output.TextureCoordinate = input.TextureCoordinate;
	
	float3 normal = mul(input.Normal, Bones[input.BoneIndex.x]);
	normal = mul(normal, WorldView);
	output.Normal = normal;			
	
    return output;
}

struct VOUT
{
    float4 Position : POSITION0;
	float2 texturecoord :  TEXCOORD0;
	float4 clipping :  TEXCOORD1;    
};


VOUT VertexShaderFunctionBASIC(VertexShaderInput input)
{
    VOUT output;
	
	float4 localPosition = mul(input.Position, Bones[input.BoneIndex.x]);
    float4 viewPosition = mul(localPosition, WorldView);
    
    viewPosition.xyz += (input.Offset.x * BillboardRight + input.Offset.y * BillboardUp) * LeafScale;
    
    output.Position = mul(viewPosition, Projection);
    
	output.texturecoord = input.TextureCoordinate;	
	
	output.clipping = 0;	
	float4 clp = output.Position;	
	output.clipping.x = dot(clp,clippingPlane) ;
	
    return output;
}

float4 PixelShaderFunctionBASIC(VOUT input) : COLOR0
{
	if(isClip)
		clip(input.clipping.x);

	float4 color = tex2D(TextureSampler, input.texturecoord);
	clip((color.a - 0.98f));	    
    return color;
}

BBPixelToFrame  PixelShaderFunctionApply(VertexShaderOutput input) 
{
	BBPixelToFrame output = (BBPixelToFrame)0;
    output.Color = float4(tex2D(TextureSampler, input.TextureCoordinate).rgb, tex2Dbias(TextureSampler, float4(input.TextureCoordinate.xy, 1, -1)).a);
	
	// Apply the alpha test.
    clip((output.Color .a - AlphaTestThreshold));

    output.Normal.rgb = 0.5f * (normalize(input.Normal) + 1.0f);              
    output.Normal.a = 0;                                        
    output.Extra1.rgba =  0;  
    output.Depth = input.Depth.x / input.Depth.y;
	output.Extra1.a =  1;		    
    return output;    
}

BBPixelToFrame  PixelShaderFunction(VertexShaderOutput input) 
{
	BBPixelToFrame output = (BBPixelToFrame)0;
    output.Color = float4( tex2D(TextureSampler, input.TextureCoordinate).rgb, tex2Dbias(TextureSampler, float4(input.TextureCoordinate.xy, 1, -1)).a);
	clip((output.Color .a - AlphaTestThreshold) * -1);
    output.Color.a = 0;
    output.Normal.rgb = 0.5f * (normalize(input.Normal) + 1.0f);
    output.Normal.a = 0;
    output.Extra1.rgba =  0;      
    output.Color.a = 0;
    return output;    
}

float4  PixelShaderFunctionForward1(VertexShaderOutput input) : Color0
{	
    float4 Color = float4( tex2D(TextureSampler, input.TextureCoordinate).rgb, tex2Dbias(TextureSampler, float4(input.TextureCoordinate.xy, 1, -1)).a);
	clip((Color.a - AlphaTestThreshold));	
	return Color;        
}


float4  PixelShaderFunctionForward2(VertexShaderOutput input) : Color0
{	
    float4 Color = float4(tex2D(TextureSampler, input.TextureCoordinate).rgb, tex2Dbias(TextureSampler, float4(input.TextureCoordinate.xy, 1, -1)).a);
	clip((Color.a - AlphaTestThreshold) * -1);	
	return Color;        
}

float4  PixelShaderFunctionApplyDEPTH(VertexShaderOutput input) : COLOR0
{	 	
    return input.Depth.x / input.Depth.y;	
}


technique First
{
    pass Opaque
    {    
		
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunctionApply();        
        //AlphaBlendEnable = false;                
        //AlphaTestEnable = true;
        //AlphaFunc = Greater;
        //AlphaRef = 230;        

        //ZEnable = true;
        //ZWriteEnable = true;        
        //CullMode = None;
    }
}

technique Second
{
    pass BlendedEdges
    {
	
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();        
        //AlphaBlendEnable = true;
        //SrcBlend = SrcAlpha;		
        //DestBlend = InvSrcAlpha;        
        //AlphaTestEnable = true;
        //AlphaFunc = LessEqual;
        //AlphaRef = 230;

        //ZEnable = true;
        //ZWriteEnable = false;
		//
        //CullMode = None;
    }
}


technique FirstFF
{
    pass Opaque
    {    
		
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunctionForward1();        
        //AlphaBlendEnable = false;                
        //ZEnable = true;
        //ZWriteEnable = true;        
        //CullMode = None;
    }
}

technique SecondFF
{
    pass BlendEdges
    {    
		
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunctionForward2();        
        //AlphaBlendEnable = true;
        //SrcBlend = SrcAlpha;		
        //DestBlend = InvSrcAlpha;        
        //ZEnable = true;
        //ZWriteEnable = false;
        //CullMode = None; 
    }
}


technique LEAF
{
    pass BlendEdges
    {    		
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunctionApplyDEPTH();        
        //AlphaBlendEnable = false;
        //ZEnable = true;
        //ZWriteEnable = true;
        //CullMode = None; 
    }
}


technique BASIC
{
    pass BlendEdges
    {    		
        VertexShader = compile vs_4_0 VertexShaderFunctionBASIC();
        PixelShader = compile ps_4_0 PixelShaderFunctionBASIC();        
        //AlphaBlendEnable = false;
        //ZEnable = true;
        //ZWriteEnable = true;
        //CullMode = None; 
    }
}

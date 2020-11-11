#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

matrix WorldViewProjection;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float z : TEXTURE0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	output.z = output.Position.z / output.Position.w;

	return output;
}

struct PixelShaderOutput
{
	float4 Color		: COLOR0;
};

PixelShaderOutput MainPS(VertexShaderOutput input)
{
	PixelShaderOutput output;

	float w = clamp(pow(min(1.0, input.Color.a * 10.0) + 0.01, 3.0) * 1e8 * pow(input.z * 0.9, 3.0), 1e-2, 3e3);
	output.Color = float4(input.Color.rgb * input.Color.a, input.Color.a) * w;
	return output;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
textures/13tokay/metalbase_trans
{
	qer_editorimage textures/13tokay/metalbase.tga
	surfaceparm nonsolid
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/metalbase.tga
		blendfunc filter
		rgbGen identity
	}
}

textures/13tokay/pipebod_trans
{
	qer_editorimage textures/13tokay/pipebod.tga
	surfaceparm nonsolid
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/pipebod.tga
		blendfunc filter
		rgbGen identity
	}
}

textures/13tokay/green1_1_trans
{
	qer_editorimage textures/13tokay/green1_1.tga
	surfaceparm nonsolid
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/green1_1.tga
		blendfunc filter
		rgbGen identity
	}
}

textures/13tokay/steptop_trans
{
	qer_editorimage textures/13tokay/steptop.tga
	surfaceparm nonsolid
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/steptop.tga
		blendfunc filter
		rgbGen identity
	}
}

textures/13tokay/stair2_trans
{
	qer_editorimage textures/13tokay/stair2.tga
	surfaceparm nonsolid
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/stair2.tga
		blendfunc filter
		rgbGen identity
	}
}

textures/13tokay/slots_trans
{
	qer_editorimage textures/13tokay/slots.tga
	surfaceparm nonsolid
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/slots.tga
		blendfunc filter
		rgbGen identity
	}
}


textures/13tokay/metalstrip2_trans
{
	qer_editorimage textures/13tokay/metalstrip2.tga
	surfaceparm nonsolid
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/metalstrip2.tga
		blendfunc filter
		rgbGen identity
	}
}

textures/13tokay/metalstrip4_trans
{
	qer_editorimage textures/13tokay/metalstrip4.tga
	surfaceparm nonsolid
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/metalstrip4.tga
		blendfunc filter
		rgbGen identity
	}
}

textures/13tokay/metalstrip2_dark
{
	qer_editorimage textures/13tokay/metalstrip2.tga
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/metalstrip2.tga
		blendfunc filter
		rgbGen const ( 0.6 0.6 0.6 )
	}
}

textures/13tokay/wastelt1_ed_trans
{
	qer_editorimage textures/13tokay/wastelt1_ed.tga
	q3map_lightimage textures/13tokay/wastelt1_glow.tga
	q3map_surfacelight 100
	surfaceparm nonsolid
	surfaceparm nomarks
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/wastelt1_ed.tga
		blendfunc filter
		rgbGen identity
	}
	{
		map textures/13tokay/wastelt1_glow.tga
		blendfunc add
		rgbGen wave noise 0.3 0.3 0 16
	}
}

textures/13tokay/weapmark_glow
{
	qer_editorimage textures/13tokay/weapmark.tga
	q3map_lightimage textures/13tokay/weapmark_glow.tga
	q3map_surfacelight 100
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/weapmark.tga
		blendfunc filter
		rgbGen identity
	}
	{
		map textures/13tokay/weapmark_glow.tga
		blendfunc add
		rgbGen const ( 0.25 0.25 0.2 ) 
	}
}

textures/13tokay/weapmark_teleexit
{
	qer_editorimage textures/13tokay/weapmark.tga
	q3map_lightimage textures/13tokay/weapmark_glow.tga
	q3map_surfacelight 100
	surfaceparm nonsolid
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/weapmark.tga
		blendfunc filter
		rgbGen identity
	}
	{
		map textures/13tokay/weapmark_glow.tga
		blendfunc add
		rgbGen wave sin 0.3 0.3 0 1
	}
}

textures/13tokay/slots_fx
{
	qer_editorimage textures/13tokay/slots.tga
	q3map_lightimage textures/13tokay/slots_fx.tga
	q3map_surfacelight 1000
	{
		map $lightmap 
		rgbGen identity
		tcGen lightmap 
	}
	{
		map textures/13tokay/slots.tga
		blendfunc filter
		rgbGen identity
	}
	{
		map textures/13tokay/slots_fx.tga
		blendfunc add
		rgbGen wave sin 0.5 0.2 0 0.3
	}
}

textures/13tokay/ceil_white7k_trans
{
	qer_editorimage textures/base_light/ceil1_39.tga
	surfaceparm nonsolid
	surfaceparm nomarks
	q3map_surfacelight 7000
	{
		map $lightmap
		rgbGen identity
		tcGen lightmap
	}
	{
		map textures/base_light/ceil1_39.tga
		blendFunc filter
		rgbGen identity
	}
	{
		map textures/base_light/ceil1_39.blend.tga
		blendfunc add
		rgbGen identity
	}
}

textures/13tokay/ceil_red10k_trans
{
	qer_editorimage textures/base_light/ceil1_22a.tga
	surfaceparm nonsolid
	surfaceparm nomarks
	q3map_surfacelight 10000
	{
		map $lightmap
		rgbGen identity
		tcGen lightmap
	}
	{
		map textures/base_light/ceil1_22a.tga
		blendFunc filter
		rgbGen identity
	}
	{
		map textures/base_light/ceil1_22a.blend.tga
		blendfunc add
		rgbGen identity
	}
}

textures/13tokay/ceil_cyan10k_trans
{
	qer_editorimage textures/base_light/ceil1_34.tga
	surfaceparm nonsolid
	surfaceparm nomarks
	q3map_surfacelight 10000
	{
		map $lightmap
		rgbGen identity
		tcGen lightmap
	}
	{
		map textures/base_light/ceil1_34.tga
		blendFunc filter
		rgbGen identity
	}
	{
		map textures/base_light/ceil1_34.blend.tga
		blendfunc add
		rgbGen identity
	}
}

textures/13tokay/bounce_xq1metalbig
{
	qer_editorimage textures/sfx/bounce_xq1metalbig.tga
	q3map_lightimage textures/sfx/jumppadsmall.tga	
	q3map_surfacelight 400
	nopicmip
	{
		map textures/sfx/bounce_xq1metalbig.tga
		rgbGen identity
	}
	{
		map $lightmap
		tcGen lightmap
		rgbGen identity
		blendfunc filter
	}
	{
		map textures/sfx/bouncepad01b_layer1.tga
		blendfunc add
		rgbGen wave sin 0.5 0.5 0 1.5	
	}
	{
		clampmap textures/sfx/jumppadsmall.tga
		blendfunc add
		tcMod stretch sin 1.2 0.8 0 1.5
		rgbGen wave square 0.5 0.5 0.25 1.5
	}
}

textures/13tokay/pipebodslime_alpha
{
	qer_editorimage textures/13tokay/pipebodslime.tga
	surfaceparm nolightmap
	surfaceparm nonsolid
	surfaceparm nomarks
	surfaceparm trans
	polygonoffset
	{
		map textures/13tokay/pipebodslime.tga
		blendfunc blend
		rgbGen exactVertex
		alphaGen Vertex
	}
}

textures/13tokay/teleporter_fx
{
	surfaceparm nolightmap
	surfaceparm nonsolid
	surfaceparm nomarks
	surfaceparm trans
	nopicmip
	{
		map textures/sfx/beam_1.tga
		blendfunc add
		rgbGen identity
	}
	{
		map textures/liquids/pool3d_6c2.tga
		blendfunc filter
		rgbGen identity
		tcMod scroll 0 -0.3
	}
}

textures/13tokay/sign_red
{
	surfaceparm nolightmap
	surfaceparm nonsolid
	surfaceparm nomarks
	surfaceparm trans
	polygonoffset
	{
		map textures/13tokay/sign_red.tga
		blendfunc gl_zero gl_one_minus_src_color
		rgbGen identity
	}
}

textures/13tokay/water
{
	qer_editorimage textures/liquids/pool3d_3e.tga
	qer_trans 0.5
	surfaceparm nonsolid
	surfaceparm trans
	surfaceparm water
	q3map_globaltexture
	cull disable
	deformVertexes wave 64 sin 0 0.5 0 0.5
	sort 8
	{ 
		map textures/liquids/pool3d_4b2.tga
		blendFunc add
		rgbgen identity	
		tcmod scroll -0.025 0.01
	}
	{ 
		map textures/liquids/pool3d_5e.tga
		blendFunc gl_dst_color gl_one
		tcmod scale 0.8 0.8
		tcmod scroll 0.025 -0.025
	}
	{
		map $lightmap
		blendFunc filter
		rgbgen identity		
	}
}

textures/13tokay/eq2_banner_move_red
{
	qer_editorimage textures/13tokay/eq2_banner_red.tga
	surfaceparm alphashadow
	surfaceparm nolightmap
	surfaceparm nonsolid
	surfaceparm nomarks
	surfaceparm trans
	sort 9
	cull disable
	deformVertexes wave 200 sin 0 2 0 0.4 
	{
		map textures/13tokay/eq2_banner_red.tga
		blendfunc blend
		rgbGen Vertex
		alphaFunc GE128
	}
}

textures/13tokay/shadow_fix
{
	qer_editorimage textures/sfx/mirrorkc.tga
	surfaceparm nolightmap
	surfaceparm nodlight
	surfaceparm nonsolid
	surfaceparm nomarks
	surfaceparm trans
	polygonoffset
	nopicmip
	{ 
		map gfx/damage/burn_med_mrk.tga
		blendfunc gl_zero gl_one_minus_src_color
		rgbgen identity
	}
}

textures/13tokay/skybox
{
	qer_editorimage textures/13tokay/env/blood-valley_up.tga
	q3map_lightimage textures/13tokay/env/blood-valley_ft.tga
	surfaceparm nolightmap
	surfaceparm noimpact
	surfaceparm nomarks
	surfaceparm sky
	q3map_globaltexture
	q3map_surfacelight 800
	q3map_lightsubdivide 192
	q3map_sunExt 1 .9 .6 150 225 30 20 16
	q3map_sunExt 1 .9 .6 100 0 90 40 16
	skyParms textures/13tokay/env/blood-valley - -
}

textures/13tokay/pipe_clip
{
	qer_editorimage textures/common/slick.tga
	surfaceparm nodamage
	surfaceparm nodraw
	surfaceparm nolightmap
	surfaceparm nomarks
	surfaceparm nonsolid
	surfaceparm playerclip
	surfaceparm slick
	qer_trans 0.4
}

textures/13tokay/cushion_clip
{
	qer_editorimage textures/common/cushion.tga
	surfaceparm nodamage
	surfaceparm nodraw
	surfaceparm nolightmap
	surfaceparm nomarks
	surfaceparm nonsolid
	surfaceparm playerclip
	qer_trans 0.4
}

models/mapobjects/13tokay/shotgun
{
	{
		map models/weapons2/shotgun/shotgun.tga
		rgbGen Vertex
	}
}


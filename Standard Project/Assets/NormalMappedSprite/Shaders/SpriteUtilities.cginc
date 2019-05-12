#ifndef SPRITEUTILS_INCLUDED
#define SPRITEUTILS_INCLUDED

fixed2 _SpriteFlip;

#define SPRITE_NORMALS_FLIP(o) o.Normal.xy *= _SpriteFlip;

//inline void SpriteNormals(inout SurfaceOutput o)
//{
//	o.Normal.xy *= _SpriteFlip;
//}
//
//inline void SpriteNormals(inout SurfaceOutputStandard o)
//{
//	o.Normal.xy *= _SpriteFlip;
//}
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quake3
{
    class Q3BSPConstants
    {
        public const int numberOfDirs = 17;
        public const int sizeTexture = 72;
        public const int sizePlane = 16;
        public const int sizeNode = 36;
        public const int sizeLeaf = 48;
        public const int sizeLeafFace = 4;
        public const int sizeLeafBrush = 4;
        public const int sizeModel = 40;
        public const int sizeBrush = 12;
        public const int sizeBrushSide = 8;
        public const int sizeVertex = 44;
        public const int sizeMeshVertex = 4;
        public const int sizeEffect = 72;
        public const int sizeFace = 104;
        public const int sizeLightMap = 128 * 128 * 3;
        public const int sizeLightVolume = 8;

        public const int lumpEntity = 0;
        public const int lumpTextures = 1;
        public const int lumpPlanes = 2;
        public const int lumpNodes = 3;
        public const int lumpLeafs = 4;
        public const int lumpLeafFaces = 5;
        public const int lumpLeafBrushes = 6;
        public const int lumpModels = 7;
        public const int lumpBrushes = 8;
        public const int lumpBrushSides = 9;
        public const int lumpVertexes = 10;
        public const int lumpMeshVertexes = 11;
        public const int lumpEffects = 12;
        public const int lumpFaces = 13;
        public const int lumpLightMaps = 14;
        public const int lumpLightVolumes = 15;
        public const int lumpVisData = 16;

        public const int faceTypePolygon = 1;
        public const int faceTypePatch = 2;
        public const int faceTypeMesh = 3;
        public const int faceTypeBillboard = 4;

        public const float epsilon = 1.0f / 32.0f;//0.001f;
        public const float scale = 4.0f;
    }
}

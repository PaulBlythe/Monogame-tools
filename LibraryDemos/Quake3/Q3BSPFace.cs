using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quake3
{
    public class Q3BSPFace
    {
        #region Fields
        int textureIndex;       // Texture index
        int effectIndex;        // Effect index
        int faceType;           // Face type, Q3BSPFaceType:Polygon, Patch, Mesh and Billboard
        int startVertex;        // Starting vertex index
        int vertexCount;        // Number of vertices
        int startMeshVertex;    // Starting mesh vertex index
        int meshVertexCount;    // Number of mesh vertices
        int lightMapIndex;      // Light map index
        int lm_startX;          // Corner of this face's lightmap image in lightmap
        int lm_startY;          // Corner of this face's lightmap image in lightmap
        int lm_width;           // Size of this face's lightmap image in lightmap
        int lm_height;          // Size of this face's lightmap image in lightmap
        Vector3 lm_origin;      // World space origin of lightmap
        Vector3 lm_sVec;        // World space lightmap s unit vector.
        Vector3 lm_tVec;        // World space lightmap t unit vector.
        Vector3 faceNormal;         // Surface normal
        int patchWidth;         // Patch dimension
        int patchHeight;        // Patch dimension

        int patchIndex;         // Patch Index 
        #endregion

        public Q3BSPFace(
            int texture,
            int effect,
            int facetype,
            int vertex,
            int vertices,
            int meshvertex,
            int meshvertices,
            int lightmap,
            int lmX,
            int lmY,
            int lmWidth,
            int lmHeight,
            Vector3 lmOrigin,
            Vector3 lmSVec,
            Vector3 lmTVec,
            Vector3 normal,
            int patchW,
            int patchH)
        {
            textureIndex = texture;
            effectIndex = effect;
            faceType = facetype;
            startVertex = vertex;
            vertexCount = vertices;
            startMeshVertex = meshvertex;
            meshVertexCount = meshvertices;
            lightMapIndex = lightmap;
            lm_startX = lmX;
            lm_startY = lmY;
            lm_width = lmWidth;
            lm_height = lmHeight;
            lm_origin = lmOrigin;
            lm_sVec = lmSVec;
            lm_tVec = lmTVec;
            faceNormal = normal;
            patchWidth = patchW;
            patchHeight = patchH;

            patchIndex = -1;
        }

        public static Q3BSPFace FromStream(BinaryReader br)
        {
            int texture = br.ReadInt32();
            int effect = br.ReadInt32();
            int type = br.ReadInt32();
            int vertex = br.ReadInt32();
            int n_vertices = br.ReadInt32();
            int meshvert = br.ReadInt32();
            int n_meshverts = br.ReadInt32();
            int lm_index = br.ReadInt32();

            int[] lm_start = new int[2];
            lm_start[0] = br.ReadInt32();
            lm_start[1] = br.ReadInt32();

            int[] lm_size = new int[2];
            lm_size[0] = br.ReadInt32();
            lm_size[1] = br.ReadInt32();

            Vector3 lm_origin = new Vector3();
            lm_origin.X = br.ReadSingle();
            lm_origin.Y = br.ReadSingle();
            lm_origin.Z = br.ReadSingle();

            float[] lm_vecs = new float[6];
            lm_vecs[0] = br.ReadSingle();
            lm_vecs[1] = br.ReadSingle();
            lm_vecs[2] = br.ReadSingle();
            lm_vecs[3] = br.ReadSingle();
            lm_vecs[4] = br.ReadSingle();
            lm_vecs[5] = br.ReadSingle();

            Vector3 normal = new Vector3();
            normal.X = br.ReadSingle();
            normal.Z = -br.ReadSingle();
            normal.Y = br.ReadSingle();

            int[] size = new int[2];
            size[0] = br.ReadInt32();
            size[1] = br.ReadInt32();

            return new Q3BSPFace(
                texture,
                effect,
                type,
                vertex,
                n_vertices,
                meshvert,
                n_meshverts,
                lm_index,
                lm_start[0],
                lm_start[1],
                lm_size[0],
                lm_size[1],
                lm_origin,
                new Vector3(lm_vecs[0], lm_vecs[1], lm_vecs[2]),
                new Vector3(lm_vecs[3], lm_vecs[4], lm_vecs[5]),
                normal,
                size[0],
                size[1]);
        }

        #region Properties
        public int TextureIndex
        {
            get { return textureIndex; }
            set { textureIndex = value; }
        }

        public int EffectIndex
        {
            get { return effectIndex; }
            set { effectIndex = value; }
        }

        public int FaceType
        {
            get { return faceType; }
            set { faceType = value; }
        }

        public int StartVertex
        {
            get { return startVertex; }
            set { startVertex = value; }
        }

        public int VertexCount
        {
            get { return vertexCount; }
            set { vertexCount = value; }
        }

        public int StartMeshVertex
        {
            get { return startMeshVertex; }
            set { startMeshVertex = value; }
        }

        public int MeshVertexCount
        {
            get { return meshVertexCount; }
            set { meshVertexCount = value; }
        }

        public int LightMapIndex
        {
            get { return lightMapIndex; }
            set { lightMapIndex = value; }
        }

        public int PatchWidth
        {
            get { return patchWidth; }
            set { patchWidth = value; }
        }

        public int PatchHeight
        {
            get { return patchHeight; }
            set { patchHeight = value; }
        }

        public Vector3 FaceNormal
        {
            get { return faceNormal; }
            set { faceNormal = value; }
        }

        public int PatchIndex
        {
            get { return patchIndex; }
            set { patchIndex = value; }
        }

        public static int SizeInBytes
        {
            get
            {
                return ((sizeof(int) * 8) +
                    (sizeof(int) * 2) +
                    (sizeof(int) * 2) +
                    (sizeof(float) * 3) +
                    (sizeof(float) * 6) +
                    (sizeof(float) * 3) +
                    (sizeof(int) * 2));
            }
        }
        #endregion
    }
}

using System;
using System.Collections;
using System.Linq;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace Quake3
{
    public class Q3BSPLevel
    {
        #region Variables
        Q3BSPTextureData[] textureData;
        Plane[] planes;
        Q3BSPNode[] nodes;
        Q3BSPLeaf[] leafs;
        int[] leafFaces;
        int[] leafBrushes;
        Q3BSPModel[] models;
        Q3BSPBrush[] brushes;
        Q3BSPBrushSide[] brushSides;
        Q3BSPVertex[] vertices;
        int[] meshVertices;
        Q3BSPEffect[] effects;
        Q3BSPFace[] faces;
        Q3BSPLightMapData[] lightMapData;
        Q3BSPLightVolume[] lightVolumes;
        Q3BSPVisData visData;
        Q3BSPPatch[] patches;
        Q3BSPLightMapManager lightMapManager;
        Q3BSPShaderManager shaderManager;
        Q3BSPEntityManager entityManager;

        VertexDeclaration vertexDeclaration;

        string levelBasePath = "e:\\quake3\\data\\";
        string effectpath = "";
        string textureextension = "";
        bool levelInitialized = false;
        bool[] facesToDraw;
        #endregion

        public Q3BSPLevel(String BasePath, String Shader, String Extension)
        {
            levelBasePath = BasePath;
            effectpath = Shader;
            textureextension = Extension;
        }

        public bool InitializeLevel(GraphicsDevice graphics, ContentManager content)
        {
            bool bSuccess = true;


            lightMapManager = new Q3BSPLightMapManager();
            bSuccess = lightMapManager.GenerateLightMaps(lightMapData, graphics);

            if (bSuccess)
            {
                shaderManager = new Q3BSPShaderManager(levelBasePath, effectpath, textureextension);
                shaderManager.BasePath = levelBasePath;
                bSuccess = shaderManager.LoadTextures(textureData, graphics, content);
            }

            if (bSuccess)
            {
                shaderManager.LightMapManager = lightMapManager;
                vertexDeclaration = Q3BSPVertex.VertexDeclaration;
            }

            levelInitialized = bSuccess;

            return levelInitialized;
        }

        public Q3BSPCollisionData Trace(Vector3 startPosition, Vector3 endPosition)
        {
            Q3BSPCollisionData cd = new Q3BSPCollisionData();

            cd.startOutside = true;
            cd.inSolid = false;
            cd.ratio = 1.0f;
            cd.startPosition = startPosition;
            cd.endPosition = endPosition;
            cd.collisionPoint = startPosition;

            WalkNode(0, 0.0f, 1.0f, startPosition, endPosition, ref cd);

            if (1.0f == cd.ratio)
            {
                cd.collisionPoint = endPosition;
            }
            else
            {
                cd.collisionPoint = startPosition + cd.ratio * (endPosition - startPosition);
            }

            return cd;
        }

        private void WalkNode(int nodeIndex, float startRatio, float endRatio, Vector3 startPosition, Vector3 endPosition, ref Q3BSPCollisionData cd)
        {
            // Is this a leaf?
            if (0 > nodeIndex)
            {
                Q3BSPLeaf leaf = leafs[-(nodeIndex + 1)];
                for (int i = 0; i < leaf.LeafBrushCount; i++)
                {
                    Q3BSPBrush brush = brushes[leafBrushes[leaf.StartLeafBrush + i]];
                    if (0 < brush.BrushSideCount &&
                        1 == (textureData[brush.TextureIndex].Contents & 1))
                    {
                        CheckBrush(ref brush, ref cd);
                    }
                }

                return;
            }

            // This is a node
            Q3BSPNode thisNode = nodes[nodeIndex];
            Plane thisPlane = planes[thisNode.Plane];
            float startDistance = Vector3.Dot(startPosition, thisPlane.Normal) - thisPlane.D;
            float endDistance = Vector3.Dot(endPosition, thisPlane.Normal) - thisPlane.D;

            if (startDistance >= 0 && endDistance >= 0)
            {
                // Both points are in front
                WalkNode(thisNode.Left, startRatio, endRatio, startPosition, endPosition, ref cd);
            }
            else if (startDistance < 0 && endDistance < 0)
            {
                WalkNode(thisNode.Right, startRatio, endRatio, startPosition, endPosition, ref cd);
            }
            else
            {
                // The line spans the splitting plane
                int side = 0;
                float fraction1 = 0.0f;
                float fraction2 = 0.0f;
                float middleFraction = 0.0f;
                Vector3 middlePosition = new Vector3();

                if (startDistance < endDistance)
                {
                    side = 1;
                    float inverseDistance = 1.0f / (startDistance - endDistance);
                    fraction1 = (startDistance + Q3BSPConstants.epsilon) * inverseDistance;
                    fraction2 = (startDistance + Q3BSPConstants.epsilon) * inverseDistance;
                }
                else if (endDistance < startDistance)
                {
                    side = 0;
                    float inverseDistance = 1.0f / (startDistance - endDistance);
                    fraction1 = (startDistance + Q3BSPConstants.epsilon) * inverseDistance;
                    fraction2 = (startDistance - Q3BSPConstants.epsilon) * inverseDistance;
                }
                else
                {
                    side = 0;
                    fraction1 = 1.0f;
                    fraction2 = 0.0f;
                }

                if (fraction1 < 0.0f) fraction1 = 0.0f;
                else if (fraction1 > 1.0f) fraction1 = 1.0f;
                if (fraction2 < 0.0f) fraction2 = 0.0f;
                else if (fraction2 > 1.0f) fraction2 = 1.0f;

                middleFraction = startRatio + (endRatio - startRatio) * fraction1;
                middlePosition = startPosition + fraction1 * (endPosition - startPosition);

                int side1;
                int side2;
                if (0 == side)
                {
                    side1 = thisNode.Left;
                    side2 = thisNode.Right;
                }
                else
                {
                    side1 = thisNode.Right;
                    side2 = thisNode.Left;
                }

                WalkNode(side1, startRatio, middleFraction, startPosition, middlePosition, ref cd);

                middleFraction = startRatio + (endRatio - startRatio) * fraction2;
                middlePosition = startPosition + fraction2 * (endPosition - startPosition);

                WalkNode(side2, middleFraction, endRatio, middlePosition, endPosition, ref cd);
            }
        }

        private void CheckBrush(ref Q3BSPBrush brush, ref Q3BSPCollisionData cd)
        {
            float startFraction = -1.0f;
            float endFraction = 1.0f;
            bool startsOut = false;
            bool endsOut = false;

            for (int i = 0; i < brush.BrushSideCount; i++)
            {
                Q3BSPBrushSide brushSide = brushSides[brush.StartBrushSide + i];
                Plane plane = planes[brushSide.PlaneIndex];

                float startDistance = Vector3.Dot(cd.startPosition, plane.Normal) - plane.D;
                float endDistance = Vector3.Dot(cd.endPosition, plane.Normal) - plane.D;

                if (startDistance > 0)
                    startsOut = true;
                if (endDistance > 0)
                    endsOut = true;

                if (startDistance > 0 && endDistance > 0)
                {
                    return;
                }

                if (startDistance <= 0 && endDistance <= 0)
                {
                    continue;
                }

                if (startDistance > endDistance)
                {
                    float fraction = (startDistance - Q3BSPConstants.epsilon) / (startDistance - endDistance);
                    if (fraction > startFraction)
                        startFraction = fraction;
                }
                else
                {
                    float fraction = (startDistance + Q3BSPConstants.epsilon) / (startDistance - endDistance);
                    if (fraction < endFraction)
                        endFraction = fraction;
                }
            }

            if (false == startsOut)
            {
                cd.startOutside = false;
                if (false == endsOut)
                    cd.inSolid = true;

                return;
            }

            if (startFraction < endFraction)
            {
                if (startFraction > -1.0f && startFraction < cd.ratio)
                {
                    if (startFraction < 0)
                        startFraction = 0;
                    cd.ratio = startFraction;
                }
            }
        }

        public void RenderLevel(Vector3 cameraPosition, Matrix viewMatrix, Matrix projMatrix, GameTime gameTime, GraphicsDevice graphics)
        {
            int cameraLeaf = GetCameraLeaf(cameraPosition);
            int cameraCluster = leafs[cameraLeaf].Cluster;

            if (0 > cameraCluster)
            {
                return;
            }

            ResetFacesToDraw();

            BoundingFrustum frustum = new BoundingFrustum(viewMatrix * projMatrix);
            ArrayList visibleFaces = new ArrayList();
            foreach (Q3BSPLeaf leaf in leafs)
            {
                if (!visData.FastIsClusterVisible(cameraCluster, leaf.Cluster))
                {
                    continue;
                }

                if (!frustum.Intersects(leaf.Bounds))
                {
                    //continue;
                }

                for (int i = 0; i < leaf.LeafFaceCount; i++)
                {
                    int faceIndex = leafFaces[leaf.StartLeafFace + i];
                    Q3BSPFace face = faces[faceIndex];
                    if (4 != face.FaceType && !facesToDraw[faceIndex])
                    {
                        facesToDraw[faceIndex] = true;
                        visibleFaces.Add(face);
                    }
                }
            }

            if (0 >= visibleFaces.Count)
            {
                return;
            }

            Q3BSPFaceComparer fc = new Q3BSPFaceComparer();
            visibleFaces.Sort(fc);

            Matrix matrixWorldViewProjection = viewMatrix * projMatrix;
            Effect effect;
            foreach (Q3BSPFace face in visibleFaces)
            {
                effect = shaderManager.GetEffect(face.TextureIndex, face.LightMapIndex);
                if (null != effect)
                {
                    effect.Parameters["WorldViewProj"].SetValue(matrixWorldViewProjection);
                    effect.Parameters["WorldView"].SetValue(viewMatrix);
                    if (Q3BSPConstants.faceTypePatch == face.FaceType)
                    {
                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            patches[face.PatchIndex].Draw(graphics);
                        }
                    }
                    else
                    {
                        RenderFace(face, effect, graphics);
                    }
                }
            }
        }

        private void RenderFace(Q3BSPFace face, Effect effect, GraphicsDevice graphics)
        {
            int[] indices;
            int triCount = face.MeshVertexCount / 3;

            indices = new int[face.MeshVertexCount];
            for (int i = 0; i < face.MeshVertexCount; i++)
            {
                indices[i] = meshVertices[face.StartMeshVertex + i];
            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<Q3BSPVertex>(
                    PrimitiveType.TriangleList,
                    vertices,
                    face.StartVertex,
                    face.VertexCount,
                    indices,
                    0,
                    triCount);
            }
        }

        private int GetCameraLeaf(Vector3 cameraPosition)
        {
            int currentNode = 0;

            while (0 <= currentNode)
            {
                Plane currentPlane = planes[nodes[currentNode].Plane];
                if (PlaneIntersectionType.Front == ClassifyPoint(currentPlane, cameraPosition))
                {
                    currentNode = nodes[currentNode].Left;
                }
                else
                {
                    currentNode = nodes[currentNode].Right;
                }
            }

            return (~currentNode);
        }

        private PlaneIntersectionType ClassifyPoint(Plane plane, Vector3 pos)
        {
            float e = Vector3.Dot(plane.Normal, pos) - plane.D;

            if (e > Q3BSPConstants.epsilon)
            {
                return PlaneIntersectionType.Front;
            }

            if (e < -Q3BSPConstants.epsilon)
            {
                return PlaneIntersectionType.Back;
            }

            return PlaneIntersectionType.Intersecting;
        }

        private void ResetFacesToDraw()
        {
            for (int i = 0; i < facesToDraw.Length; i++)
            {
                facesToDraw[i] = false;
            }
        }

        public bool LoadFromFile(string bspFileName)
        {
            String fn = levelBasePath + bspFileName;

            BinaryReader fileReader = null;
            bool bSuccess = true;

            if (!File.Exists(fn))
            {
                throw new Exception("Q3BSPLevel::LoadFromFile::  Missing bsp file");
            }

            try
            {
                fileReader = new BinaryReader(File.Open(fn, FileMode.Open, FileAccess.Read), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                bSuccess = false;
            }

            if (bSuccess)
            {
                bSuccess = LoadFromStream(fileReader);
                fileReader.Close();
            }

            return bSuccess;
        }

        public bool LoadFromStream(BinaryReader fileReader)
        {
            const int bspMagic = (0x49) | (0x42 << 8) | (0x53 << 16) | (0x50 << 24);
            uint magic;
            long fileStart = fileReader.BaseStream.Position;
            int bspVersion = 0;
            int i = 0;
            bool bSuccess = true;
            Q3BSPDirEntry[] dirEntries = new Q3BSPDirEntry[Q3BSPConstants.numberOfDirs];

            magic = fileReader.ReadUInt32();

            if (bspMagic != magic)
            {
                return false;
            }

            bspVersion = fileReader.ReadInt32();

            for (i = 0; i < Q3BSPConstants.numberOfDirs; i++)
            {
                int dirOffset = fileReader.ReadInt32();
                int dirLength = fileReader.ReadInt32();

                dirEntries[i] = new Q3BSPDirEntry(dirOffset, dirLength);
            }

            fileReader.BaseStream.Position = fileStart;
            fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpEntity].Offset, SeekOrigin.Current);
            string entityString = new string(fileReader.ReadChars(dirEntries[Q3BSPConstants.lumpEntity].Length));
            entityManager = new Q3BSPEntityManager();
            bool entityLoaded = entityManager.LoadEntities(entityString);


            fileReader.BaseStream.Position = fileStart;
            fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpTextures].Offset, SeekOrigin.Current);
            bSuccess = LoadTextureLump(dirEntries[Q3BSPConstants.lumpTextures], fileReader);

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpPlanes].Offset, SeekOrigin.Current);
                bSuccess = LoadPlaneLump(dirEntries[Q3BSPConstants.lumpPlanes], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpNodes].Offset, SeekOrigin.Current);
                bSuccess = LoadNodeLump(dirEntries[Q3BSPConstants.lumpNodes], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpLeafs].Offset, SeekOrigin.Current);
                bSuccess = LoadLeafLump(dirEntries[Q3BSPConstants.lumpLeafs], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpLeafFaces].Offset, SeekOrigin.Current);
                bSuccess = LoadLeafFaceLump(dirEntries[Q3BSPConstants.lumpLeafFaces], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpLeafBrushes].Offset, SeekOrigin.Current);
                bSuccess = LoadLeafBrushLump(dirEntries[Q3BSPConstants.lumpLeafBrushes], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpModels].Offset, SeekOrigin.Current);
                bSuccess = LoadModelLump(dirEntries[Q3BSPConstants.lumpModels], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpBrushes].Offset, SeekOrigin.Current);
                bSuccess = LoadBrushLump(dirEntries[Q3BSPConstants.lumpBrushes], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpBrushSides].Offset, SeekOrigin.Current);
                bSuccess = LoadBrushSideLump(dirEntries[Q3BSPConstants.lumpBrushSides], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpVertexes].Offset, SeekOrigin.Current);
                bSuccess = LoadVertexLump(dirEntries[Q3BSPConstants.lumpVertexes], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpMeshVertexes].Offset, SeekOrigin.Current);
                bSuccess = LoadMeshVertexLump(dirEntries[Q3BSPConstants.lumpMeshVertexes], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpEffects].Offset, SeekOrigin.Current);
                bSuccess = LoadEffectLump(dirEntries[Q3BSPConstants.lumpEffects], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpFaces].Offset, SeekOrigin.Current);
                bSuccess = LoadFaceLump(dirEntries[Q3BSPConstants.lumpFaces], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpLightMaps].Offset, SeekOrigin.Current);
                bSuccess = LoadLightMapLump(dirEntries[Q3BSPConstants.lumpLightMaps], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpLightVolumes].Offset, SeekOrigin.Current);
                bSuccess = LoadLightVolumeLump(dirEntries[Q3BSPConstants.lumpLightVolumes], fileReader);
            }

            if (bSuccess)
            {
                fileReader.BaseStream.Position = fileStart;
                fileReader.BaseStream.Seek(dirEntries[Q3BSPConstants.lumpVisData].Offset, SeekOrigin.Current);
                bSuccess = LoadVisDataLump(dirEntries[Q3BSPConstants.lumpVisData], fileReader);
            }

            if (bSuccess)
            {
                bSuccess = GeneratePatches(2);
            }

            return bSuccess;
        }

        private bool LoadTextureLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int textureCount = dir.Length / Q3BSPConstants.sizeTexture;

            textureData = new Q3BSPTextureData[textureCount];
            for (int i = 0; i < textureCount; i++)
            {
                textureData[i] = Q3BSPTextureData.FromStream(fileReader);
            }
            return true;
        }

        private bool LoadPlaneLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int planeCount = dir.Length / Q3BSPConstants.sizePlane;

            planes = new Plane[planeCount];

            for (int i = 0; i < planeCount; i++)
            {
                planes[i] = Q3BSPPlane.FromStream(fileReader);
            }
            return true;
        }

        private bool LoadNodeLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int nodeCount = dir.Length / Q3BSPConstants.sizeNode;

            nodes = new Q3BSPNode[nodeCount];

            for (int i = 0; i < nodeCount; i++)
            {
                nodes[i] = Q3BSPNode.FromStream(fileReader);
            }
            return true;
        }

        private bool LoadLeafLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int leafCount = dir.Length / Q3BSPConstants.sizeLeaf;

            leafs = new Q3BSPLeaf[leafCount];
            for (int i = 0; i < leafCount; i++)
            {
                leafs[i] = Q3BSPLeaf.FromStream(fileReader);
            }
            return true;
        }

        private bool LoadLeafFaceLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int leafFaceCount = dir.Length / Q3BSPConstants.sizeLeafFace;

            leafFaces = new int[leafFaceCount];
            for (int i = 0; i < leafFaceCount; i++)
            {
                leafFaces[i] = fileReader.ReadInt32();
            }
            return true;
        }

        private bool LoadLeafBrushLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int leafBrushCount = dir.Length / Q3BSPConstants.sizeLeafBrush;

            leafBrushes = new int[leafBrushCount];
            for (int i = 0; i < leafBrushCount; i++)
            {
                leafBrushes[i] = fileReader.ReadInt32();
            }
            return true;
        }

        private bool LoadModelLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int modelCount = dir.Length / Q3BSPConstants.sizeModel;

            models = new Q3BSPModel[modelCount];
            for (int i = 0; i < modelCount; i++)
            {
                models[i] = Q3BSPModel.FromStream(fileReader);
            }
            return true;
        }

        private bool LoadBrushLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int brushCount = dir.Length / Q3BSPConstants.sizeBrush;

            brushes = new Q3BSPBrush[brushCount];
            for (int i = 0; i < brushCount; i++)
            {
                brushes[i] = Q3BSPBrush.FromStream(fileReader);
            }
            return true;
        }

        private bool LoadBrushSideLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int brushSideCount = dir.Length / Q3BSPConstants.sizeBrushSide;

            brushSides = new Q3BSPBrushSide[brushSideCount];
            for (int i = 0; i < brushSideCount; i++)
            {
                brushSides[i] = Q3BSPBrushSide.FromStream(fileReader);
            }
            return true;
        }

        private bool LoadVertexLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int vertexCount = dir.Length / Q3BSPConstants.sizeVertex;

            vertices = new Q3BSPVertex[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i] = Q3BSPVertex.FromStream(fileReader);
            }
            return true;
        }

        private bool LoadMeshVertexLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int meshVertexCount = dir.Length / Q3BSPConstants.sizeMeshVertex;

            meshVertices = new int[meshVertexCount];
            for (int i = 0; i < meshVertexCount; i++)
            {
                meshVertices[i] = fileReader.ReadInt32();
            }
            return true;
        }

        private bool LoadEffectLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int effectCount = dir.Length / Q3BSPConstants.sizeEffect;

            effects = new Q3BSPEffect[effectCount];
            for (int i = 0; i < effectCount; i++)
            {
                effects[i] = Q3BSPEffect.FromStream(fileReader);
            }
            return true;
        }

        private bool LoadFaceLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int faceCount = dir.Length / Q3BSPConstants.sizeFace;

            faces = new Q3BSPFace[faceCount];
            facesToDraw = new bool[faceCount];
            for (int i = 0; i < faceCount; i++)
            {
                faces[i] = Q3BSPFace.FromStream(fileReader);
            }
            return true;
        }

        private bool LoadLightMapLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int lightMapCount = dir.Length / Q3BSPConstants.sizeLightMap;

            lightMapData = new Q3BSPLightMapData[lightMapCount];
            for (int i = 0; i < lightMapCount; i++)
            {
                lightMapData[i] = new Q3BSPLightMapData();
                lightMapData[i].FromStream(fileReader);
            }
            return true;
        }

        private bool LoadLightVolumeLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            int lightVolumeCount = dir.Length / Q3BSPConstants.sizeLightVolume;

            lightVolumes = new Q3BSPLightVolume[lightVolumeCount];
            for (int i = 0; i < lightVolumeCount; i++)
            {
                lightVolumes[i] = Q3BSPLightVolume.FromStream(fileReader);
            }
            return true;
        }

        private bool LoadVisDataLump(Q3BSPDirEntry dir, BinaryReader fileReader)
        {
            visData = new Q3BSPVisData();
            visData.FromStream(fileReader);

            return true;
        }

        private bool GeneratePatches(int level)
        {
            int patchCount = 0;
            int i = 0;
            int patchIndex = 0;

            foreach (Q3BSPFace face in faces)
            {
                if (Q3BSPConstants.faceTypePatch == face.FaceType)
                {
                    patchCount++;
                }
            }

            if (0 == patchCount)
            {
                return true;
            }

            patches = new Q3BSPPatch[patchCount];

            patchIndex = 0;
            for (i = 0; i < faces.Length; i++)
            {
                if (Q3BSPConstants.faceTypePatch == faces[i].FaceType)
                {
                    Q3BSPPatch patch = new Q3BSPPatch();

                    patch.GeneratePatch(
                        vertices,
                        faces[i].StartVertex,
                        faces[i].VertexCount,
                        faces[i].PatchWidth,
                        faces[i].PatchHeight,
                        level);
                    faces[i].PatchIndex = patchIndex;
                    patches[patchIndex] = patch;
                    patchIndex++;
                }
            }

            return true;
        }

        #region Properties
        public string BasePath
        {
            get
            {
                return levelBasePath;
            }
            set
            {
                levelBasePath = value;
            }
        }
        #endregion


    }
}

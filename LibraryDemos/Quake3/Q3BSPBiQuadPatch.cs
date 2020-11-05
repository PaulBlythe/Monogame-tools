using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Quake3
{
    class Q3BSPBiQuadPatch
    {
        public Q3BSPVertex[] vertices;
        public int[] indices;
        int tesLevel;

        public void Tesselate(Q3BSPVertex[] controls, int level)
        {
            float px, py;
            Q3BSPVertex[] temp = new Q3BSPVertex[3];

            tesLevel = level;
            vertices = new Q3BSPVertex[(level + 1) * (level + 1)];

            for (int v = 0; v <= level; v++)
            {
                px = (float)v / level;
                float a = ((1.0f - px) * (1.0f - px));
                float b = ((1.0f - px) * px * 2);
                float c = px * px;

                vertices[v] = controls[0] * a + controls[3] * b + controls[6] * c;
            }

            for (int u = 1; u <= level; u++)
            {
                py = (float)u / level;
                float a = ((1.0f - py) * (1.0f - py));
                float b = ((1.0f - py) * py * 2);
                float c = py * py;

                temp[0] = controls[0] * a + controls[1] * b + controls[2] * c;
                temp[1] = controls[3] * a + controls[4] * b + controls[5] * c;
                temp[2] = controls[6] * a + controls[7] * b + controls[8] * c;

                for (int v = 0; v <= level; v++)
                {
                    px = (float)v / level;

                    a = (1.0f - px) * (1.0f - px);
                    b = (1.0f - px) * px * 2;
                    c = px * px;

                    vertices[u * (level + 1) + v] = temp[0] * a + temp[1] * b + temp[2] * c;
                }
            }

            indices = new int[level * (level + 1) * 2];
            for (int row = 0; row < level; row++)
            {
                for (int pt = 0; pt <= level; pt++)
                {
                    indices[(row * (level + 1) + pt) * 2 + 1] = row * (level + 1) + pt;
                    indices[(row * (level + 1) + pt) * 2] = (row + 1) * (level + 1) + pt;
                }
            }
        }

        public void Draw(GraphicsDevice graphics)
        {
            int tris = (tesLevel + 1) * 2;
            for (int i = 0; i < tesLevel; i++)
            {

                graphics.DrawUserIndexedPrimitives<Q3BSPVertex>
                    (PrimitiveType.TriangleStrip,
                    vertices,
                    0,
                    vertices.Length,
                    indices,
                    i * tris,
                    tris - 2);


            }
        }
    }

    public class Q3BSPPatch
    {
        private int width = 0;
        private int height = 0;
        private int tesselation;
        private Q3BSPBiQuadPatch[] patches = null;

        public void GeneratePatch(Q3BSPVertex[] vList, int vStart, int nVerts, int size_w, int size_h, int level)
        {
            width = (size_w - 1) / 2;
            height = (size_h - 1) / 2;

            tesselation = level;
            patches = new Q3BSPBiQuadPatch[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Q3BSPBiQuadPatch bpatch = new Q3BSPBiQuadPatch();
                    Q3BSPVertex[] ctrlPoints = new Q3BSPVertex[9];
                    for (int r = 0; r < 3; r++)
                    {
                        for (int c = 0; c < 3; c++)
                        {
                            ctrlPoints[r * 3 + c] = vList[vStart + (y * 2 * size_w + x * 2) + r * size_w + c];
                        }
                    }
                    bpatch.Tesselate(ctrlPoints, level);
                    patches[(y * width + x)] = bpatch;
                }
            }
        }

        public void Draw(GraphicsDevice graphics)
        {
            foreach (Q3BSPBiQuadPatch patch in patches)
            {
                patch.Draw(graphics);
            }
        }

        #region Properties
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Tesselation
        {
            get { return tesselation; }
            set { tesselation = value; }
        }

        public int PatchCount
        {
            get
            {
                if (null != patches)
                {
                    return patches.Length;
                }

                return 0;
            }
        }
        #endregion
    }
}

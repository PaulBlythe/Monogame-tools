using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quake3
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Q3BSPLeaf
    {
        int clusterIndex;
        int leafArea;
        BoundingBox leafBounds;
        int startLeafFace; // Starting index of leaf face
        int leafFaceCount;
        int startLeafBrush; // Starting index of brush
        int leafBrushCount;

        public Q3BSPLeaf(
            int cluster,
            int area,
            BoundingBox bounds,
            int leafface,
            int leaffaces,
            int leafbrush,
            int leafbrushes)
        {
            clusterIndex = cluster;
            leafArea = area;
            leafBounds = bounds;
            startLeafFace = leafface;
            leafFaceCount = leaffaces;
            startLeafBrush = leafbrush;
            leafBrushCount = leafbrushes;
        }

        public static Q3BSPLeaf FromStream(BinaryReader br)
        {
            int cluster = br.ReadInt32();
            int area = br.ReadInt32();

            Vector3 bmin = new Vector3();
            bmin.X = br.ReadInt32() / Q3BSPConstants.scale;
            bmin.Z = -br.ReadInt32() / Q3BSPConstants.scale;
            bmin.Y = br.ReadInt32() / Q3BSPConstants.scale;

            Vector3 bmax = new Vector3();
            bmax.X = br.ReadInt32() / Q3BSPConstants.scale;
            bmax.Z = -br.ReadInt32() / Q3BSPConstants.scale;
            bmax.Y = br.ReadInt32() / Q3BSPConstants.scale;

            int leafface = br.ReadInt32();
            int n_leaffaces = br.ReadInt32();
            int leafbrush = br.ReadInt32();
            int n_leafbrushes = br.ReadInt32();

            return new Q3BSPLeaf(
                cluster,
                area,
                new BoundingBox(bmin, bmax),
                leafface,
                n_leaffaces,
                leafbrush,
                n_leafbrushes);
        }

        #region Properties
        public int Cluster
        {
            get { return clusterIndex; }
            set { clusterIndex = value; }
        }

        public int Area
        {
            get { return leafArea; }
            set { leafArea = value; }
        }

        public BoundingBox Bounds
        {
            get { return leafBounds; }
            set { leafBounds = value; }
        }

        public int StartLeafFace
        {
            get { return startLeafFace; }
            set { startLeafFace = value; }
        }

        public int LeafFaceCount
        {
            get { return leafFaceCount; }
            set { LeafFaceCount = value; }
        }

        public int StartLeafBrush
        {
            get { return startLeafBrush; }
            set { startLeafBrush = value; }
        }

        public int LeafBrushCount
        {
            get { return leafBrushCount; }
            set { leafBrushCount = value; }
        }

        public static int SizeInBytes
        {
            get { return sizeof(int) * 6 + sizeof(float) * 3 * 2; }
        }
        #endregion
    }
}

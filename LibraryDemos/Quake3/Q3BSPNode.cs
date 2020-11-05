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
    public struct Q3BSPNode
    {
        int planeIndex;
        int leftNode;
        int rightNode;
        BoundingBox nodeBounds;

        public Q3BSPNode(int planeIndex, int nodeLeft, int nodeRight, BoundingBox bounds)
        {
            this.planeIndex = planeIndex;
            this.leftNode = nodeLeft;
            this.rightNode = nodeRight;
            this.nodeBounds = bounds;
        }

        public static Q3BSPNode FromStream(BinaryReader br)
        {
            int plane;
            int left;
            int right;
            Vector3 min = new Vector3();
            Vector3 max = new Vector3();

            plane = br.ReadInt32();

            left = br.ReadInt32();
            right = br.ReadInt32();

            min.X = br.ReadInt32() / Q3BSPConstants.scale;
            min.Z = -br.ReadInt32() / Q3BSPConstants.scale;
            min.Y = br.ReadInt32() / Q3BSPConstants.scale;

            max.X = br.ReadInt32() / Q3BSPConstants.scale;
            max.Z = -br.ReadInt32() / Q3BSPConstants.scale;
            max.Y = br.ReadInt32() / Q3BSPConstants.scale;

            return new Q3BSPNode(plane, left, right, new BoundingBox(min, max));
        }

        #region Properties
        public int Plane
        {
            get { return planeIndex; }
            set { planeIndex = value; }
        }

        public int Left
        {
            get { return leftNode; }
            set { leftNode = value; }
        }

        public int Right
        {
            get { return rightNode; }
            set { rightNode = value; }
        }

        public BoundingBox Bounds
        {
            get { return nodeBounds; }
            set { nodeBounds = value; }
        }

        public static int SizeInBytes
        {
            get
            {
                return (sizeof(float) * 3 * 2 + sizeof(int) * 3);
            }
        }
        #endregion
    }
}

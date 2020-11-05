using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Quake3
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Q3BSPVisData
    {
        int vectorCount;
        int vectorSize;
        public byte[] vectors;  // n_vecs * sz_vecs

        public Q3BSPVisData(int vCount, int vSize, byte[] vecs)
        {
            vectorCount = vCount;
            vectorSize = vSize;
            vectors = vecs;
        }

        public void FromStream(BinaryReader br)
        {
            vectorCount = br.ReadInt32();
            vectorSize = br.ReadInt32();

            vectors = new byte[vectorCount * vectorSize];
            vectors = br.ReadBytes(vectorCount * vectorSize);
        }

        public bool IsClusterVisible(int fromCluster, int toCluster)
        {
            if (null == vectors)
            {
                return false;
            }

            int index = fromCluster * vectorSize + toCluster / 8;

            if (0 <= index && vectors.Length > index)
                return ((vectors[index] & (1 << (toCluster & 7))) != 0);

            return false;
        }

        public bool FastIsClusterVisible(int fromCluster, int toCluster)
        {
            int index = fromCluster * vectorSize + toCluster / 8;
            return ((vectors[index] & (1 << (toCluster & 7))) != 0);
        }
    }
}

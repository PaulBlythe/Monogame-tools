using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Quake3
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Q3BSPBrushSide
    {
        int planeIndex;
        int textureIndex;

        public Q3BSPBrushSide(int plane, int texture)
        {
            planeIndex = plane;
            textureIndex = texture;
        }

        public static Q3BSPBrushSide FromStream(BinaryReader br)
        {
            int plane = br.ReadInt32();
            int texture = br.ReadInt32();

            return new Q3BSPBrushSide(plane, texture);
        }

        #region Properties
        public int PlaneIndex
        {
            get { return planeIndex; }
            set { planeIndex = value; }
        }

        public int TextureIndex
        {
            get { return textureIndex; }
            set { textureIndex = value; }
        }

        public static int SizeInBytes
        {
            get
            {
                return sizeof(int) * 2;
            }
        }
        #endregion
    }
}

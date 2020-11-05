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
    public struct Q3BSPBrush
    {
        int startBrushSide;
        int brushSideCount;
        int textureIndex;

        public Q3BSPBrush(int brushside, int brushsides, int texture)
        {
            startBrushSide = brushside;
            brushSideCount = brushsides;
            textureIndex = texture;
        }

        public static Q3BSPBrush FromStream(BinaryReader br)
        {
            int brushside = br.ReadInt32();
            int n_brushsides = br.ReadInt32();
            int texture = br.ReadInt32();

            return new Q3BSPBrush(brushside, n_brushsides, texture);
        }

        #region Properties
        public int StartBrushSide
        {
            get { return startBrushSide; }
            set { startBrushSide = value; }
        }

        public int BrushSideCount
        {
            get { return brushSideCount; }
            set { brushSideCount = value; }
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
                return sizeof(int) * 3;
            }
        }
        #endregion
    }
}

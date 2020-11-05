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
    public struct Q3BSPPlane
    {
        public static Plane FromStream(BinaryReader br)
        {
            float x = br.ReadSingle();
            float z = -br.ReadSingle();
            float y = br.ReadSingle();
            float d = br.ReadSingle() / Q3BSPConstants.scale;

            return new Plane(x, y, z, d);
        }
    }
}

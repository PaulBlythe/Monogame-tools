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
    public struct Q3BSPTextureData
    {
        public const int NameSize = 64;

        string textureName;
        int flags;
        int contents;

        public Q3BSPTextureData(string name, int f, int c)
        {
            textureName = name;
            flags = f;
            contents = c;
        }

        public static Q3BSPTextureData FromStream(BinaryReader br)
        {
            string name = new string(br.ReadChars(NameSize));
            name = name.TrimEnd(new char[] { '\0', ' ' });

            int f = br.ReadInt32();
            int c = br.ReadInt32();

            return new Q3BSPTextureData(name, f, c);
        }

        #region Properties
        public string Name
        {
            get { return textureName; }
            set { textureName = value; }
        }

        public int Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        public int Contents
        {
            get { return contents; }
            set { contents = value; }
        }
        #endregion
    }
}

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
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Q3BSPEffect
    {
        private const int NAME_SIZE = 64;
        string effectName;
        int brushIndex;
        int unknownData;

        public Q3BSPEffect(string name, int brush, int unknown)
        {
            effectName = name;
            brushIndex = brush;
            unknownData = unknown;
        }

        public static Q3BSPEffect FromStream(BinaryReader br)
        {
            string name = new string(br.ReadChars(NAME_SIZE));
            name = name.TrimEnd(new char[] { '\0', ' ' });
            int brush = br.ReadInt32();
            int unknown = br.ReadInt32();

            return new Q3BSPEffect(name, brush, unknown);
        }

        #region Properties
        public string EffectName
        {
            get { return effectName; }
            set { effectName = value; }
        }

        public int BrushIndex
        {
            get { return brushIndex; }
            set { brushIndex = value; }
        }

        public int UnknownData
        {
            get { return unknownData; }
            set { unknownData = value; }
        }

        public static int SizeInBytes
        {
            get
            {
                return NAME_SIZE + sizeof(int) * 2;
            }
        }
        #endregion
    }
}

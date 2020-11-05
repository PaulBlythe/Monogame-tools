using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace Quake3
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Q3BSPLightVolume
    {
        Color ambient;
        Color directional;
        byte dirPhi;
        byte dirTheta;

        public Q3BSPLightVolume(Color amb, Color dir, byte phi, byte theta)
        {
            ambient = amb;
            directional = dir;
            dirPhi = phi;
            dirTheta = theta;
        }

        public static Q3BSPLightVolume FromStream(BinaryReader br)
        {
            byte[] ambient = new byte[3];
            ambient = br.ReadBytes(3);

            byte[] dir = new byte[3];
            dir = br.ReadBytes(3);

            byte[] ldir = new byte[2];
            ldir = br.ReadBytes(2);

            return new Q3BSPLightVolume(
                new Color(ambient[0], ambient[1], ambient[2]),
                new Color(dir[0], dir[1], dir[2]),
                ldir[0],
                ldir[1]);
        }

        #region Properties
        public Color Ambient
        {
            get { return ambient; }
            set { ambient = value; }
        }

        public Color Directional
        {
            get { return directional; }
            set { directional = value; }
        }

        public byte Phi
        {
            get { return dirPhi; }
            set { dirPhi = value; }
        }

        public byte Theta
        {
            get { return dirTheta; }
            set { dirTheta = value; }
        }

        public static int SizeInBytes
        {
            get
            {
                return (sizeof(int) + sizeof(int) + sizeof(Byte) * 2);
            }
        }
        #endregion
    }
}

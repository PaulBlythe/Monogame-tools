using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;


namespace Quake3
{
    public struct Q3BSPLightMapData
    {
        public const int LightMapDimension = 128;
        public const int LightMapDataSize = 128 * 128 * 3;

        public byte[] mapData;

        public Q3BSPLightMapData(byte[] data)
        {
            mapData = data;
        }

        public void FromStream(BinaryReader br)
        {
            mapData = new byte[LightMapDataSize];
            mapData = br.ReadBytes(LightMapDataSize);
        }

        public Texture2D GenerateTexture(GraphicsDevice graphicsDevice, float gamma)
        {
            if (null == mapData)
            {
                return null;
            }

            Texture2D thisTexture;

            thisTexture = new Texture2D(graphicsDevice, LightMapDimension, LightMapDimension);

            uint[] lightData;
            lightData = new uint[LightMapDimension * LightMapDimension];

            for (int j = 0; j < (LightMapDimension * LightMapDimension); j++)
            {
                uint r, g, b;
                float rf, gf, bf;
                r = mapData[j * 3];
                g = mapData[j * 3 + 1];
                b = mapData[j * 3 + 2];

                rf = r * gamma / 255.0f;
                gf = g * gamma / 255.0f;
                bf = b * gamma / 255.0f;

                float scale = 1.0f;
                float temp;

                if (rf > 1.0f && (temp = (1.0f / rf)) < scale) scale = temp;
                if (gf > 1.0f && (temp = (1.0f / gf)) < scale) scale = temp;
                if (bf > 1.0f && (temp = (1.0f / bf)) < scale) scale = temp;

                scale *= 255.0f;
                r = (uint)(rf * scale);
                g = (uint)(gf * scale);
                b = (uint)(bf * scale);

                lightData[j] = (b << 0) | (g << 8) | (r << 16) | (((uint)0xFF) << 24);
            }

            thisTexture.SetData<uint>(lightData);

            return thisTexture;
        }

        #region Properties
        public static int SizeInBytes
        {
            get { return LightMapDataSize; }
        }
        #endregion
    }
}

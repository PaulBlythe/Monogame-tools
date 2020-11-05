using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Quake3
{
    public class Q3BSPLightMapManager
    {
        Texture2D[] lightMaps;
        private const int lightMapSize = 128;
        public bool GenerateLightMaps(Q3BSPLightMapData[] lightMapData, GraphicsDevice graphicsDevice)
        {
            int ltCount = lightMapData.Length;
            lightMaps = new Texture2D[ltCount + 1];

            for (int i = 0; i < ltCount; i++)
            {
                lightMaps[i] = lightMapData[i].GenerateTexture(graphicsDevice, 2.75f);
            }

            // set a white 2x2 texture for default lightmap
            {
                Texture2D defLt = new Texture2D(graphicsDevice, 2, 2, false, SurfaceFormat.Color, 1);
                uint[] ltData = new uint[2 * 2];
                for (int l = 0; l < ltData.Length; l++)
                {
                    ltData[l] = 0xFF7F7F7F;
                }

                defLt.SetData<uint>(ltData);
                lightMaps[ltCount] = defLt;
            }

            return true;
        }

        public Texture2D GetLightMap(int index)
        {
            if (0 <= index && (lightMaps.Length - 1) > index)
            {
                return lightMaps[index];
            }

            return lightMaps[lightMaps.Length - 1];
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Quake3
{
    public class Q3BSPShaderManager
    {
        const string noShader = "noshader";
        string fileExt = ".jpg";
        string textureBasePath = "e:\\quake3\\data\\";
        string effect;

        Texture2D[] diffuseTextures;
        Texture2D nullTexture;
        Q3BSPLightMapManager lightMapManager = null;
        Effect basicEffect = null;

        public Q3BSPShaderManager(String BasePath, String Shader, String FileExt)
        {
            textureBasePath = BasePath;
            effect = Shader;
            fileExt = FileExt;
        }

        public bool LoadTextures(Q3BSPTextureData[] textures, GraphicsDevice graphics, ContentManager content)
        {
            string texName;
            int texCount = textures.Length;

            diffuseTextures = new Texture2D[texCount];

            for (int i = 0; i < texCount; i++)
            {
                texName = textures[i].Name.Trim();
                Texture2D thisTexture = null;

                if (noShader != texName)
                {
                    texName = texName.Replace('/', '\\');
                    if (File.Exists(textureBasePath + texName + fileExt))
                    {
                        using (var stream = new System.IO.FileStream(textureBasePath + texName + fileExt, FileMode.Open))
                        {
                            thisTexture = Texture2D.FromStream(graphics, stream);
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Missing texture " + textureBasePath + texName + fileExt);
                    }
                }
                diffuseTextures[i] = thisTexture;
            }

            basicEffect = content.Load<Effect>(effect);

            if (null == basicEffect)
            {
                throw (new Exception("basicQ3Effect failed to load. Ensure 'basicQ3Effect.fx' is added to the project."));
            }

            {
                Texture2D defT = new Texture2D(graphics, 2, 2, false, SurfaceFormat.Color, 1);
                uint[] ltData = new uint[2 * 2];
                for (int l = 0; l < ltData.Length; l++)
                {
                    ltData[l] = 0xFFFFFFFF;
                }

                defT.SetData<uint>(ltData);
                nullTexture = defT;
            }

            return true;
        }

        public Effect GetEffect(int textureIndex, int lightMapIndex)
        {
            Texture2D tex = null;
            Texture2D ltm = null;

            if (null == basicEffect)
            {
                return null;
            }

            if (0 <= textureIndex && diffuseTextures.Length > textureIndex)
            {
                tex = diffuseTextures[textureIndex];
            }

            if (null != lightMapManager)
            {
                ltm = lightMapManager.GetLightMap(lightMapIndex);
            }

            if (null == tex)
            {
                tex = nullTexture;
            }

            basicEffect.Parameters["DiffuseTexture"].SetValue(tex);
            basicEffect.Parameters["LightMapTexture"].SetValue(ltm);
            if (null == ltm)
            {
                basicEffect.CurrentTechnique = basicEffect.Techniques["TransformAndTextureDiffuse"];
            }
            else
            {
                basicEffect.CurrentTechnique = basicEffect.Techniques["TransformAndTextureDiffuseAndLightMap"];
            }

            return basicEffect;
        }

        public string BasePath
        {
            get
            {
                return textureBasePath;
            }
            set
            {
                textureBasePath = value;
            }
        }

        public Q3BSPLightMapManager LightMapManager
        {
            get
            {
                return lightMapManager;
            }
            set
            {
                lightMapManager = value;
            }
        }
    }
}

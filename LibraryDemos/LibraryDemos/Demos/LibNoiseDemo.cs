using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

namespace LibraryDemos.Demos
{
    public class LibNoiseDemo:DemoClass
    {
        GraphicsDevice m_graphics = null;
        SpriteBatch m_spriteBatch = null;
        Noise2D m_noiseMap = null;
        Texture2D[] m_textures = new Texture2D[4];
        ContentManager m_content = null;
        float zoom = 0.5f;
        Perlin perlin = new Perlin();
        RiggedMultifractal rigged = new RiggedMultifractal();
        Add add;

        Rectangle[] Buttons = new Rectangle[]
        {
            new Rectangle(10,800,60,32),
            new Rectangle(210,800,60,32),
        };

        String[] ButtonLabels = new string[]
        {
            "-",
            "+",
        };

        public override void Initialise(GraphicsDevice device, ContentManager content)
        {
            m_graphics = device;
            m_spriteBatch = new SpriteBatch(device);
            m_content = content;

            // Create the module network
           
            add = new Add(perlin, rigged);

            // Initialize the noise map
            m_noiseMap = new Noise2D(256, 256, add);
            m_noiseMap.GeneratePlanar(-1, 1, -1, 1);

            // Generate the textures
            m_textures[0] = m_noiseMap.GetTexture(m_graphics, Gradient.Grayscale);
            m_textures[1] = m_noiseMap.GetTexture(m_graphics, Gradient.Terrain);
            m_textures[2] = m_noiseMap.GetNormalMap(m_graphics, 3.0f);

            // Zoom in or out do something like this.
            
            m_noiseMap.GeneratePlanar(-1 * zoom, 1 * zoom, -1 * zoom, 1 * zoom);
            m_textures[3] = m_noiseMap.GetTexture(m_graphics, Gradient.Terrain);
        }

        public override void Update(float dt)
        {
            if ((Game1.Instance.last_mouse_state.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) &&
                (Game1.Instance.current_mouse_state.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released))
            {
                for (int i=0; i<Buttons.Length; i++)
                {
                    if (Buttons[i].Contains(Game1.Instance.current_mouse_state.X,Game1.Instance.current_mouse_state.Y))
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    zoom -= 0.05f;
                                    m_noiseMap.GeneratePlanar(-1 * zoom, 1 * zoom, -1 * zoom, 1 * zoom);
                                    m_textures[3] = m_noiseMap.GetTexture(m_graphics, Gradient.Terrain);
                                }
                                break;

                            case 1:
                                {
                                    zoom += 0.05f;
                                    m_noiseMap.GeneratePlanar(-1 * zoom, 1 * zoom, -1 * zoom, 1 * zoom);
                                    m_textures[3] = m_noiseMap.GetTexture(m_graphics, Gradient.Terrain);
                                }
                                break;
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            int w = this.m_graphics.Viewport.Width / 4;
            m_graphics.Clear(Color.Black);
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_textures[0], new Rectangle(0, 0, w, w), Color.White);
            m_spriteBatch.Draw(m_textures[1], new Rectangle(w, 0, w, w), Color.White);
            m_spriteBatch.Draw(m_textures[2], new Rectangle(w * 2, 0, w, w), Color.White);
            m_spriteBatch.Draw(m_textures[3], new Rectangle(w * 3, 0, w, w), Color.White);

            m_spriteBatch.DrawString(Game1.Instance.MenuFont, "Zoom", new Vector2(110, 802), Color.White);
            m_spriteBatch.End();

            Game1.Instance.spriteBatch.Begin();
            for (int i=0; i<Buttons.Length; i++)
            {
                Game1.Instance.DrawButton(Buttons[i], ButtonLabels[i]);
            }
            Game1.Instance.spriteBatch.End();

        }

        public override void Unload()
        {
            m_noiseMap.Dispose();
        }
    }
}

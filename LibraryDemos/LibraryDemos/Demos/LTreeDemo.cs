using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using LTreesLibrary.Trees;

namespace LibraryDemos.Demos
{
    public class LTreeDemo:DemoClass
    {
        ContentManager m_content;
        GraphicsDevice m_device;
        Effect m_trunk;
        Effect m_leaves;
        Texture2D m_trunk_texture;
        Texture2D m_leaves_texture;

        Rectangle[] Buttons = new Rectangle[]
        {
            new Rectangle(10,800,200,32),
            new Rectangle(10,850,200,32),
            new Rectangle(10,900,200,32),
            new Rectangle(10,950,200,32),
            new Rectangle(210,800,200,32),
            new Rectangle(210,850,200,32),
        };

        String[] ButtonLabels = new string[]
        {
            "New Pine",
            "New Birch",
            "New Willow",
            "New Graywood",
            "New Gardenwood",
            "New Rug",
        };

        SimpleTree m_tree;
        TreeProfile m_profile;
        TreeGenerator m_generator;
        Random m_rand;

        Matrix m_projection;
        Matrix m_world;
        Matrix m_view;

        Vector3 m_camera_position;
        Vector3 m_camera_target;

        float angle = 0;

        public override void Initialise(GraphicsDevice device, ContentManager content)
        {
            m_content = content;
            m_camera_position = new Vector3(600, 2, -500);
            m_camera_target = new Vector3(0, 2, 0);
            m_rand = new Random();
            m_trunk = content.Load<Effect>("Trunk");
            m_leaves = content.Load<Effect>("Leaves");
            m_trunk_texture = content.Load<Texture2D>(@"Trees\PineBark");
            m_leaves_texture = content.Load<Texture2D>(@"Trees\PineLeaf");

            m_generator = TreeGenerator.CreateFromXml(@"Data\Trees\Pine.xml");
            m_profile = new TreeProfile(device, m_generator, m_trunk_texture, m_leaves_texture, m_trunk, m_leaves);
            m_tree = m_profile.GenerateSimpleTree(m_rand);

            m_projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), device.Viewport.AspectRatio, 0.5f, 20000.0f);
            m_world = Matrix.CreateScale(0.1f); // Matrix.CreateTranslation(Vector3.Zero);
            m_view = Matrix.CreateLookAt(m_camera_position, m_camera_target, Vector3.Up);
            m_device = device;
        }

        public override void Update(float dt)
        {
            angle += MathHelper.ToRadians(10) * dt;
            m_world = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY(angle);

            if ((Game1.Instance.last_mouse_state.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) &&
               (Game1.Instance.current_mouse_state.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released))
            {
                for (int i = 0; i < Buttons.Length; i++)
                {
                    if (Buttons[i].Contains(Game1.Instance.current_mouse_state.X, Game1.Instance.current_mouse_state.Y))
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    m_trunk_texture = m_content.Load<Texture2D>(@"Trees\PineBark");
                                    m_leaves_texture = m_content.Load<Texture2D>(@"Trees\PineLeaf");
                                    m_generator = TreeGenerator.CreateFromXml(@"Data\Trees\Pine.xml");
                                    m_profile = new TreeProfile(m_device, m_generator, m_trunk_texture, m_leaves_texture, m_trunk, m_leaves);
                                    m_tree = m_profile.GenerateSimpleTree(m_rand);
                                }
                                break;

                            case 1:
                                {
                                    m_trunk_texture = m_content.Load<Texture2D>(@"Trees\BirchBark");
                                    m_leaves_texture = m_content.Load<Texture2D>(@"Trees\BirchLeaf");
                                    m_generator = TreeGenerator.CreateFromXml(@"Data\Trees\Birch.xml");
                                    m_profile = new TreeProfile(m_device, m_generator, m_trunk_texture, m_leaves_texture, m_trunk, m_leaves);
                                    m_tree = m_profile.GenerateSimpleTree(m_rand);
                                }
                                break;

                            case 2:
                                {
                                    m_trunk_texture = m_content.Load<Texture2D>(@"Trees\wood_dark");
                                    m_leaves_texture = m_content.Load<Texture2D>(@"Trees\WillowLeaf");
                                    m_generator = TreeGenerator.CreateFromXml(@"Data\Trees\Willow.xml");
                                    m_profile = new TreeProfile(m_device, m_generator, m_trunk_texture, m_leaves_texture, m_trunk, m_leaves);
                                    m_tree = m_profile.GenerateSimpleTree(m_rand);
                                }
                                break;

                            case 3:
                                {
                                    m_trunk_texture = m_content.Load<Texture2D>(@"Trees\GrayBark");
                                    m_leaves_texture = m_content.Load<Texture2D>(@"Trees\GraywoodLeaf");
                                    m_generator = TreeGenerator.CreateFromXml(@"Data\Trees\Graywood.xml");
                                    m_profile = new TreeProfile(m_device, m_generator, m_trunk_texture, m_leaves_texture, m_trunk, m_leaves);
                                    m_tree = m_profile.GenerateSimpleTree(m_rand);
                                }
                                break;

                            case 4:
                                {
                                    m_trunk_texture = m_content.Load<Texture2D>(@"Trees\wood_dark");
                                    m_leaves_texture = m_content.Load<Texture2D>(@"Trees\OakLeaf");
                                    m_generator = TreeGenerator.CreateFromXml(@"Data\Trees\Gardenwood.xml");
                                    m_profile = new TreeProfile(m_device, m_generator, m_trunk_texture, m_leaves_texture, m_trunk, m_leaves);
                                    m_tree = m_profile.GenerateSimpleTree(m_rand);
                                }
                                break;

                            case 5:
                                {
                                    m_trunk_texture = m_content.Load<Texture2D>(@"Trees\PineBark");
                                    m_leaves_texture = m_content.Load<Texture2D>(@"Trees\WillowLeaf");
                                    m_generator = TreeGenerator.CreateFromXml(@"Data\Trees\Rug.xml");
                                    m_profile = new TreeProfile(m_device, m_generator, m_trunk_texture, m_leaves_texture, m_trunk, m_leaves);
                                    m_tree = m_profile.GenerateSimpleTree(m_rand);
                                }
                                break;
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            
            m_tree.DrawTrunk(m_world, m_view, m_projection, true, false);
            m_tree.DrawLeaves(m_world, m_view, m_projection, true, false);

            Game1.Instance.spriteBatch.Begin();
            for (int i = 0; i < Buttons.Length; i++)
            {
                Game1.Instance.DrawButton(Buttons[i], ButtonLabels[i]);
            }
            Game1.Instance.spriteBatch.End();
        }

        public override void Unload()
        {
            m_device.DepthStencilState = DepthStencilState.Default;
            m_content.Unload();
            m_content.Dispose();
        }
    }
}

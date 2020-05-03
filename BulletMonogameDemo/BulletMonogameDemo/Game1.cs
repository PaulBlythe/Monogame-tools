using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using BulletMonogameDemo.Demos;

namespace BulletMonogameDemo
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static Game1 Instance;
        public XNA_ShapeDrawer shape_drawer;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DemoApplication currentDemo = null;
        SpriteFont Menu;
        SpriteFont InGame;
        MouseState oldms;
        int over = -1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreferMultiSampling = false;
            Window.IsBorderless = true;
            Window.Position = new Point(0, 0);

            IsMouseVisible = true;
            Instance = this;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            shape_drawer = new XNA_ShapeDrawer(this);

            FrameRateCounter fc = new FrameRateCounter(this, new Vector3(600, 20, 10), shape_drawer);
            Components.Add(fc);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Menu = Content.Load<SpriteFont>("MenuFont");
            InGame = Content.Load<SpriteFont>("InGame");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            MouseState ms = Mouse.GetState();

            if (currentDemo == null)
            {
               
                if (over > -1)
                {
                    
                    if ((ms.LeftButton == ButtonState.Released) && (oldms.LeftButton == ButtonState.Pressed))
                    {
                        if (currentDemo != null)
                        {
                            currentDemo.Cleanup();
                            currentDemo.Dispose(true);
                        }

                        switch (over)
                        {
                            case 0:
                                currentDemo = new AndrewDemo();
                                break;
                            case 1:
                                currentDemo = new BasicDemo();
                                break;
                            case 2:
                                currentDemo = new Box2DDemo();
                                break;
                            case 3:
                                currentDemo = new BvhBugDemo();
                                break;
                            case 4:
                                currentDemo = new CharacterDemo();
                                break;
                            case 5:
                                currentDemo = new ConcaveDemo();
                                break;
                            case 6:
                                currentDemo = new ConcaveRaycastDemo();
                                break;
                            case 7:
                                currentDemo = new ConstraintDemo();
                                break;
                            case 8:
                                currentDemo = new CornerDemo();
                                break;
                            case 9:
                                currentDemo = new ForkLiftDemo();
                                break;
                            case 10:
                                currentDemo = new GhostObjectDemo();
                                break;
                            case 11:
                                currentDemo = new GImpactTestDemo();
                                break;
                            case 12:
                                currentDemo = new InternalEdgeDemo();
                                break;
                            case 13:
                                currentDemo = new IslandStressDemo();
                                break;
                            case 14:
                                currentDemo = new JengaDemo();
                                break;
                            case 15:
                                currentDemo = new LargeMeshDemo();
                                break;
                            case 16:
                                currentDemo = new MotorDemo();
                                break;
                            case 17:
                                currentDemo = new MultiWorldDemo();
                                break;
                            case 18:
                                currentDemo = new Pachinko();
                                break;
                            case 19:
                                currentDemo = new PyramidDemo();
                                break;
                            case 20:
                                currentDemo = new RagDollDemo();
                                break;
                            case 21:
                                currentDemo = new SimpleCompoundDemo();
                                break;
                            case 22:
                                currentDemo = new StaticLevelDemo();
                                break;
                            case 23:
                                currentDemo = new TerrainDemo();
                                break;


                        }

                        if (currentDemo != null)
                        {
                            currentDemo.Initialize();
                            
                            currentDemo.LoadContent();
                        }
                    }
                }
               
            }
            else
            {
                currentDemo.Update(gameTime);
                Rectangle back = new Rectangle(1750, 1000, 200, 30);
                if (back.Contains(ms.X,ms.Y))
                {
                    if ((ms.LeftButton==ButtonState.Released) && (oldms.LeftButton==ButtonState.Pressed))
                    {
                        currentDemo.Cleanup();
                        currentDemo.Dispose(true);
                        currentDemo = null;
                    }
                }
            }
            oldms = ms;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            MouseState ms = Mouse.GetState();

            if (currentDemo == null)
            {

                Rectangle r = new Rectangle(10, 10, 200, 30);
                spriteBatch.Begin();
                int i = 0;
                over = -1;
                foreach (String s in Demos)
                {
                    if (r.Contains(ms.X, ms.Y))
                    {
                        spriteBatch.FillRectangle(r, Color.DarkBlue);
                        over = i;
                    }
                    else
                        spriteBatch.FillRectangle(r, Color.LightBlue);
                    spriteBatch.DrawRectangle(r, Color.White);

                    Vector2 ss = Menu.MeasureString(s);
                    ss *= -0.5f;
                    ss.X += (r.X + (r.Width / 2));
                    ss.Y += (r.Y + (r.Height / 2));
                    spriteBatch.DrawString(Menu, s, ss, Color.Black);

                    r.X += 250;
                    if (r.X > (GraphicsDevice.Viewport.Width-210))
                    {
                        r.X = 10;
                        r.Y += 50;
                    }
                    i++;
                }
                
                spriteBatch.End();
            }
            else
            {
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                currentDemo.Draw(gameTime);

                if ((shape_drawer.GetDebugMode() & BulletMonogame.LinearMath.DebugDrawModes.DBG_NoHelpText) != 0)
                {
                    spriteBatch.Begin();
                    Vector2 pos = new Vector2(5, 25);
                    spriteBatch.DrawString(InGame, "L   Step left", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "R   Step right", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "F   Step front", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "B   Step back", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "Z   Zoom in", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "X   Zoom out", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "I   Idle", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "G   Toggle shadows", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "U   Toggle textures", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "W   Wireframe", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "P   Profile timings", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "M   SAT compare", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "N   LCP toggle", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "T   Text toggle", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "Y   Features text", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "A   Draw AABB", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "C   Draw contacts", pos, Color.Black);
                    pos.Y += 20;
                    spriteBatch.DrawString(InGame, "D   Toggle deactivation", pos, Color.Black);
                    spriteBatch.End();
                }
                Rectangle back = new Rectangle(1750, 1000, 200, 30);
                spriteBatch.Begin();
                if (back.Contains(ms.X, ms.Y))
                {
                    spriteBatch.FillRectangle(back, Color.LightBlue);
                    spriteBatch.DrawString(Menu, "Back", new Vector2(back.X + 5, back.Y + 5), Color.Black);
                }
                else
                {
                    spriteBatch.FillRectangle(back, Color.DarkBlue);
                    spriteBatch.DrawString(Menu, "Back", new Vector2(back.X + 5, back.Y + 5), Color.White);
                }
                spriteBatch.DrawRectangle(back, Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        String[] Demos = new String[]
        {
            "Andrew",
            "Basic",
            "Box2D",
            "BvhBug",
            "Character",
            "Concave",
            "Concave Ray",
            "Contraint",
            "Corner",
            "Forklift",
            "Ghost object",
            "GImpact test",
            "Internal edge",
            "Island stress",
            "Jenga",
            "Large mesh",
            "Motor",
            "Multiworld",
            "Pachinko",
            "Pyramid",
            "Rag doll",
            "Compound",
            "Static level",
            "Terrain"
        };

    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using LibraryDemos.Demos;
using C3.XNA;

namespace LibraryDemos
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static Game1 Instance;

        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        DemoClass current_demo = null;
        public SpriteFont MenuFont;
        public MouseState last_mouse_state;
        public MouseState current_mouse_state;

        Rectangle back_button = new Rectangle(10, 1040, 200, 32);

        Rectangle[] Buttons = new Rectangle[]
        {
            new Rectangle(20, 60, 200, 32),
            new Rectangle(20, 110, 200, 32),
        };

        string[] ButtonText = new string[]
        {
            "LibNoise",
            "LTree",
        };

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Window.Position = new Point(0, 0);
            Window.IsBorderless = true;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

            Instance = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
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
            MenuFont = Content.Load<SpriteFont>("MenuFont");
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            
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

            last_mouse_state = current_mouse_state;
            current_mouse_state = Mouse.GetState();

            if (current_demo != null)
            {
                current_demo.Update(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                if ((last_mouse_state.LeftButton == ButtonState.Pressed) && (current_mouse_state.LeftButton == ButtonState.Released))
                {
                    if (back_button.Contains(current_mouse_state.X,current_mouse_state.Y))
                    {
                        current_demo.Unload();
                        current_demo = null;
                    }
                }
            }
            else
            {
                if ((last_mouse_state.LeftButton == ButtonState.Pressed) && (current_mouse_state.LeftButton == ButtonState.Released))
                {
                    for (int i = 0; i < Buttons.Length; i++)
                    {
                        if (Buttons[i].Contains(current_mouse_state.X, current_mouse_state.Y))
                        {
                            switch (i)
                            {
                                case 0:
                                    {
                                        current_demo = new LibNoiseDemo();
                                        current_demo.Initialise(GraphicsDevice, Content);
                                        ContentManager manager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
                                        current_demo.Initialise(GraphicsDevice, manager);
                                    }
                                    break;

                                case 1:
                                    {
                                        current_demo = new LTreeDemo();
                                        current_demo.Initialise(GraphicsDevice, Content);
                                        ContentManager manager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
                                        current_demo.Initialise(GraphicsDevice, manager);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (current_demo != null)
            {
                current_demo.Draw();
                spriteBatch.Begin();
                DrawButton(back_button, "Back");
                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(MenuFont, "Handy MonoGame libraries for your delight and delectation.", new Vector2(10, 10), Color.White);

                for (int i = 0; i < Buttons.Length; i++)
                {
                    DrawButton(Buttons[i], ButtonText[i]);
                }
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public void DrawButton(Rectangle r, string s)
        {
            Vector2 size = MenuFont.MeasureString(s);
            size *= -0.5f;
            size.X += r.X + (r.Width / 2);
            size.Y += r.Y + (r.Height / 2);

            if (r.Contains(current_mouse_state.X,current_mouse_state.Y))
                spriteBatch.FillRectangle(r, Color.DarkOrange);
            else
                spriteBatch.FillRectangle(r, Color.DarkSlateBlue);

            r.X += 1;
            r.Y += 1;
            r.Width -= 2;
            r.Height -= 2;
            spriteBatch.DrawRectangle(r, Color.LightSlateGray);
            r.X += 1;
            r.Y += 1;
            r.Width -= 2;
            r.Height -= 2;
            spriteBatch.DrawRectangle(r, Color.Black);

            spriteBatch.DrawString(MenuFont, s, size, Color.White);
        }
    }
}

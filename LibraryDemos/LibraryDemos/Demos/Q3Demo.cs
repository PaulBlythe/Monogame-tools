using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Quake3;
using LibraryDemos.DemoHelpers;

namespace LibraryDemos.Demos
{
    public class Q3Demo : DemoClass
    {
        Q3BSPLevel level;
        QuatCamera camera;
        GameTime gameTime;
        MouseState oldMouseState;

        public override void Initialise(GraphicsDevice device, ContentManager content)
        {
            level = new Q3BSPLevel(@"C:\GitHub\Monogame-tools\LibraryDemos\LibraryDemos\Data\Q3\", @"Q3\basicQ3Effect", ".jpg");
            level.LoadFromFile(@"maps\13tokay.bsp");
            level.InitializeLevel(Game1.Instance.GraphicsDevice, Game1.Instance.Content);
            camera = new QuatCamera(Game1.Instance.GraphicsDevice.Viewport);
        }

        public override void Update(float dt)
        {
            gameTime = Game1.Instance.lastGameTime;
            camera.Update();

            MouseState ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                float dx = oldMouseState.X - ms.X;
                float dy = oldMouseState.Y - ms.Y;
                camera.RotateX(dy * 10 * dt);
                camera.RotateY(dx * 10 * dt);
            }
            if (ms.RightButton == ButtonState.Pressed)
            {
                float dx = oldMouseState.X - ms.X;
                camera.RotateZ(dx * 10 * dt);
            }
            float dz = oldMouseState.ScrollWheelValue - ms.ScrollWheelValue;
            camera.Move(camera.World.Forward, dz * 2 * dt);
            oldMouseState = ms;
        }

        public override void Draw()
        {
            Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game1.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            level.RenderLevel(camera.Position, camera.View, camera.Projection, gameTime, Game1.Instance.GraphicsDevice);
        }

        public override void Unload()
        {
            level = null;
        }
    }
}

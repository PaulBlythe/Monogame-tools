using System;
using System.Collections.Generic;
using System.Linq;

using LibraryDemos.DemoHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using C3.XNA;

namespace LibraryDemos.Demos
{ 
    public class OITDemo : DemoClass
    {
        GraphicsDevice Device;
        SpriteBatch batch;
        Texture2D backdrop;
        StaticCamera camera;
        SpriteFont font;
        Effect oit_pass1;
        Effect oit_pass2;
        Effect oit_pass3;
        RenderTarget2D oit_colour_rt;
        RenderTarget2D oit_weight_rt;
        BlendState bs = new BlendState();

        int nPlanes = 4;
        QuadPlane[] planes;
        float rotate = 0;
        int mode = 0;

        MouseState oldms;
        Rectangle LastButton = new Rectangle(100, 900, 100, 32);
        Rectangle NextButton = new Rectangle(1720, 900, 100, 32);

        BasicEffect Normal;

        public override void Initialise(GraphicsDevice device, ContentManager content)
        {
            Device = device;
            backdrop = content.Load<Texture2D>(@"Textures\OITBackdrop");
            oit_pass1 = content.Load<Effect>(@"Shaders\OITPass1");
            oit_pass2 = content.Load<Effect>(@"Shaders\OITPass2");
            oit_pass3 = content.Load<Effect>(@"Shaders\OITPass3");
            batch = new SpriteBatch(device);
            camera = new StaticCamera(new Vector3(0, 2, -150), Vector3.Zero, Device.Viewport.AspectRatio, MathHelper.ToRadians(60), 0.5f, 1000.0f);
            oit_colour_rt = new RenderTarget2D(Device, Device.Viewport.Width, Device.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
            oit_weight_rt =new RenderTarget2D(Device, Device.Viewport.Width, Device.Viewport.Height, false, SurfaceFormat.Single, DepthFormat.None);

            planes = new QuadPlane[nPlanes];
            for (int i=0; i<nPlanes; i++)
            {
                float angle = (i * 360.0f) / nPlanes;
                Color c = ColourSpaces.HSVToRGB(angle, 1, 1);             
                planes[i] = new QuadPlane(Vector3.Zero, 80, Vector3.Up, Vector3.Right, c);
            }
            Normal = new BasicEffect(Device);
            Normal.TextureEnabled = false;
            Normal.VertexColorEnabled = true;
            Normal.LightingEnabled = false;
            Normal.Projection = camera.Projection;
            Normal.View = camera.View;

            font = content.Load<SpriteFont>("MenuFont");

            bs.ColorBlendFunction = BlendFunction.Add;

            bs.AlphaSourceBlend = Blend.Zero;
            bs.ColorSourceBlend = Blend.Zero;

            bs.AlphaDestinationBlend = Blend.InverseSourceAlpha;
            bs.ColorDestinationBlend = Blend.InverseSourceAlpha;

           
        }

        public override void Update(float dt)
        {
            rotate += dt * 10;
            MouseState ms = Mouse.GetState();
            if ((ms.LeftButton == ButtonState.Released)&&(oldms.LeftButton== ButtonState.Pressed))
            {
                if (NextButton.Contains(ms.X,ms.Y))
                {
                    mode++;
                    mode = Math.Min(2, mode);
                }
                if (LastButton.Contains(ms.X, ms.Y))
                {
                    mode--;
                    mode = Math.Max(0, mode);
                }
            }
            oldms = ms;
        }

        public override void Draw()
        {
           


            switch (mode)
            {
                case 0:     // Alpha blended with depth buffer
                    {
                        batch.Begin();
                        batch.Draw(backdrop, new Rectangle(0, 0, Device.Viewport.Width, Device.Viewport.Height), Color.White);
                        batch.End();

                        Device.DepthStencilState = DepthStencilState.Default;
                        Device.RasterizerState = RasterizerState.CullNone;
                        Device.BlendState = BlendState.AlphaBlend;

                        float angle = 0;
                        for (int i = 0; i < nPlanes; i++)
                        {
                            angle = rotate + (i * 180.0f / nPlanes);
                            Matrix world = Matrix.CreateRotationY(MathHelper.ToRadians(angle));
                            Normal.World = world;
                            planes[i].Draw(Device, Normal);
                        }
                        batch.Begin();
                        Vector2 tp = font.MeasureString("Alpha blended with depth buffer");
                        tp = (tp * -0.5f) + new Vector2(1920 / 2, 800);
                        batch.DrawString(font, "Alpha blended with depth buffer", tp, Color.White);
                        batch.End();
                    }
                    break;
                case 1:     // Alpha blended without depth buffer
                    {
                        batch.Begin();
                        batch.Draw(backdrop, new Rectangle(0, 0, Device.Viewport.Width, Device.Viewport.Height), Color.White);
                        batch.End();

                        Device.DepthStencilState = DepthStencilState.None;
                        Device.RasterizerState = RasterizerState.CullNone;
                        Device.BlendState = BlendState.AlphaBlend;

                        float angle = 0;
                        for (int i = 0; i < nPlanes; i++)
                        {
                            angle = rotate + (i * 180.0f / nPlanes);
                            Matrix world = Matrix.CreateRotationY(MathHelper.ToRadians(angle));
                            Normal.World = world;
                            planes[i].Draw(Device, Normal);
                        }
                        batch.Begin();
                        Vector2 tp = font.MeasureString("Alpha blended without depth buffer");
                        tp = (tp * -0.5f) + new Vector2(1920 / 2, 800);
                        batch.DrawString(font, "Alpha blended without depth buffer", tp, Color.White);
                        batch.End();
                    }
                    break;

                case 2:     // OIT
                    {

                        // first pass. write modified colour
                        Device.DepthStencilState = DepthStencilState.None;
                        Device.RasterizerState = RasterizerState.CullNone;
                        Device.BlendState = BlendState.Additive;
                        Device.SetRenderTarget(oit_colour_rt);
                       
                        float angle = 0;
                        for (int i = 0; i < nPlanes; i++)
                        {
                            angle = rotate + (i * 180.0f / nPlanes);
                            Matrix world = Matrix.CreateRotationY(MathHelper.ToRadians(angle));
                            oit_pass1.Parameters["WorldViewProjection"].SetValue(world * camera.View * camera.Projection);
                            planes[i].Draw(Device, oit_pass1);
                        }

                        // second pass . write weight
                        Device.DepthStencilState = DepthStencilState.None;
                        Device.RasterizerState = RasterizerState.CullNone;
                        Device.BlendState = bs;
                        Device.SetRenderTarget(oit_weight_rt);

                        for (int i = 0; i < nPlanes; i++)
                        {
                            angle = rotate + (i * 180.0f / nPlanes);
                            Matrix world = Matrix.CreateRotationY(MathHelper.ToRadians(angle));
                            oit_pass2.Parameters["WorldViewProjection"].SetValue(world * camera.View * camera.Projection);
                            planes[i].Draw(Device, oit_pass2);
                        }
                        Device.SetRenderTarget(null);

                        batch.Begin();
                        batch.Draw(backdrop, new Rectangle(0, 0, Device.Viewport.Width, Device.Viewport.Height), Color.White);
                        batch.End();

                        // third pass . compositing
                        oit_pass3.Parameters["WeightTexture"]?.SetValue(oit_weight_rt);
                        batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, null, oit_pass3, null);
                        batch.Draw(oit_colour_rt, Vector2.Zero, Color.White);
                        batch.End();

                        batch.Begin();
                        Vector2 tp = font.MeasureString("Order independant transparency");
                        tp = (tp * -0.5f) + new Vector2(1920 / 2, 800);
                        batch.DrawString(font, "Order independant transparency", tp, Color.White);
                        batch.End();
                    }
                    break;
            }

            batch.Begin();

            if (LastButton.Contains(oldms.X,oldms.Y))
                batch.FillRectangle(LastButton, Color.LightSteelBlue);
            else
                batch.FillRectangle(LastButton, Color.DarkSlateBlue);
            batch.DrawRectangle(LastButton, Color.White);
            batch.DrawString(font, "Last", new Vector2(LastButton.X + 25, LastButton.Y + 3), Color.White);

            if (NextButton.Contains(oldms.X, oldms.Y))
                batch.FillRectangle(NextButton, Color.LightSteelBlue);
            else
                batch.FillRectangle(NextButton, Color.DarkSlateBlue);
            batch.DrawRectangle(NextButton, Color.White);
            batch.DrawString(font, "Next", new Vector2(NextButton.X + 25, NextButton.Y + 3), Color.White);

            batch.End();

        }

        public override void Unload()
        {
            backdrop.Dispose();
            batch.Dispose();
        }
    }
}

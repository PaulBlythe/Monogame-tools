using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LibraryDemos.DemoHelpers
{
    public class QuadPlane
    {
        VertexPositionColor[] verts = new VertexPositionColor[6];

        public QuadPlane(Vector3 centre, float Size, Vector3 Up, Vector3 Right, Color colour)
        {
            Vector3 xs = Right * Size * 0.5f;
            Vector3 ys = Up * Size * 0.5f;

            verts[0] = new VertexPositionColor(centre - xs + ys, colour);
            verts[1] = new VertexPositionColor(centre + xs + ys, colour);
            verts[2] = new VertexPositionColor(centre + xs - ys, colour);

            verts[3] = new VertexPositionColor(centre - xs + ys, colour);
            verts[4] = new VertexPositionColor(centre - xs - ys, colour);
            verts[5] = new VertexPositionColor(centre + xs - ys, colour);
        }

        public void Draw(GraphicsDevice device, Effect fx)
        {
            foreach (EffectPass pass in fx.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, verts, 0, 2);
            }
        }
    }
}

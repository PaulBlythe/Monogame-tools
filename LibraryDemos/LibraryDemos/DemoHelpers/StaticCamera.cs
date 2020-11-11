using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LibraryDemos.DemoHelpers
{
    public class StaticCamera
    {
        public Matrix View;
        public Matrix Projection;

        public StaticCamera(Vector3 pos, Vector3 tar, float aspect, float fov,  float near, float far)
        {
            View = Matrix.CreateLookAt(pos, tar, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(fov, aspect, near, far);
        }
    }
}

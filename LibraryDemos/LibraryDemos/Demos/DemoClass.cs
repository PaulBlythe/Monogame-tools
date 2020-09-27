using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace LibraryDemos.Demos
{
    public abstract class DemoClass
    {
        public abstract void Initialise(GraphicsDevice device, ContentManager content);
        public abstract void Update(float dt);
        public abstract void Draw();
        public abstract void Unload();


    }
}

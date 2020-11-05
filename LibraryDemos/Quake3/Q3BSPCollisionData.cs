using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quake3
{
    public class Q3BSPCollisionData
    {
        public float ratio;
        public Vector3 collisionPoint;
        public bool startOutside;
        public bool inSolid;
        public Vector3 startPosition;
        public Vector3 endPosition;
    }
}

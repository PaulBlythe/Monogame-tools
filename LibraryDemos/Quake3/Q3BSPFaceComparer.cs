using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace Quake3
{
    public class Q3BSPFaceComparer : IComparer
    {
        #region IComparer Members
        int IComparer.Compare(object x, object y)
        {
            Q3BSPFace face1 = (Q3BSPFace)x;
            Q3BSPFace face2 = (Q3BSPFace)y;

            return (face1.TextureIndex - face2.TextureIndex);
        }
        #endregion
    }
}

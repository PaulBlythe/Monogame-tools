using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quake3
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Q3BSPModel
    {
        BoundingBox modelBounds;    // Bounds
        int startFace;              // Starting face index for this model
        int faceCount;              // Number of faces
        int startBrush;             // Starting brush index
        int brushCount;             // Number of brushes

        public Q3BSPModel(BoundingBox bounds, int face, int faces, int brush, int brushes)
        {
            modelBounds = bounds;
            startFace = face;
            faceCount = faces;
            startBrush = brush;
            brushCount = brushes;
        }

        public static Q3BSPModel FromStream(BinaryReader br)
        {
            Vector3 bmin = new Vector3();
            bmin.X = br.ReadSingle() / Q3BSPConstants.scale;
            bmin.Z = -br.ReadSingle() / Q3BSPConstants.scale;
            bmin.Y = br.ReadSingle() / Q3BSPConstants.scale;

            Vector3 bmax = new Vector3();
            bmax.X = br.ReadSingle() / Q3BSPConstants.scale;
            bmax.Z = -br.ReadSingle() / Q3BSPConstants.scale;
            bmax.Y = br.ReadSingle() / Q3BSPConstants.scale;

            int face = br.ReadInt32();
            int nFaces = br.ReadInt32();
            int brush = br.ReadInt32();
            int nBrushes = br.ReadInt32();

            return new Q3BSPModel(
                new BoundingBox(bmin, bmax),
                face,
                nFaces,
                brush,
                nBrushes);
        }

        #region Properties
        public BoundingBox Bounds
        {
            get { return modelBounds; }
            set { modelBounds = value; }
        }

        public int StartFace
        {
            get { return startFace; }
            set { startFace = value; }
        }

        public int FaceCount
        {
            get { return faceCount; }
            set { faceCount = value; }
        }

        public int StartBrush
        {
            get { return startBrush; }
            set { startBrush = value; }
        }

        public int BrushCount
        {
            get { return brushCount; }
            set { brushCount = value; }
        }

        public static int SizeInBytes
        {
            get
            {
                return sizeof(float) * 3 * 2 + sizeof(int) * 4;
            }
        }
        #endregion
    }
}

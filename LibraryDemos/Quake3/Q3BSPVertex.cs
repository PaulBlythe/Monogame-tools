using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quake3
{
    [Serializable]
    public struct Q3BSPVertex : IVertexType
    {
        private Vector3 position;       // Position
        private Vector3 normal;         // Normal
        private Vector2 textureCoord;   // Diffuse Texture coord
        private Vector2 lightMapCoord;  // Light map coord
        private Color vertexColor;      // Vertex color

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(sizeof(float) * 10, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

        public Q3BSPVertex(Vector3 position, Vector3 normal, Vector2 textureCoord, Vector2 lightMapCoord, Color vertexColor)
        {
            this.position = position;
            this.normal = normal;
            this.textureCoord = textureCoord;
            this.lightMapCoord = lightMapCoord;
            this.vertexColor = vertexColor;
        }

        #region Operators
        public static bool operator !=(Q3BSPVertex left, Q3BSPVertex right)
        {
            return left.GetHashCode() != right.GetHashCode();
        }

        public static bool operator ==(Q3BSPVertex left, Q3BSPVertex right)
        {
            return left.GetHashCode() == right.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this == (Q3BSPVertex)obj;
        }

        // Used for patch tesselation only
        public static Q3BSPVertex operator +(Q3BSPVertex left, Q3BSPVertex right)
        {
            return new Q3BSPVertex(
                left.position + right.position,
                left.normal + right.normal,
                left.textureCoord + right.textureCoord,
                left.lightMapCoord + right.lightMapCoord,
                new Color((left.vertexColor.ToVector4() + right.vertexColor.ToVector4())));
        }

        // Used for patch tesselation only
        public static Q3BSPVertex operator *(Q3BSPVertex left, float right)
        {
            return new Q3BSPVertex(
                left.position * right,
                left.normal * right,
                left.textureCoord * right,
                left.lightMapCoord * right,
                new Color(left.vertexColor.ToVector4() * right));
        }

        #endregion 

        #region Properties
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        public Vector2 TextureCoord
        {
            get { return textureCoord; }
            set { textureCoord = value; }
        }

        public Vector2 LightMapCoord
        {
            get { return lightMapCoord; }
            set { lightMapCoord = value; }
        }

        public Color Color
        {
            get { return vertexColor; }
            set { vertexColor = value; }
        }

        public static int SizeInBytes
        {
            get { return sizeof(float) * 10 + 4; }
        }
        #endregion 

        public override int GetHashCode()
        {
            return position.GetHashCode() |
                normal.GetHashCode() |
                textureCoord.GetHashCode() |
                lightMapCoord.GetHashCode() |
                vertexColor.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(
                "<{0}><{1}><{2}><{3}><{4}>",
                position.ToString(),
                normal.ToString(),
                textureCoord.ToString(),
                lightMapCoord.ToString(),
                vertexColor.ToString());
        }

        public static Q3BSPVertex FromStream(BinaryReader br)
        {
            Vector3 p = new Vector3();
            p.X = br.ReadSingle() / Q3BSPConstants.scale;
            p.Z = -br.ReadSingle() / Q3BSPConstants.scale;
            p.Y = br.ReadSingle() / Q3BSPConstants.scale;

            Vector2 tc = new Vector2();
            tc.X = br.ReadSingle();
            tc.Y = br.ReadSingle();

            Vector2 lc = new Vector2();
            lc.X = br.ReadSingle();
            lc.Y = br.ReadSingle();

            Vector3 n = new Vector3();
            n.X = br.ReadSingle();
            n.Z = -br.ReadSingle();
            n.Y = br.ReadSingle();

            byte[] vc = new byte[4];
            vc[0] = br.ReadByte();
            vc[1] = br.ReadByte();
            vc[2] = br.ReadByte();
            vc[3] = br.ReadByte();

            return new Q3BSPVertex(
                p,
                n,
                tc,
                lc,
                new Color(vc[0], vc[1], vc[2], vc[3]));
        }
    }
}

using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quake3
{
    public class Q3BSPEntityManager
    {
        Q3BSPEntity[] entities;

        public bool LoadEntities(string entityString)
        {
            Regex rx = new Regex("{([^}]*)}", RegexOptions.Compiled | RegexOptions.Multiline);
            MatchCollection matches = rx.Matches(entityString);

            if (0 < matches.Count)
            {
                entities = new Q3BSPEntity[matches.Count];
                for (int i = 0; i < matches.Count; i++)
                {
                    entities[i] = new Q3BSPEntity();
                    entities[i].ParseString(matches[i].Groups[1].Value);
                }
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            if (null == entities)
            {
                return "null";
            }

            string str = "Entity Count: " + entities.Length.ToString();
            foreach (Q3BSPEntity e in entities)
            {
                str += "\r\n" + e.ToString();
            }

            return str;
        }

    }
}

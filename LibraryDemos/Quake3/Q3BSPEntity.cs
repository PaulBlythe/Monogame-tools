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
    public class Q3BSPEntity
    {
        Hashtable entries;
        string className;

        public Q3BSPEntity()
        {
            entries = new Hashtable();
            className = "none";
        }

        public string this[string key]
        {
            get { return (string)entries[key]; }
            set { entries[key] = value; }
        }

        public Hashtable Entries
        {
            get { return entries; }
            set { entries = value; }
        }

        public void ParseString(string inputString)
        {
            string[] lines = inputString.Split(new char[] { '\n' });
            Regex rx = new Regex("\"([^\"]*)\"", RegexOptions.Compiled);

            foreach (string oneLine in lines)
            {
                MatchCollection matches;

                string str = oneLine.Trim();
                matches = rx.Matches(str);
                if (1 < matches.Count)
                {
                    if ("classname" == matches[0].Groups[1].Value)
                    {
                        className = matches[1].Groups[1].Value;
                    }
                    else
                    {
                        entries[matches[0].Groups[1].Value] = matches[1].Groups[1].Value;
                    }
                }
            }
        }

        public override string ToString()
        {
            return "classname=" + className;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Quake3
{
    [StructLayout(LayoutKind.Sequential)]
    struct Q3BSPDirEntry
    {
        int offset;
        int length;

        public Q3BSPDirEntry(int o, int l)
        {
            offset = o;
            length = l;
        }

        public int Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public int Length
        {
            get { return length; }
            set { length = value; }
        }
    }
}

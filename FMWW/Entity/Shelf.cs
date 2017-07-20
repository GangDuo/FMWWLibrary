using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FMWW.Entity
{
    public class Shelf
    {
        public string Code1;
        public string Code2;
        public string Code3;

        public string Name1;
        public string Name2;
        public string Name3;

        public Shelf(IList<string> fields)
        {
            Debug.Assert(fields.Count == 6);
            Code1 = fields[0];
            Name1 = fields[1];
            Code2 = fields[2];
            Name2 = fields[3];
            Code3 = fields[4];
            Name3 = fields[5];
        }
    }
}

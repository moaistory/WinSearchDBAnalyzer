using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSearchDBAnalyzer.EDBParser
{
    public class Column
    {
        private int id;
        private int type;
        private int spaceUsage;
        private string name;
        private bool ignore;
        public Column() { }

        public Column(int id, int type, int spaceUsage, string name)
        {
            this.id = id;
            this.type = type;
            this.spaceUsage = spaceUsage;
            this.name = name;
            this.ignore = false;
        }


        public int getID()
        {
            return this.id;
        }

        public int getType()
        {
            return this.type;
        }

        public int getSpaceUsage()
        {
            return this.spaceUsage;
        }

        public string getName()
        {
            return this.name;
        }

        public void setIgnore(bool ignore)
        {
            this.ignore = ignore;
        }

        public bool getIgnore() 
        {
            return this.ignore;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSearchDBAnalyzer.EDBParser
{
    public class DatabaseHeader
    {
        private HexReader hexReader;
        private int signature;
        private int version;
        private int status;
        private int revision;
        private int pagesize;

        public DatabaseHeader()
        {
        }


        public DatabaseHeader(HexReader hexReader)
        {
            this.hexReader = hexReader;
        }



        /***** private method ****/


        /***** public method *****/

        public void parseHeaderArea()
        {
            this.signature = this.hexReader.readInt(0x04);
            this.version = this.hexReader.readInt(0x08);
            this.status = this.hexReader.readInt(0x34);
            this.revision = this.hexReader.readInt(0xE8);
            this.pagesize = this.hexReader.readInt(0xEC);
        }
        public void setHexReader(HexReader hexReader)
        {
            this.hexReader = hexReader;
        }


        public int getSignature()
        {
            return this.signature;
        }

        public int getVersion()
        {
            return this.version;
        }

        public int getStatus()
        {
            return this.status;
        }

        public int getRevision()
        {
            return this.revision;
        }

        public int getPagesize()
        {
            return this.pagesize;
        }
    }
}

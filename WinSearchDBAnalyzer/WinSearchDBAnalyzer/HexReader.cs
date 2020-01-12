using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace WinSearchDBAnalyzer
{
    public class HexReader
    {
        String inputPath;
        FileStream fstream = null;
        BinaryReader br;
        public HexReader(String inputPath)
        {
            this.inputPath = inputPath;
            this.fstream = new FileStream(inputPath, FileMode.Open, FileAccess.Read,
                                 FileShare.ReadWrite);
            this.br = new BinaryReader(fstream);
        }

        public void close()
        {
            if (this.fstream != null)
            {
                this.fstream.Close();
                this.fstream = null;
            }
        }

        public long getFileSize()
        {
            return fstream.Length;
        }

        public byte[] readByteDump(int offset, int len)
        {

            fstream.Seek(offset, SeekOrigin.Begin);
            //br.BaseStream.Seek(offset, );
            return br.ReadBytes(len);
        }

        public String readString(int offset, int len)
        {
            byte[] bytes = readByteDump(offset, len);
            string str = Encoding.Default.GetString(bytes);
            return str;
        }

        public ushort readUshort(int offset)
        {
            byte[] bytes = readByteDump(offset, 2);
            ushort s = BitConverter.ToUInt16(bytes, 0);
            return s;
        }

        public short readShort(int offset)
        {
            byte[] bytes = readByteDump(offset, 2);
            short s = BitConverter.ToInt16(bytes, 0);
            return s;
        }

        public uint readUint(int offset)
        {
            byte[] bytes = readByteDump(offset, 4);
            uint i = BitConverter.ToUInt32(bytes, 0);
            return i;
        }

        public int readInt(int offset)
        {
            byte[] bytes = readByteDump(offset, 4);
            int i = BitConverter.ToInt32(bytes, 0);
            return i;
        }

        public int readIntBigEndian(int offset)
        {
            byte[] bytes = readByteDump(offset, 4);
            Array.Reverse(bytes);
            int i = BitConverter.ToInt32(bytes, 0);
            return i;
        }

        public ulong readUlong(int offset)
        {
            byte[] bytes = readByteDump(offset, 8);
            ulong l = BitConverter.ToUInt64(bytes, 0);
            return l;
        }

        public long readLong(int offset)
        {
            byte[] bytes = readByteDump(offset, 8);
            long l = BitConverter.ToInt64(bytes, 0);
            return l;
        }

    }
}

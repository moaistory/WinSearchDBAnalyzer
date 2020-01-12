using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinSearchDBAnalyzer.EDBParser
{
    public class Item
    {
        private int offset;
        private int size;
        private int id;
        private int maxLength;
        private string columnName;
        private ColType.TYPE type;
        private byte taggedDataItemFlag;
        private HexReader hexReader;
        private string UTCAddHour;
        private string UTCAddMinute;
        private bool isWin10;
        public Item() { }


        public Item(HexReader hexReader, int id, string columnName, ColType.TYPE type, int maxLength, int offset, int size)
        {
            this.hexReader = hexReader;
            this.id = id;
            this.columnName = columnName;
            this.offset = offset;
            this.size = size;
            this.maxLength = maxLength;
            this.type = type;
            if (this.id >= 0x100)
            {
                this.taggedDataItemFlag = hexReader.readByteDump(offset, 1)[0];
                this.offset++;
                this.size--;
            }
            else
            {
                this.taggedDataItemFlag = 0;
            }
            this.isWin10 = true;
        }

        public int getOffset()
        {
            return offset;
        }
        public void setWin7()
        {
            this.isWin10 = false;
        }

        public void setColumnName(string columnName)
        {
            this.columnName = columnName;
        }

        public void setColumnType(ColType.TYPE type)
        {
            this.type = type;
        }

        public void UTCTime(string UTCAddHour, string UTCAddMinute)
        {
            this.UTCAddHour = UTCAddHour;
            this.UTCAddMinute = UTCAddMinute;
        }

        public int getID()
        {
            return this.id;
        }

        public ColType.TYPE getType()
        {
            return this.type;
        }


        public byte getTaggedDataItemFlag()
        {
            return this.taggedDataItemFlag;
        }

        public int getSize()
        {
            return this.size;
        }

        public void setIdType(int id, ColType.TYPE type)
        {
            this.id = id;
            this.type = type;
        }

        public int getPointNumber()
        {
            Byte[] data = hexReader.readByteDump(offset, 4);
            return BitConverter.ToInt32(data, 0);
        }

        public Byte[] changeEndian(Byte[] data)
        {
            for(int i =0; i< data.Length/2; i++)
            {
                Byte b = data[i];
                data[i] = data[data.Length-1-i];
                data[data.Length - i - 1] = b;                
            }
            return data;
        }
        public string getValue()
        {
            try
            {
                Byte[] data = hexReader.readByteDump(offset, size);
                if (columnName.Contains("Date") || columnName == "LastModified" || columnName.Contains("System_Search_GatherTime") || columnName.Contains("ActivityHistory_StartTime") || columnName.Contains("ActivityHistory_EndTime"))
                {
                    try
                    {
                        if(columnName == "LastModified" || isWin10 == false)
                            data = changeEndian(data);
                        long timeValue = (long)BitConverter.ToUInt64(data, 0);
                        if (timeValue == 0)
                        {
                            return "0";
                        }
                        DateTime datetime = DateTime.FromBinary(timeValue);
                        datetime = datetime.AddYears(1600);
                        datetime = datetime.AddHours(Int32.Parse(UTCAddHour));
                        datetime = datetime.AddMinutes(Int32.Parse(UTCAddMinute));
                        return datetime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    catch
                    {
                        return "0";
                    }
                }

                if (columnName.Contains("System_Size"))
                {
                    if (data[0] == 0x2A)
                    {
                        return "";
                    }
                    else
                    {
                        if(!isWin10)
                            data = changeEndian(data);
                        return BitConverter.ToUInt64(data, 0).ToString();
                    }
                }

                if (columnName.Contains("System_Search_AutoSummary"))
                {
                    if (!isWin10)
                    {
                        return getDeobfuscation();
                    }
                }


                if (this.type == ColType.TYPE.JET_coltypNil) //bool
                {
                    return "NULL";
                }
                else if (this.type == ColType.TYPE.JET_coltypBit) //bool
                {
                    return data[0] == 0 ? "FALSE" : "TRUE";
                }
                else if (this.type == ColType.TYPE.JET_coltypUnsignedByte) //byte
                {
                    return data[0].ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypShort) //short
                {
                    return BitConverter.ToInt16(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypLong) //integer
                {
                    return BitConverter.ToInt32(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypCurrency) //binary
                {
                    double temp = (double)BitConverter.ToInt64(data, 0) / 1E4;
                    return temp.ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypIEEESingle) //float
                {
                    return BitConverter.ToSingle(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypIEEEDouble) //double
                {
                    return BitConverter.ToDouble(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypDateTime) //dateTime
                {
                    long longVar = BitConverter.ToInt64(data, 0);
                    DateTime dateTimeVar = new DateTime(1980, 1, 1).AddMilliseconds(longVar);
                    dateTimeVar.AddHours((Double)Int32.Parse(UTCAddHour));
                    dateTimeVar.AddMinutes((Double)Int32.Parse(UTCAddMinute));
                    return dateTimeVar.ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypBinary) //binary
                {
                    string hex = BitConverter.ToString(data);
                    return hex.Replace("-", "");
                }
                else if (this.type == ColType.TYPE.JET_coltypText) //text
                {
                    if (maxLength == 255)
                    {
                        return Encoding.UTF8.GetString(data);
                    }
                    else
                    {
                        return Encoding.Unicode.GetString(data);
                    }
                }
                else if (this.type == ColType.TYPE.JET_coltypLongBinary) //binary
                {
                    string hex = BitConverter.ToString(data);
                    return hex.Replace("-", "");
                }
                else if (this.type == ColType.TYPE.JET_coltypLongText) //text
                {
                    return Encoding.Unicode.GetString(data);
                }
                else if (this.type == ColType.TYPE.JET_coltypSLV) //integer
                {
                    return BitConverter.ToInt32(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypUnsignedLong) //unsigned integer
                {
                    return BitConverter.ToUInt32(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypLongLong) //unsigned long long
                {
                    return BitConverter.ToUInt64(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypGUID) //text
                {
                    return Encoding.UTF8.GetString(data);
                }
                else if (this.type == ColType.TYPE.JET_coltypUnsignedShort) //unsigned short
                {
                    return BitConverter.ToInt16(data, 0).ToString();
                }
                else
                {
                    return "UNKNOWN";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message.ToString() + "\r\noffset : " + offset, "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Parsing Error : (Offset : " + offset + ")";
            }
        }

        private string ByteToBitStream(byte value){
            string buffer = "";
            buffer += (value & 0x80)!= 0  ? '1':'0';
            buffer += (value & 0x40)!= 0  ? '1':'0';
            buffer += (value & 0x20)!= 0  ? '1':'0';
            buffer += (value & 0x10)!= 0  ? '1':'0';
            buffer += (value & 0x08)!= 0  ? '1':'0';
            buffer += (value & 0x04)!= 0  ? '1':'0';
            buffer += (value & 0x02)!= 0  ? '1':'0';
            buffer += (value & 0x01)!= 0  ? '1':'0';
            return buffer;
        }
        private byte BitStreamToByte(string value){
            int toShift = 0;
            char[] buffer = value.ToCharArray();
	        int size = value.Length;
	        for(int i = 0; i < size; i++) {
		        toShift = (toShift << 1);
                if(buffer[i] == '1') {    
			        toShift++;
                }
            }
            return (byte)toShift;
        }

        public string getColumnName()
        {
            return this.columnName;
        }

        public string getDecompressText()
        {
            int firstByte = this.hexReader.readByteDump(offset, 1)[0];
            if (firstByte == 0x18)
            {
                int size = this.hexReader.readShort(offset+1);
                var bytes = hexReader.readByteDump(offset + 3, this.size - 3);
                byte[] dec_bytes = Xpress_Decompress(bytes, size);
                int checkSize = size;
                if (checkSize > dec_bytes.Length)
                    checkSize = dec_bytes.Length;
                bool unicodeCheck = false;
                for (int i = 0; i < checkSize; i++)
                {
                    if (i == 100) break;
                    if (dec_bytes[i] == 0)
                    {
                        unicodeCheck = true;
                    }
                }
                if (unicodeCheck)
                {
                    return Encoding.Unicode.GetString(dec_bytes.ToArray());
                }
                else
                {
                    return Encoding.UTF8.GetString(dec_bytes.ToArray());
                }
            }


            List<byte> data = new List<byte>();
            string temp1 = "";
            string temp2 = "";

            byte byteValue = 0;
            for (int i = 1; i < this.size; i++)
            {
                byteValue = this.hexReader.readByteDump(offset + i,1)[0];
                temp2 = ByteToBitStream(byteValue);
                for (int j = temp2.Length; j < 8; j++)
                {
                    temp2 = '0' + temp2;
                }
                temp1 = temp2 + temp1;
                while (temp1.Length >= 7)
                {
                    
                    temp2 = temp1.Substring(temp1.Length - 7);
                    data.Add(BitStreamToByte(temp2));
                    temp1 = temp1.Substring(0, temp1.Length - 7);
                }
            }
            return Encoding.UTF8.GetString(data.ToArray());
        }

        public static byte[] Xpress_Decompress(byte[] inputBuffer, int outputSize)
        {
            int outputIndex, inputIndex;
            int indicator, indicatorBit;
            int length;
            int offset;
            int nibbleIndex;
            int nibbleIndicator;
            int inputSize = inputBuffer.Length;

            outputIndex = 0;
            inputIndex = 0;
            indicator = 0;
            indicatorBit = 0;
            length = 0;
            offset = 0;
            nibbleIndex = 0;
            nibbleIndicator = 0x19880922;

            var outputBuffer = new byte[outputSize];
            while ((outputIndex < outputSize) && (inputIndex < inputSize))
            {
                if (indicatorBit == 0)
                {
                    if (inputIndex + 3 >= inputSize) goto Done;
                    indicator = GetInt(inputBuffer, inputIndex);
                    inputIndex += sizeof(int);
                    indicatorBit = 32;
                }
                indicatorBit--;

                //* check whether the bit specified by IndicatorBit is set or not
                //* set in Indicator. For example, if IndicatorBit has value 4
                //* check whether the 4th bit of the value in Indicator is set

                if (((indicator >> indicatorBit) & 1) == 0)
                {
                    if (outputIndex >= outputSize) 
                        goto Done;
                    outputBuffer[outputIndex] = inputBuffer[inputIndex];
                    inputIndex += sizeof(byte);
                    outputIndex += sizeof(byte);
                }
                else
                {
                    if (inputIndex + 1 >= inputSize) 
                        goto Done;
                    length = (inputBuffer[inputIndex + 1] << 8) | inputBuffer[inputIndex];

                    /*
                    if ((OutputIndex > 0xD0) && (OutputIndex < 0xF0))
                    {
                        printf("DECOMP: READ AT [0x%08X] = %04X \n", InputIndex, Length);
                    }
                    */
                    inputIndex += sizeof(ushort);
                    offset = length / 8;
                    length = length % 8;
                    //if ((OutputIndex > 0xD0) && (OutputIndex < 0xF0)) printf("--1 Len: %02X (%d)\n", Length, Length);
                    if (length == 7)
                    {
                        if (nibbleIndex == 0)
                        {
                            nibbleIndex = inputIndex;
                            if (inputIndex >= inputSize) 
                                goto Done;
                            length = inputBuffer[inputIndex] % 16;
                            //if ((OutputIndex > 0xD0) && (OutputIndex < 0xF0)) printf("--2 Len: %02X (%d)\n", Length, Length);
                            inputIndex += sizeof(byte);
                        }
                        else
                        {
                            length = inputBuffer[nibbleIndex] / 16;
                            //if ((OutputIndex > 0xD0) && (OutputIndex < 0xF0)) printf("--3 Len: %02X (%d)\n", Length, Length);
                            nibbleIndex = 0;
                        }

                        if (length == 15)
                        {
                            if (inputIndex >= inputSize) goto Done;
                            length = inputBuffer[inputIndex];
                            //if ((OutputIndex > 0xD0) && (OutputIndex < 0xF0)) printf("--4 Len: %02X (%d)\n", Length, Length);
                            inputIndex += sizeof(byte);
                            if (length == 255)
                            {
                                if (inputIndex + 2 >= inputSize) goto Done;
                                length = (inputBuffer[inputIndex + 1] << 8) | inputBuffer[inputIndex];
                                inputIndex += sizeof(ushort);
                                length -= (15 + 7);
                            }
                            length += 15;
                            //if ((OutputIndex > 0xD0) && (OutputIndex < 0xF0)) printf("--5 Len: %02X (%d)\n", Length, Length);
                        }
                        length += 7;
                        //if ((OutputIndex > 0xD0) && (OutputIndex < 0xF0)) printf("--6 Len: %02X (%d)\n", Length, Length);
                    }

                    length += 3;
                    //if ((OutputIndex > 0xD0) && (OutputIndex < 0xF0)) printf("--7 Len: %02X (%d)\n", Length, Length);
                    //if (Length > 280) printf("DECOMP DEBUG: [0x%08X]->[0x%08X] Len: %d Offset: %08X\n",
                    //    OutputIndex, InputIndex, Length, Offset);
                    while (length != 0)
                    {
                        if ((outputIndex >= outputSize) || ((offset + 1) >= outputIndex)) break;
                        outputBuffer[outputIndex] = outputBuffer[outputIndex - offset - 1];
                        outputIndex += sizeof(byte);
                        length -= sizeof(byte);
                    }
                }

            }

            Done:

            if (outputIndex < outputBuffer.Length)
            {
                var buffer = new byte[outputIndex];
                Buffer.BlockCopy(outputBuffer, 0, buffer, 0, outputIndex);
                outputBuffer = buffer;
            }


            return outputBuffer;
        }

        public static int GetInt(byte[] buffer, int offset)
        {
            return
                ((int)buffer[offset]) |
                ((int)buffer[offset + 1] << 8) |
                ((int)buffer[offset + 2] << 16) |
                ((int)buffer[offset + 3] << 24);
        }

        public string getDeobfuscation()
        {
            Byte[] data = hexReader.readByteDump(offset, size);
            try
            {         
                int data_iterator = 0;
                UInt32 bitmask32 = 0x05000113;
                bitmask32 ^= (UInt32)this.size;
                Byte bitmask;
                for (int i = 0; i < this.size; i++)
                {
                    switch( i & 0x03 )
                    {
                        case 3:
                            bitmask = (Byte)((bitmask32 >> 24) & 0xff);
                            break;
                        case 2:
                            bitmask = (Byte)((bitmask32 >> 16) & 0xff);
                            break;
                        case 1:
                            bitmask = (Byte)((bitmask32 >> 8) & 0xff);
                            break;
                        default:
                            bitmask = (Byte)(bitmask32 & 0xff);
                            break;
                    }
                    bitmask = (Byte) (bitmask ^ (i & 0xFF));
                    data[data_iterator++] = (Byte) (data[i] ^ bitmask);
                }
            
                if (data[0] == 4)
                {
                    return Encoding.Unicode.GetString(data.Skip(1).ToArray());
                }
                else if (data[0] == 1)
                {
                    return Encoding.UTF8.GetString(data.Skip(1).ToArray());
                }

                else if (data[0] == 0 || data[0] == 2)
                {
                    byte[] parseData = data;
                    int parseSize = this.size;
                    int pos = 1;
                    if (data[0] == 2)
                    {
                        parseData = decompress_3(data.Skip(1).ToArray());
                        parseSize = parseData.Length;
                        pos = 0;
                    }
                
                    string result = "";
                    while (pos < parseSize)
                    {
                        if (parseData[pos] == 0x01)
                        {
                            //unicode
                            if (pos + 2 < parseSize)
                            {
                                byte[] unicodeBytes = { parseData[pos + 2], parseData[pos + 1] };
                                result += Encoding.Unicode.GetString(unicodeBytes, 0, 2);
                            }
                            pos += 3;
                        }
                        else
                        {
                            int compressSize = parseData[pos];
                            if (compressSize == 0)
                            {
                                break;
                            }
                            //unicode
                            if (pos + 1 + compressSize < parseSize)
                            {
                                byte compressByte = parseData[pos + 1];
                                pos += 2;
                                for (int j = 0; j < compressSize; j++)
                                {
                                    if (pos + j < parseSize)
                                    {
                                        byte[] unicodeBytes = { parseData[pos + j], compressByte };
                                        result += Encoding.Unicode.GetString(unicodeBytes, 0, 2);
                                    }
                                }
                            }
                            pos += compressSize;
                       
                        }
                    }
                    return result;
                }
                else
                {
                    if (data[0] == 3 || data[0] == 6)
                    {
                            byte[] dec_bytes = decompress_3(data.Skip(1).ToArray());
                            bool unicodeCheck = false;
                            for (int i = 0; i < dec_bytes.Length; i++)
                            {
                                if (i == 100) break;
                                if (dec_bytes[i] == 0)
                                {
                                    unicodeCheck = true;
                                }
                            }
                            try
                            {
                                if (unicodeCheck)
                                {
                                    return Encoding.Unicode.GetString(dec_bytes.ToArray());
                                }
                                else
                                {
                                    return Encoding.UTF8.GetString(dec_bytes.ToArray());
                                }
                            }
                            catch (Exception e)
                            {
                                return "[Encoding Error]" + Environment.NewLine + BitConverter.ToString(dec_bytes.ToArray()).Replace("-", "");
                            }
                    }
                
                    return BitConverter.ToString(data.ToArray()).Replace("-", "");
                }
            }
            catch (Exception e)
            {
                return "[Decompression Error]" + Environment.NewLine + BitConverter.ToString(data.ToArray()).Replace("-", "");
            }
        }

        public byte[] decompress_3(Byte[] compressed_data)
        {

	        ushort[] compression_value_table = new ushort[ 2048 ];
	        uint[] nibble_count_table = new uint[ 16 ]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	        uint[] total_nibble_count_table = new uint[ 16 ]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	        int compressed_data_iterator         = 0;
	        int compression_iterator             = 0;
	        int uncompressed_data_iterator       = 0;

	        uint compressed_data_bit_stream     = 0;
	        uint compression_offset             = 0;
	        uint nibble_count                   = 0;
	        uint total_nibble_count             = 0;
	        uint value_32bit                    = 0;
	        int compression_value_table_index   = 0;
	        ushort compression_size               = 0;
	        ushort compression_value              = 0;
	        ushort stored_uncompressed_data_size  = 0;
	        ushort value_0x0400                   = 0;
	        ushort value_0x0800                   = 0;
	        ushort value_0x2000                   = 0;
	        byte nibble_count_table_index        = 0;
	        sbyte number_of_bits_available         = 0;
	        sbyte number_of_bits_used              = 0;


            int compressed_data_size = compressed_data.Length;
            int uncompressed_data_size = this.hexReader.readShort(offset+1);
            //byte[] uncompressed_data = new byte[uncompressed_data_size];
            byte[] uncompressed_data = new byte[10000];
	        /* Byte 2 - 257 contain the compression table
	         *
	         * The table contains a compression value for every byte
	         * bits 0 - 3 contain ???
	         * bits 4 - 7 contain the number of bits used to store the compressed data
	         */
	        for( compressed_data_iterator = 0;
	             compressed_data_iterator < 256;
	             compressed_data_iterator++ )
	        {
		        nibble_count_table_index = compressed_data[ 2 + compressed_data_iterator ];

		        nibble_count_table[ nibble_count_table_index & 0x0f ] += 1;
		        nibble_count_table[ nibble_count_table_index >> 4 ]   += 1;
	        }

	        /* Make copy of the nibble count table
	         */
	        for( nibble_count_table_index = 0;
	             nibble_count_table_index < 16;
	             nibble_count_table_index++ )
	        {
		        total_nibble_count_table[ nibble_count_table_index ] = nibble_count_table[ nibble_count_table_index ];
	        }
	        nibble_count = 0;

	        for( nibble_count_table_index = 15;
	             nibble_count_table_index > 0;
	             nibble_count_table_index-- )
	        {
		        nibble_count += total_nibble_count_table[ nibble_count_table_index ];

		        if( nibble_count == 1 )
		        {
			        break;
		        }
		        nibble_count >>= 1;
	        }
	        /* Determine the total nible counts
	         */
	        nibble_count = 0;
	        for( nibble_count_table_index = 1;
	             nibble_count_table_index < 16;
	             nibble_count_table_index++ )
	        {
		        total_nibble_count_table[ nibble_count_table_index ] += nibble_count;
		        nibble_count                                          = total_nibble_count_table[ nibble_count_table_index ];
	        }
	        total_nibble_count = nibble_count;

	        /* Fill the compression value table
	         */
	        value_0x2000 = 0x2000;

	        while( value_0x2000 > 0 )
	        {
		        value_0x2000 -= 0x10;

		        compressed_data_iterator = value_0x2000 >> 5;

		        nibble_count_table_index = (byte)(compressed_data[ 2 + compressed_data_iterator ] >> 4);

		        if( nibble_count_table_index > 0 )
		        {
			        total_nibble_count_table[ nibble_count_table_index ] -= 1;
			        compression_value_table_index                         =(int) total_nibble_count_table[ nibble_count_table_index ];
			        compression_value_table[ compression_value_table_index ] = (ushort)(value_0x2000 | nibble_count_table_index);
		        }
		        value_0x2000 -= 0x10;

		        compressed_data_iterator = value_0x2000 >> 5;

		        nibble_count_table_index = (byte)(compressed_data[ 2 + compressed_data_iterator ] & 0x0f);

		        if( nibble_count_table_index > 0 )
		        {
			        total_nibble_count_table[ nibble_count_table_index ] -= 1;
			        compression_value_table_index                         = (int)(total_nibble_count_table[ nibble_count_table_index ]);
			        compression_value_table[ compression_value_table_index ] = (ushort)(value_0x2000 | nibble_count_table_index);
		        }
	        }
	        compression_value_table_index = 0x0800;
	        value_0x0800                  = 0x0800;
	        value_0x0400                  = 0x0400;
	        for( nibble_count_table_index = 15;
	             nibble_count_table_index > 10;
	             nibble_count_table_index-- )
	        {
		        if( value_0x0800 > compression_value_table_index )
		        {
			        value_0x0800                  -= 2;
			        compression_value_table_index -= 1;

			        compression_value_table[ compression_value_table_index ] =(ushort)(value_0x0800 | 0x8000);
		        }
		        for( nibble_count = nibble_count_table[ nibble_count_table_index ];
		             nibble_count > 0;
		             nibble_count-- )
		        {
			        total_nibble_count -= 1;

			        compression_value              = compression_value_table[ total_nibble_count ];
			        compression_value_table_index -= 1;

			        compression_value_table[ compression_value_table_index ] = compression_value;
		        }
	        }
	        while( value_0x0800 > compression_value_table_index )
	        {
		        value_0x0800 -= 2;
		        value_0x0400 -= 1;

		        compression_value_table[ value_0x0400 ] = (ushort)(value_0x0800 | 0x8000);
	        }
	        while( total_nibble_count > 0 )
	        {
		        total_nibble_count -= 1;

		        compression_value             = compression_value_table[ total_nibble_count ];
		        compression_value_table_index = value_0x0400 - ( 0x0400 >> ( compression_value & 0x0f ) );

		        do
		        {
			        value_0x0400 -= 1;

			        compression_value_table[ value_0x0400 ] = compression_value;
		        }
		        while( value_0x0400 > compression_value_table_index );
	        }
	        /* Byte 258 - end contain the compression data bit stream
	         */
	        compressed_data_iterator = 2 + 0x100;
	        /* Read the data as 16-bit little endian values
	         */
	        compressed_data_bit_stream   = compressed_data[ compressed_data_iterator + 1 ];
	        compressed_data_bit_stream <<= 8;
	        compressed_data_bit_stream  += compressed_data[ compressed_data_iterator ];
	        compressed_data_bit_stream <<= 8;
	        compressed_data_bit_stream  += compressed_data[ compressed_data_iterator + 3 ];
	        compressed_data_bit_stream <<= 8;
	        compressed_data_bit_stream  += compressed_data[ compressed_data_iterator + 2 ];

	        compressed_data_iterator += 4;

	        number_of_bits_available = 0x10;

	        /* The compression data is stored a 16-bit little-endian values
	         * it contains a bit stream which contains the following values
	         * starting with the first bit in the stream
	         * 0 - 9 compression value table index (where 0 is the MSB of the value)
	         */
	        while( compressed_data_iterator < compressed_data_size )
	        {
		        /* Read a 10-bit table index from the decoded data
		         * maximum index of 1023
		         */
		        compression_value_table_index =(int)(compressed_data_bit_stream >> 0x16);

		        /* Check if the table entry contains an ignore index flag (bit 15)
		         */
		        if( ( compression_value_table[ compression_value_table_index ] & 0x8000 ) != 0 )
		        {
			        /* Ignore the 10-bit index
			         */
			        compressed_data_bit_stream <<= 10;

			        do
			        {
				        compression_value_table_index = compression_value_table[ compression_value_table_index ] & 0x7fff;

				        /* Add the MSB of the compressed data bit stream to the
				         * compression value table index
				         */
				        compression_value_table_index += (int)(compressed_data_bit_stream >> 31);

				        /* Ignore 1 bit for empty compression values
				         */
				        compressed_data_bit_stream <<= 1;
			        }
			        while( compression_value_table[ compression_value_table_index ] == 0 );

			        /* Retrieve the number of bits used (lower 4-bit) of from the table entry
			         */
			        number_of_bits_used = (sbyte) ( compression_value_table[ compression_value_table_index ] & 0x0f );

			        /* Retrieve the compression value from the table entry
			         */
			        compression_value = (ushort)(compression_value_table[ compression_value_table_index ] >> 4);

			        number_of_bits_available -= number_of_bits_used;
		        }
		        else
		        {
			        /* Retrieve the number of bits used (lower 4-bit) of from the table entry
			         */
			        number_of_bits_used = (sbyte) ( compression_value_table[ compression_value_table_index ] & 0x0f );

			        /* Retrieve the compression value from the table entry
			         */
			        compression_value = (ushort)(compression_value_table[ compression_value_table_index ] >> 4);

			        number_of_bits_available    -= number_of_bits_used;
			        compressed_data_bit_stream <<= number_of_bits_used;
		        }
		        if( number_of_bits_available < 0 )
		        {
			        number_of_bits_used = (sbyte) (-1 * number_of_bits_available);

			        /* Read the data as 16-bit little endian values
			         */
			        value_32bit   = compressed_data[ compressed_data_iterator + 1 ];
			        value_32bit <<= 8;
			        value_32bit  += compressed_data[ compressed_data_iterator ];

			        compressed_data_iterator += 2;

			        value_32bit               <<= number_of_bits_used;
			        compressed_data_bit_stream += value_32bit;

			        number_of_bits_available += 0x10;
		        }
		        /* Check if the table entry contains a compression tuple flag (bit 12)
		         */
		        if( ( compression_value_table[ compression_value_table_index ] & 0x1000 ) != 0 )
		        {
			        /* Retrieve the size of the compression (bit 4-7) from the table entry
			         */
			        compression_size = (ushort) ( ( compression_value_table[ compression_value_table_index ] >> 4 ) & 0x0f );

			        /* Retrieve the size of the compression (bit 8-11) from the table entry
			         */
			        number_of_bits_used = (sbyte) ( ( compression_value_table[ compression_value_table_index ] >> 8 ) & 0x0f );

			        /* Break if the end of the compressed data is reached
			         * and both the compression size and number of bits used for the compression offset are 0
			         */
			        if( ( compressed_data_iterator == compressed_data_size )
			         && ( compression_size == 0 )
			         && ( number_of_bits_used == 0 ) )
			        {
				        break;
			        }
			        /* Retrieve the compression offset from the decoded data
			         */
			        compression_offset = ( compressed_data_bit_stream >> 1 ) | 0x80000000;

			        compression_offset = ( compression_offset >> ( 31 - number_of_bits_used ) );

			        compressed_data_bit_stream <<= number_of_bits_used;
			        number_of_bits_available    -= number_of_bits_used;

			        if( compression_size == 0x0f )
			        {
				        compression_size += compressed_data[ compressed_data_iterator ];

				        compressed_data_iterator += 1;
			        }
                    //next homework what is byte_stream_copy_to_int16_little_endian?
			        if( compression_size == ( 0xff + 0x0f ) )
			        {
                        ( compression_size )   = ( compressed_data )[ 1 ];
                        (compression_size) <<= 8;
                        (compression_size) |= (compressed_data)[0];
				        compressed_data_iterator += 2;

			        }
                    
			        compression_size += 3;

			        if( number_of_bits_available < 0 )
			        {
				        number_of_bits_used = (sbyte) (-1 * number_of_bits_available);

				        /* Read the data as 16-bit little endian values
				         */
				        value_32bit   = compressed_data[ compressed_data_iterator + 1 ];
				        value_32bit <<= 8;
				        value_32bit  += compressed_data[ compressed_data_iterator ];

				        compressed_data_iterator += 2;

				        value_32bit               <<= number_of_bits_used;
				        compressed_data_bit_stream += value_32bit;

				        number_of_bits_available += 0x10;
			        }
			        compression_iterator = (int)(uncompressed_data_iterator - compression_offset);

			        while( compression_size > 0 )
			        {
				        uncompressed_data[ uncompressed_data_iterator++ ] = uncompressed_data[ compression_iterator++ ];

				        compression_size--;
			        }
		        }
		        else
		        {
			        uncompressed_data[ uncompressed_data_iterator++ ] = (byte) ( compression_value & 0xff );
		        }
	        }
            return uncompressed_data;
        }
    }
}

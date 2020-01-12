using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSearchDBAnalyzer.EDBParser
{
    public class ColType
    {
        public enum TYPE
        {
            JET_coltypNil = 0,				//null
            JET_coltypBit = 1,				//boolean
            JET_coltypUnsignedByte = 2,		//byte
            JET_coltypShort = 3,			//short
            JET_coltypLong = 4,				//integer
            JET_coltypCurrency = 5,			//Currency (64-bit)
            JET_coltypIEEESingle = 6,		//float
            JET_coltypIEEEDouble = 7,		//double
            JET_coltypDateTime = 8,			//time
            JET_coltypBinary = 9,			//binary
            JET_coltypText = 10,			//text
            JET_coltypLongBinary = 11,		//long binary
            JET_coltypLongText = 12,		//long text
            JET_coltypSLV = 13,				//long value
            JET_coltypUnsignedLong = 14,	//unsigned integer
            JET_coltypLongLong = 15,		//long long
            JET_coltypGUID = 16,			//text
            JET_coltypUnsignedShort = 17,	//unsigned short
            UNKNOWN_TYPE = -1				//unknown type
        };
    }
}

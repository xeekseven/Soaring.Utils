using System;
using System.Collections.Generic;
using System.Text;

namespace Soaring.HttpCS.Util
{
    public class ResponseResult
    {
        public int RESCode { get; set; }
        public string ResponseString { get; set; }
        public string ContentType { get; set; }
    }
}

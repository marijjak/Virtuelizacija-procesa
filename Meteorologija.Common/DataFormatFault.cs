using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace Meteorologija.Common
{
    [DataContract]
    public class DataFormatFault
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Field { get; set; }

        public DataFormatFault(string message, string field)
        {
            Message = message;
            Field = field;
        }
    }
}

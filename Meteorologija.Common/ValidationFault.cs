using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Meteorologija.Common
{
    [DataContract]
    public class ValidationFault
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Field { get; set; }

        [DataMember]
        public double Value { get; set; }

        public ValidationFault(string message, string field, double value)
        {
            Message = message;
            Field = field;
            Value = value;
        }
    }
}
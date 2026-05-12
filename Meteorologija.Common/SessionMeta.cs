using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace Meteorologija.Common
{
    [DataContract]
    public class SessionMeta
    {
        [DataMember]
        public string StationName { get; set; }

        [DataMember]
        public string DatasetPath { get; set; }

        [DataMember]
        public int TotalSamples { get; set; }
    }
}
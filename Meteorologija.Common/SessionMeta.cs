using System.Runtime.Serialization;

namespace Meteorologija.Common
{
    [DataContract]
    public class SessionMeta
    {
        [DataMember]
        public double T { get; set; }

        [DataMember]
        public double Pressure { get; set; }

        [DataMember]
        public double Tpot { get; set; }

        [DataMember]
        public double Tdew { get; set; }

        [DataMember]
        public double Rh { get; set; }

        [DataMember]
        public double Sh { get; set; }

        [DataMember]
        public string Date { get; set; }

        [DataMember]
        public string StationName { get; set; }

        [DataMember]
        public int TotalSamples { get; set; }
    }
}
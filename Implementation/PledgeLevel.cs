using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace gov.hhs.cdc.cs.campaigns.pledge.Implementation
{
    [DataContract] 
    public class PledgeLevel
    {
        [DataMember]
        public long PledgeLevelId { get; set; }
        [DataMember]
        public string PledgeLevelName { get; set; }
        [DataMember]
        public string PledgeLevelDescription { get; set; }
        [DataMember]
        public string PledgeLevelImagePath { get; set; }
        [DataMember]
        public bool Active { get; set; }
    }
}
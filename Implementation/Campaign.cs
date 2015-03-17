using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace gov.hhs.cdc.cs.campaigns.pledge.Implementation
{
    [DataContract] 
    public class Campaign
    {
        [DataMember]
        public string CampaignId { get; set; }
        [DataMember]
        public string CampaignName { get; set; }
        [DataMember]
        public string CampaignDescription { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        [DataMember]
        public bool Active { get; set; }
    }
}
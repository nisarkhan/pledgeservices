using gov.hhs.cdc.cs.campaigns.pledge.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace gov.hhs.cdc.cs.campaigns.pledge.Implementation
{

    [Serializable]
    public class KMLResponse
    {

        protected List<PlaceMark> _placemarks = new List<PlaceMark>();

        public List<PlaceMark> PlaceMarks
        {
            get
            {
                return _placemarks;
            }
        }

        // public bool AddPlaceMark(string FirstName, string LastInitial, FluPledgeLevel level,


        public string ToKMLString(string campaignId, string campaignName)
        {
            //- 
            //- <name>Flu Pledges</name><description>Pledgers list</description> 
            StringBuilder sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
            sb.AppendLine("<Document>");
            sb.AppendLine("<Name>" + campaignName + " " + "Pledges</Name>");
            sb.AppendLine("<Description>Pledgers list</Description>");


            PledgeService.PledgeService ps = new PledgeService.PledgeService();

            //long totalCount = ps.GetPledgeCountforCampaign("", Helper.GetCampaignId.ToString());
            long totalCount = ps.GetPledgeCountforCampaign("", campaignId);

            sb.Append("<Total>");
            sb.Append(totalCount);
            sb.Append("</Total>");

            sb.AppendLine();

            foreach (PlaceMark p in _placemarks)
            {
                sb.AppendLine(p.ToString());

            }
            sb.AppendLine("</Document>");
            sb.AppendLine("</kml>");
            return sb.ToString();
        }
         
    }

}

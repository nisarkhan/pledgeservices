using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace gov.hhs.cdc.cs.campaigns.pledge.Implementation
{

    [Serializable]
    public class PlaceMark
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

        public PlaceMark()
        {
        }

        public PlaceMark(Pledger p)
        {
            
        }


        public override string ToString()
        {

            if (this.Latitude != double.MinValue &&
               this.Longitude != double.MinValue &&
               !string.IsNullOrEmpty(this.Name))
            {
                // built the coordinates string longitude, latitude, altitude (always 0 for now)
                string coord = this.Longitude.ToString() + "," + this.Latitude.ToString() + ",0";
                StringBuilder sb = new StringBuilder("<Placemark>");
                sb.AppendLine("<name>" + this.Name + "</name>");

                if (!string.IsNullOrEmpty(this.Description))
                    sb.AppendLine("<description>" + this.Description + "</description>");

                if (!string.IsNullOrEmpty(this.IconPath))
                    sb.AppendLine("<Icon><href>" + this.IconPath + "</href></Icon>");

                sb.AppendLine("<Point><coordinates>" + coord + "</coordinates></Point>");
                sb.AppendLine("</Placemark>");
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Hosting; 

namespace gov.hhs.cdc.cs.campaigns.pledge
{
    public static class Helper
    {
        public static string CleanName(string nameIn, List<string> forbiddenNames)
        {
            string result = string.Empty;

            if (!NameIsOk(nameIn, forbiddenNames))

                result = nameIn.Substring(0, 1);
            else
                result = nameIn;

            return result;
        }

        public static bool NameIsOk(string nameIn, List<string> forbiddenNames)
        {
            bool result = false;

            if (!forbiddenNames.Exists(a => a.ToLower() == nameIn.ToLower()))
                result = true;

            return result;
        }

        //ezx8 12/07/2011
        //release: 2.6.1
        //taking out dependency from Settings.setting to web.config.
        public static string PledgeDB
        {
            get
            {
               // string result = null;
                ConnectionStringSettings result = ConfigurationManager.ConnectionStrings["CDC_TOOLS"];
                if (!string.IsNullOrEmpty(result.ConnectionString))
                {
                    return result.ToString();
                }
                else
                {
                    return "";  //???? THROW EXCEPTION???
                }
            }
        }

        public static Int32 DefaultRecordCount
        {
            get
            {
                string result = "49";
                result = ConfigurationManager.AppSettings["DefaultRecordCount"];
                if (!string.IsNullOrEmpty(result))
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    return 49;   
                }
            }
        }
        //end of 2.6.1 modification

        public static string GetCampaignId
        {
            get
            {
                string result = null;
                result = ConfigurationManager.AppSettings["campaign_Id"];
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
                else
                {
                    return "1";
                }   
            }
        }

        public static string GetKMLFileFor_GetCompletePledgeListforCampaignandLevel
        {
            get
            {
                string result = null;
                result = ConfigurationManager.AppSettings["KMLFileFor_GetCompletePledgeListforCampaignandLevel"];
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
                else
                {
                    return "CompletePledgeListforCampaignandLevel.XML";
                } 
            }
        }
        public static string GetKMLFileFor_GetCompletePledgeListforCampaign
        {
            get
            {
                string result = null;
                result = ConfigurationManager.AppSettings["KMLFileFor_GetCompletePledgeListforCampaign"];
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
                else
                {
                    return "CompletePledgeListforCampaign.XML";
                } 
            }
        }
        public static string GetKMLFileFor_GetRecentPledgeListforCampaignandLevel
        {
            get
            {
                string result = null;
                result = ConfigurationManager.AppSettings["KMLFileFor_GetRecentPledgeListforCampaignandLevel"];
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
                else
                {
                    return "RecentPledgeListforCampaignandLevel.XML";
                } 
            }
        }
        public static string GetKMLFileFor_GetRecentPledgeListforCampaign
        {
            get
            {
                string result = null;
                result = ConfigurationManager.AppSettings["KMLFileFor_GetRecentPledgeListforCampaign"];
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
                else
                {
                    return "RecentPledgeListforCampaign.XML";
                } 
            }
        }
        public static string GetKMLFileFor_DoPledge50
        {
            get
            {
                string result = null;
                result = ConfigurationManager.AppSettings["KMLFileFor_DoPledge50"];
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
                else
                {
                    return "DoPledge50.XML";
                } 
            }
        }



        public static string StorePledgeServiceKMLFile
        {            
            get
            {
                string _rootFolder = null;
                _rootFolder = ConfigurationManager.AppSettings["StorePledgeServiceKMLFile"];

                if (!string.IsNullOrEmpty(_rootFolder))
                {
                    if (_rootFolder.StartsWith("~"))
                    {
                        _rootFolder = HostingEnvironment.MapPath(_rootFolder);
                    }
                    if (!_rootFolder.EndsWith("\\"))
                    {
                        _rootFolder += "\\";
                    }
                    return _rootFolder;
                }
                else
                {
                    return HostingEnvironment.MapPath("~/cachedata"); 
                }
                
            }
        } 
    }
}
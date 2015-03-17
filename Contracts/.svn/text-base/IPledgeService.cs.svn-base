using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.Ajax.Samples;
using gov.hhs.cdc.cs.campaigns.pledge.Implementation;

namespace gov.hhs.cdc.cs.campaigns.pledge.Contracts
{
    [ServiceContract]
    public interface IPledgeService
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        Campaign[] GetCampaignList();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        long GetPledgeCountforCampaign(string method, string campaignId);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        long GetPledgeCountforCampaignandLevel(string method, string campaignId, long pledgeLevelId);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        string DoPledge(string method, string campaignId, string firstName, string lastName, string city, string stateProvince, string country, long pledgeLevelId);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        PledgeLevel[] GetPledgeLevels(string method, string campaignId);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        bool CheckCampaignActive(string method, string campaignId);

        //returns KML as a string
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        string GetRecentPledgeListforCampaign(string method, string campaignId, int countToGet);

        //returns KML as a string
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        string GetRecentPledgeListforCampaignandLevel(string method, string campaignId, long pledgeLevelId, int countToGet);

        //returns KML as a string
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        string GetCompletePledgeListforCampaign(string method, string campaignId);

        //returns KML as a string
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        string GetCompletePledgeListforCampaignandLevel(string method, string campaignId, long pledgeLevelId);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        string GetCurrentUser();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        string StorePledgeServiceKMLFile();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        string GetCurrentVersion();

        //11/09/11
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "method")]
        string GetHostingEnvironmentMapPath();  
    } 
}
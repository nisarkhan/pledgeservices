using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;
using gov.hhs.cdc.caching;
using GeoCoding.Google;
using System.Web;
using System.IO;
using System.Security.Principal;
using gov.hhs.cdc.cs.campaigns.pledge;
//using gov.hhs.cdc.cs.campaigns.pledge.Properties;
using gov.hhs.cdc.cs.campaigns.pledge.Implementation;
using gov.hhs.cdc.cs.campaigns.pledge.Contracts;
//using gov.hhs.cdc.cs.syndication.storefront.Properties;

//11/07/2011
using gov.hhs.cdc.cs.campaigns.pledge.PledgeService;
using System.Web.Hosting;
using System.Reflection;


namespace gov.hhs.cdc.cs.campaigns.pledge.PledgeService
{
    public class PledgeService : IPledgeService
    {
        List<string> _forbiddennames = new List<string>();
        List<Campaign> _campaigns = new List<Campaign>();
        SqlConnection _conn = null;
        KMLResponse _response = new KMLResponse();
        SerializedObjectStore _store = null;

        public PledgeService()
        {
            _store = new SerializedObjectStore("PledgeService");

            _conn = new SqlConnection(Helper.PledgeDB);
            _conn.Open();
            if (_conn.State == System.Data.ConnectionState.Open)
            {
                ProcessQueuedPledges();
                LoadForbiddenNames();
                LoadCampaigns();
            }
        }

        #region Protected
        protected void ProcessQueuedPledges()
        {
            //TODO:  Add function here to pick up any pledges that may have come in while the DB was down
        }

        protected void LoadForbiddenNames()
        {
            _forbiddennames = new List<string>();
            if (_conn != null && _conn.State == System.Data.ConnectionState.Open)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = _conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "pledge_GetForbiddenNames";

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _forbiddennames.Add(dr.GetString(dr.GetOrdinal("ForbiddenName")));
                        }
                    }
                    dr.Close();
                    if (_store != null && _store.Valid)
                    {
                        _store.Add(gov.hhs.cdc.cs.campaigns.pledge.Implementation.Constants.BADNAME_KEY, _forbiddennames);
                    }

                }
            }
            else
            {
                if (_store != null && _store.Valid)
                {
                    if (_store.Contains(gov.hhs.cdc.cs.campaigns.pledge.Implementation.Constants.BADNAME_KEY))
                    {
                        _forbiddennames = (List<string>)_store[gov.hhs.cdc.cs.campaigns.pledge.Implementation.Constants.BADNAME_KEY];
                    }
                }
            }

        }

        protected void LoadCampaigns()
        {
            _campaigns = new List<Campaign>();
            if (_conn != null && _conn.State == System.Data.ConnectionState.Open)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = _conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "pledge_GetCampaigns";

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Campaign toAdd = new Campaign();
                            toAdd.CampaignId = dr.GetInt64(dr.GetOrdinal("CampaignId")).ToString();
                            toAdd.CampaignName = dr.GetString(dr.GetOrdinal("Name"));
                            toAdd.StartDate = (DateTime)dr.GetValue(dr.GetOrdinal("StartDate"));
                            toAdd.Active = dr.GetBoolean(dr.GetOrdinal("Active"));

                            if (!dr.IsDBNull(dr.GetOrdinal("Description")))
                                toAdd.CampaignDescription = dr.GetString(dr.GetOrdinal("Description"));

                            if (!dr.IsDBNull(dr.GetOrdinal("EndDate")))
                                toAdd.EndDate = (DateTime)dr.GetValue(dr.GetOrdinal("EndDate"));

                            _campaigns.Add(toAdd);


                        }

                        if (_store != null && _store.Valid)
                            _store.Add(gov.hhs.cdc.cs.campaigns.pledge.Implementation.Constants.CAMPAIGN_KEY, _campaigns);
                    }
                    dr.Close();
                }
            }
            else
            {
                if (_store != null && _store.Valid)
                {
                    if (_store.Contains(gov.hhs.cdc.cs.campaigns.pledge.Implementation.Constants.CAMPAIGN_KEY))
                    {
                        _campaigns = (List<Campaign>)_store[gov.hhs.cdc.cs.campaigns.pledge.Implementation.Constants.CAMPAIGN_KEY];
                    }
                }

            }
        }
        #endregion

        #region GetPledgeCountforCampaign
        public long GetPledgeCountforCampaign(string method, string campaignId)
        {
            long result = -1;
            long lcampaignId = -1;
            if (long.TryParse(campaignId, out lcampaignId))
            {
                if (_conn != null && _conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = _conn;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "pledge_GetPledgeCounts";
                        cmd.Parameters.AddWithValue("@CampaignId", lcampaignId);
                        cmd.Parameters.AddWithValue("@PledgeLevelId", DBNull.Value);
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                result = dr.GetInt64(0);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            return result;
        }
        #endregion

        #region GetPledgeCountforCampaignandLevel
        public long GetPledgeCountforCampaignandLevel(string method, string campaignId, long pledgeLevelId)
        {
            long result = -1;
            long lcampaignId = -1;
            if (long.TryParse(campaignId, out lcampaignId))
            {
                if (_conn != null && _conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = _conn;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "pledge_GetPledgeCounts";
                        cmd.Parameters.AddWithValue("@CampaignId", lcampaignId);
                        cmd.Parameters.AddWithValue("@PledgeLevelId", pledgeLevelId);

                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                result = dr.GetInt64(0);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            return result;
        }
        #endregion

        #region LoadPledgers
        protected KMLResponse LoadPledgers(string campaignId, long pledgelevelid, int count)
        {
            string storekey = gov.hhs.cdc.cs.campaigns.pledge.Implementation.Constants.PLEDGER_KEY + campaignId + pledgelevelid.ToString() + count.ToString();

            long campaign;
            KMLResponse result = new KMLResponse();
            if (long.TryParse(campaignId, out campaign))
            {
                if (_conn != null && _conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = _conn;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "pledge_GetPledgeData";
                        cmd.Parameters.AddWithValue("@CampaignId", campaign);
                        if (pledgelevelid != -1)
                        {
                            cmd.Parameters.AddWithValue("@PledgeLevelId", pledgelevelid);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@PledgeLevelId", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("@ApprovedOnly", true);

                        if (count != -1)
                            cmd.Parameters.AddWithValue("@RecordCount", count);

                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                string city = string.Empty;
                                string stateprovince = string.Empty;
                                string country = string.Empty;
                                string pledge = string.Empty;
                                string imagepath = string.Empty;
                                string imageName = string.Empty;

                                double latitude = double.NegativeInfinity;
                                double longitude = double.NegativeInfinity;

                                string fName = dr.GetString(dr.GetOrdinal("FirstName"));
                                string lName = dr.GetString(dr.GetOrdinal("LastInitial"));

                                if (!dr.IsDBNull(dr.GetOrdinal("LevelDescription")))
                                    pledge = dr.GetString(dr.GetOrdinal("LevelDescription"));

                                if (!dr.IsDBNull(dr.GetOrdinal("LevelImagePath")))
                                    imagepath = dr.GetString(dr.GetOrdinal("LevelImagePath"));

                                if (!dr.IsDBNull(dr.GetOrdinal("LevelImageName")))
                                    imageName = dr.GetString(dr.GetOrdinal("LevelImageName"));

                                if (!dr.IsDBNull(dr.GetOrdinal("City")))
                                    city = dr.GetString(dr.GetOrdinal("City"));

                                if (!dr.IsDBNull(dr.GetOrdinal("StateProvince")))
                                    stateprovince = dr.GetString(dr.GetOrdinal("StateProvince"));

                                if (!dr.IsDBNull(dr.GetOrdinal("Country")))
                                    country = dr.GetString(dr.GetOrdinal("Country"));

                                if (!dr.IsDBNull(dr.GetOrdinal("Latitude")))
                                    latitude = dr.GetDouble(dr.GetOrdinal("Latitude"));

                                if (!dr.IsDBNull(dr.GetOrdinal("Longitude")))
                                    longitude = dr.GetDouble(dr.GetOrdinal("Longitude"));

                                PlaceMark p = new PlaceMark();
                                p.Name = fName + " " + lName + ".";
                                //p.Description = p.Name + " " + pledge;
                                p.Description = p.Name + " " + "from" + " " + city + "," + " " + stateprovince + " " + pledge;
                                p.IconPath = imagepath + imageName;

                                if (!double.IsNegativeInfinity(latitude))
                                    p.Latitude = latitude;
                                if (!double.IsNegativeInfinity(longitude))
                                    p.Longitude = longitude;

                                result.PlaceMarks.Add(p);

                            }

                            if (_store != null && _store.Valid)
                                _store.Add(storekey, result.PlaceMarks);
                        }
                        dr.Close();
                    }
                }
                else
                {
                    if (_store != null && _store.Valid)
                    {
                        if (_store.Contains(storekey))
                        {
                            result.PlaceMarks.Clear();
                            result.PlaceMarks.AddRange((List<PlaceMark>)_store[storekey]);
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        #region GetPledgeLevels
        public PledgeLevel[] GetPledgeLevels(string method, string campaignId)
        {
            string storekey = gov.hhs.cdc.cs.campaigns.pledge.Implementation.Constants.LEVEL_KEY + campaignId;
            List<PledgeLevel> result = new List<PledgeLevel>();
            long lcampaignId = -1;
            if (long.TryParse(campaignId, out lcampaignId))
            {
                if (_conn != null && _conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = _conn;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "pledge_GetPledgeLevels";
                        cmd.Parameters.AddWithValue("@CampaignId", lcampaignId);

                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                PledgeLevel p = new PledgeLevel();
                                p.PledgeLevelId = dr.GetInt64(dr.GetOrdinal("PledgeLevelId"));
                                p.PledgeLevelName = dr.GetString(dr.GetOrdinal("LevelName"));
                                p.Active = dr.GetBoolean(dr.GetOrdinal("Active"));

                                if (!dr.IsDBNull(dr.GetOrdinal("LevelDescription")))
                                    p.PledgeLevelDescription = dr.GetString(dr.GetOrdinal("LevelDescription"));

                                if (!dr.IsDBNull(dr.GetOrdinal("LevelImagePath")))
                                    p.PledgeLevelImagePath = dr.GetString(dr.GetOrdinal("LevelImagePath")) + dr.GetString(dr.GetOrdinal("LevelImageName")); //ezx8



                                result.Add(p);
                            }

                            if (_store != null && _store.Valid)
                                _store.Add(storekey, result);
                        }
                        dr.Close();
                    }
                }
                else
                {
                    if (_store != null && _store.Valid)
                    {
                        if (_store.Contains(storekey))
                        {
                            result = (List<PledgeLevel>)_store[storekey];
                        }
                    }
                }
            }
            return result.ToArray();
        }
        #endregion

        #region ValidatePledger
        protected bool ValidatePledger(string firstName, string lastName, string stateProvince, string country)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(firstName) &&
                !string.IsNullOrEmpty(lastName) &&
                !string.IsNullOrEmpty(stateProvince) &&
                !string.IsNullOrEmpty(country))
            {
                result = true;
            }
            return result;
        }
        #endregion

        #region CheckCampaignActive
        public bool CheckCampaignActive(string method, string campaignId)
        {
            Campaign oCheck = _campaigns.FindLast(a => a.CampaignId == campaignId);
            if (oCheck != null)
                return oCheck.Active;
            else
                return false;
        }
        #endregion

        #region DoPledge
        public string DoPledge(string method, string campaignId, string firstName, string lastName, string city, string stateProvince, string country, long pledgeLevelId)//Pledger value)
        {
            int defaultCount;
            bool coordinates = false;
            PlaceMark p = new PlaceMark();

            string storekey = gov.hhs.cdc.cs.campaigns.pledge.Implementation.Constants.QUEUE_KEY + campaignId.ToString();

            PledgeLevel[] aLevels = this.GetPledgeLevels("", campaignId);

            try
            {
                defaultCount = Helper.DefaultRecordCount;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                defaultCount = 49;
            }

            KMLResponse k = LoadPledgers(campaignId, -1, defaultCount);

            if (ValidatePledger(firstName, lastName, stateProvince, country) && CheckCampaignActive("", campaignId))
            //if (CheckCampaignActive(campaignId))
            {
                double lat = double.NegativeInfinity;
                double lon = double.NegativeInfinity;

                string fName = Helper.CleanName(firstName, _forbiddennames);
                string lName = lastName.Substring(0, 1);

                string newname = fName.Trim() + " " + lName.Trim();

                GoogleGeoCoder ggc = new GoogleGeoCoder("vt57HFRIcPt8BlEzh4RrzEVfCGQ=");
                GeoCoding.Address[] addresses = ggc.GeoCode(string.Empty, city, stateProvince, string.Empty, country);
                if (addresses.Length > 0)
                {
                    coordinates = true;
                    // found something... take the first one and get the lat long
                    lat = addresses[0].Coordinates.Latitude;
                    lon = addresses[0].Coordinates.Longitude;

                    string pledge = "took a pledge";
                    PledgeLevel pl = aLevels.FirstOrDefault(a => a.PledgeLevelId == pledgeLevelId);

                    if (Helper.NameIsOk(newname, _forbiddennames))
                    {
                        p.Name = fName + " " + lName + ".";
                    }
                    else
                    {
                        p.Name = "Someone";
                    }

                    if (pl != null)
                    {
                        pledge = pl.PledgeLevelDescription;
                        p.IconPath = pl.PledgeLevelImagePath;
                    }

                    p.Description = p.Name + " " + pledge;
                    p.Latitude = lat; //added: 12/01/10 ezx8
                    p.Longitude = lon; //added: 12/01/10 ezx8

                }

                //string pledge = "took a pledge";
                //PledgeLevel pl = aLevels.FirstOrDefault(a => a.PledgeLevelId == pledgeLevelId);

                //PlaceMark p = new PlaceMark();
                //if (Helper.NameIsOk(newname, _forbiddennames))
                //{
                //    p.Name = fName + " " + lName + ".";
                //}
                //else
                //{
                //    p.Name = "Someone";
                //}

                //if (pl != null)
                //{
                //    pledge = pl.PledgeLevelDescription;
                //    p.IconPath = pl.PledgeLevelImagePath;
                //}

                //p.Description = p.Name + " " + pledge;
                //p.Latitude = lat; //added: 12/01/10 ezx8
                //p.Longitude = lon; //added: 12/01/10 ezx8

                #region db connect
                if (_conn != null && _conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = _conn;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "pledge_RecordPledge";
                        cmd.Parameters.AddWithValue("@CampaignId", long.Parse(campaignId));
                        cmd.Parameters.AddWithValue("@PledgeLevelId", pledgeLevelId);
                        cmd.Parameters.AddWithValue("@FirstName", fName);
                        cmd.Parameters.AddWithValue("@LastInitial", lName);

                        if (!string.IsNullOrEmpty(city))
                            cmd.Parameters.AddWithValue("@City", city);
                        else
                            cmd.Parameters.AddWithValue("@City", DBNull.Value);

                        if (!string.IsNullOrEmpty(stateProvince))
                            cmd.Parameters.AddWithValue("@StateProvince", stateProvince);
                        else
                            cmd.Parameters.AddWithValue("@StateProvince", DBNull.Value);

                        if (!string.IsNullOrEmpty(country))
                            cmd.Parameters.AddWithValue("@Country", country);
                        else
                            cmd.Parameters.AddWithValue("@Country", DBNull.Value);

                        if (!double.IsNegativeInfinity(lat))
                            cmd.Parameters.AddWithValue("@Latitude", lat);
                        else
                            cmd.Parameters.AddWithValue("@Latitude", DBNull.Value);

                        if (!Double.IsNegativeInfinity(lon))
                            cmd.Parameters.AddWithValue("@Longitude", lon);
                        else
                            cmd.Parameters.AddWithValue("@Longitude", DBNull.Value);

                        int iReturn = cmd.ExecuteNonQuery();

                        if (iReturn != 0 && coordinates)
                        {
                            k.PlaceMarks.Insert(0, p);
                        }

                    }
                }
                //else
                //{
                //    List<Pledger> queue = null;
                //    if (_store != null && _store.Valid)
                //    {
                //        if (_store.Contains(storekey))
                //        {
                //            queue = (List<Pledger>)_store[storekey];
                //        }
                //        else
                //        {
                //            queue = new List<Pledger>();
                //        }

                //    }
                //}
            }
                #endregion

            #region generating kml file file after the pledger pledge:
            string _kmlPath = Helper.StorePledgeServiceKMLFile;
            string _kmlFileName = Helper.GetKMLFileFor_DoPledge50;

            string result = k.ToKMLString(campaignId, GetCampaignName(campaignId).ToString()).ToString();
            //leaving as it is since the flu pledge is already in production!
            if (campaignId == "1")// && _kmlFileName == "DoPledge50.XML") 
            {
                using (StreamWriter sw = new StreamWriter(_kmlPath + _kmlFileName))
                {
                    sw.Write(result);
                }
            }
            else
            {
                //going forward DoPledge50.xml will generate based on the comapignId
                //so for campaignId = 2 the xml file will be something like this:
                //DoPledge50_2.xml
                using (StreamWriter sw = new StreamWriter(_kmlPath + "DoPledge50_" + campaignId + ".XML"))
                {
                    //sw.Write(k.ToKMLString(campaignId, GetCampaignName(campaignId).ToString()));
                    sw.Write(result);
                }
            }
            #endregion

            return result;
        }
        #endregion

        #region GetCampaignList
        public Campaign[] GetCampaignList()
        {
            if (_campaigns != null && _campaigns.Count > 0)
                return _campaigns.ToArray();
            else
                return new Campaign[0];
        }
        #endregion

        #region Returns KML
        //(@"\\apd-v-nchm-vss1\webdev\nisar\file.xml"))//HttpContext.Current.Server.MapPath("KMLFile.kml")))
        #region GetRecentPledgeListforCampaign
        public string GetRecentPledgeListforCampaign(string method, string campaignId, int countToGet)
        {
            KMLResponse k = LoadPledgers(campaignId, -1, countToGet);
            string _kmlPath = Helper.StorePledgeServiceKMLFile;
            string _kmlFileName = Helper.GetKMLFileFor_GetRecentPledgeListforCampaign;
            string result = k.ToKMLString(campaignId, GetCampaignName(campaignId).ToString()).ToString();
            using (StreamWriter sw = new StreamWriter(_kmlPath + _kmlFileName))
            {
                sw.Write(result);//k.ToKMLString(campaignId, GetCampaignName(campaignId).ToString()));
            }
            return result;
        }
        #endregion

        #region GetRecentPledgeListforCampaign
        public string GetRecentPledgeListforCampaignandLevel(string method, string campaignId, long pledgeLevelId, int countToGet)
        {
            KMLResponse k = LoadPledgers(campaignId, pledgeLevelId, countToGet);
            string _kmlPath = Helper.StorePledgeServiceKMLFile;
            string _kmlFileName = Helper.GetKMLFileFor_GetRecentPledgeListforCampaignandLevel;
            string result = k.ToKMLString(campaignId, GetCampaignName(campaignId).ToString()).ToString();
            using (StreamWriter sw = new StreamWriter(_kmlPath + _kmlFileName))
            {
                sw.Write(result);
            }
            return result;
        }
        #endregion

        #region GetRecentPledgeListforCampaign

        public string GetCompletePledgeListforCampaign(string method, string campaignId)
        {
            KMLResponse k = LoadPledgers(campaignId, -1, -1);
            string _kmlPath = Helper.StorePledgeServiceKMLFile;
            string _kmlFileName = Helper.GetKMLFileFor_GetCompletePledgeListforCampaign;
            string result = k.ToKMLString(campaignId, GetCampaignName(campaignId).ToString()).ToString();
            using (StreamWriter sw = new StreamWriter(_kmlPath + _kmlFileName))
            {
                sw.Write(result);//k.ToKMLString(campaignId, GetCampaignName(campaignId).ToString()));
            }
            return k.ToString();
        }
        #endregion

        #region GetRecentPledgeListforCampaign
        public string GetCompletePledgeListforCampaignandLevel(string method, string campaignId, long pledgeLevelId)
        {
            KMLResponse k = LoadPledgers(campaignId, pledgeLevelId, -1);
            string _kmlPath = Helper.StorePledgeServiceKMLFile;
            string _kmlFileName = Helper.GetKMLFileFor_GetCompletePledgeListforCampaignandLevel;
            string result = k.ToKMLString(campaignId, GetCampaignName(campaignId).ToString()).ToString();
            using (StreamWriter sw = new StreamWriter(_kmlPath + _kmlFileName))
            {
                sw.Write(result);//k.ToKMLString(campaignId, GetCampaignName(campaignId).ToString()));
            }
            return k.ToString();
        }
        #endregion

        #endregion

        public string GetCurrentUser()
        {
            return WindowsIdentity.GetCurrent().Name;
        }

        public string StorePledgeServiceKMLFile()
        {
            return Helper.StorePledgeServiceKMLFile;
        }

        //ezx8
        //added on 06/15/2011
        private string GetCampaignName(string campaignId)
        {
            //ezx8 06/15/2011
            string campaignName = "Not Found";

            //get the campaign based on campaign id:
            Campaign campaign = _campaigns.FirstOrDefault(c => c.CampaignId == campaignId);

            if (campaign != null)
            {
                campaignName = campaign.CampaignName;
            }
            return campaignName;
        }

        public string GetCurrentVersion()
        {
           return String.Format("The version of the currently executing assembly is:=  {0}", Assembly.GetExecutingAssembly().GetName().Version);
        }

        //11/09/11
        public string GetHostingEnvironmentMapPath()
        {
            return HostingEnvironment.MapPath("~/virtualPath");
        }   
    }
}   
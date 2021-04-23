using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace IEXTrading.Models
{
    public class Companys
    {
        [Key]
        public int company_number { get; set; }
        public string company_name { get; set; }
 
    }

    public class customer
    {
        [Key]
        public int customer_ID { get; set; }
        public int customer_areacode { get; set; }
        public string customer_city { get; set; }
        public string customer_state { get; set; }
        
    }
    public class DoNotCallComplaints
    {
        [Key]
        public string complaint_ID { get; set; }
        public DateTime violation_date { get; set; }
        public DateTime creation_date { get; set; }
        public string subject { get; set; }
        public string Robocall { get; set; }
        public Companys ViolComapny { get; set; }
        public customer ComplCustomet { get; set; }
    }

    public class NoRoboComplaints
    {
        public string id { get; set; }
        [JsonProperty("company-phone-number")]
        public string company_phone_number {get; set;}
        public DateTime violation_date { get; set; }
        public DateTime created_date { get; set; }
        public string consumer_area_code { get; set; }
        public string consumer_city { get; set; }
        public string consumer_state { get; set; }
        public string subject { get; set; }
        public string Robocall { get; set; }




    }


    public class desRobo
    {
        public string type { get; set; }
        public string id { get; set; }
        [JsonProperty("attributes")]
        public attribute attributes { get; set; }
        //public string relationships { get; set; }
        //public string meta { get; set; }
        //public List<link> links { get; set; }


    }

    public class attribute
    {
        [JsonProperty("company-phone-number")]
        public string company_phone_number { get; set; }
        [JsonProperty("violation-date")]
        public DateTime violation_date { get; set; }
        [JsonProperty("created-date")]
        public DateTime created_date { get; set; }
        [JsonProperty("consumer-area-code")]
        public string consumer_area_code { get; set; }
        [JsonProperty("consumer-city")]
        public string consumer_city { get; set; }
        [JsonProperty("consumer-state")]
        public string consumer_state { get; set; }
        public string subject { get; set; }
        [JsonProperty("recorded-message-or-robocall")]
        public string Robocall { get; set; }
    }


public class inputstate
    {
        public string state { get; set; }
    }
    public class link
    {
        public string self { get; set; }
    }

}

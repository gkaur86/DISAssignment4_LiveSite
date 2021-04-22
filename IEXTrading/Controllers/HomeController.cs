using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IEXTrading.Infrastructure.IEXTradingHandler;
using IEXTrading.Models;
using IEXTrading.Models.ViewModel;
using IEXTrading.DataAccess;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace MVCTemplate.Controllers
{
    
    public class HomeController : Controller
    {
        HttpClient httpClient;
        static string initial_json = "https://gurmeetk.com/DIS/datasmaller.json";

        static string base_url = "";
        static string AP_key = "";

        public ApplicationDbContext dbContext;
        private readonly AppSettings _appSettings;
        public const string SessionKeyName = "StockData";
        //List<Company> companies = new List<Company>();
        public HomeController(ApplicationDbContext context, IOptions<AppSettings> appSettings)
        {
            dbContext = context;
            _appSettings = appSettings.Value;
        }
        
       

        public IActionResult HelloIndex()
        {
            ViewBag.Hello = _appSettings.Hello;
            return View();
        }
        public IActionResult Index()
        {
            //dbContext.Mitigate();
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
          new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


            string initialData = "";

            //NoRoboComplaints Storehere = null;

            httpClient.BaseAddress = new Uri(initial_json);

            try
            {
                HttpResponseMessage response = httpClient.GetAsync(initial_json).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    initialData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                }

                if(!initialData.Equals(""))
                {
                    //Console.WriteLine(initialData);
                    Console.WriteLine("Starting");
                    var Storehere = JsonConvert.DeserializeObject < List < desRobo >> (initialData);
                    Console.WriteLine("I am here");
                    Storehere.ForEach(p =>
                    {
                        dbContext.Database.EnsureCreated();
                        Console.WriteLine("I am here too");
                        var urlNameExists = dbContext.Robocalls.Any(x => x.id == p.id);
                        Console.WriteLine("I am here three");
                        if (!urlNameExists)
                        {
                            Console.WriteLine("I am here FOUR");
                            NoRoboComplaints Newrecord = new NoRoboComplaints()
                            {
                                id = p.id,
                                consumer_area_code = p.attributes.consumer_area_code,
                                created_date = p.attributes.created_date,
                                violation_date = p.attributes.violation_date,
                                consumer_city = p.attributes.consumer_city,
                                consumer_state = p.attributes.consumer_state,
                                company_phone_number =  p.attributes.company_phone_number,
                                subject = p.attributes.subject,
                                Robocall = p.attributes.Robocall
                            
                            };
                            dbContext.Robocalls.Add(Newrecord);
                            dbContext.SaveChanges();

                        }





                    });
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return View();
        }
        public IActionResult AboutUS()
        {
            return View();
        }
        /****
         * The Symbols action calls the GetSymbols method that returns a list of Companies.
         * This list of Companies is passed to the Symbols View.
        ****/
        public IActionResult Symbols()
        {
            //Set ViewBag variable first
            ViewBag.dbSucessComp = 0;
            IEXHandler webHandler = new IEXHandler();
            List<Company> companies = webHandler.GetSymbols();

            String companiesData = JsonConvert.SerializeObject(companies);
            //int size =  System.Text.ASCIIEncoding.ASCII.GetByteCount(companiesData);

            HttpContext.Session.SetString(SessionKeyName, companiesData);
            //Save comapnies in TempData
            //if ( size < 4000)
            //{
            //    TempData["Companies"] = companiesData;
            //}
            //else
            //{
            //    TempData["Companies"] = "Fetch";
            //}
            
            

            return View(companies);
        }

        /****
         * The Chart action calls the GetChart method that returns 1 year's equities for the passed symbol.
         * A ViewModel CompaniesEquities containing the list of companies, prices, volumes, avg price and volume.
         * This ViewModel is passed to the Chart view.
        ****/
        public IActionResult Chart(string symbol)
        {
            //Set ViewBag variable first
            ViewBag.dbSuccessChart = 0;
            List<Equity> equities = new List<Equity>();
            if (symbol != null)
            {
                IEXHandler webHandler = new IEXHandler();
                equities = webHandler.GetChart(symbol);
                equities = equities.OrderBy(c => c.date).ToList(); //Make sure the data is in ascending order of date.
            }

            CompaniesEquities companiesEquities = getCompaniesEquitiesModel(equities);

            return View(companiesEquities);
        }



        public IActionResult Charted(string state)
        {

            return View();
        }
        /****
         * The Refresh action calls the ClearTables method to delete records from a or all tables.
         * Count of current records for each table is passed to the Refresh View.
        ****/
        public IActionResult Refresh(string tableToDel)
        {
            ClearTables(tableToDel);
            Dictionary<string, int> tableCount = new Dictionary<string, int>();
            tableCount.Add("Companies", dbContext.Companies.Count());
            tableCount.Add("Charts", dbContext.Equities.Count());
            return View(tableCount);
        }

        /****
         * Saves the Symbols in database.
        ****/
        public IActionResult PopulateSymbols()
        {
            string companiesData = HttpContext.Session.GetString(SessionKeyName);
            List<Company> companies = null;
            if (companiesData != "")
            {
                 companies = JsonConvert.DeserializeObject<List<Company>>(companiesData);
            }
            
            foreach (Company company in companies)
            {
                //Database will give PK constraint violation error when trying to insert record with existing PK.
                //So add company only if it doesnt exist, check existence using symbol (PK)
                if (dbContext.Companies.Where(c => c.symbol.Equals(company.symbol)).Count() == 0)
                {
                    dbContext.Companies.Add(company);
                }
            }
            dbContext.SaveChanges();
            ViewBag.dbSuccessComp = 1;
            return View("Symbols", companies);
        }

        /****
         * Saves the equities in database.
        ****/
        public IActionResult SaveCharts(string symbol)
        {
            IEXHandler webHandler = new IEXHandler();
            List<Equity> equities = webHandler.GetChart(symbol);
            //List<Equity> equities = JsonConvert.DeserializeObject<List<Equity>>(TempData["Equities"].ToString());
            foreach (Equity equity in equities)
            {
                if (dbContext.Equities.Where(c => c.date.Equals(equity.date)).Count() == 0)
                {
                    dbContext.Equities.Add(equity);
                }
            }

            dbContext.SaveChanges();
            ViewBag.dbSuccessChart = 1;

            CompaniesEquities companiesEquities = getCompaniesEquitiesModel(equities);

            return View("Chart", companiesEquities);
        }
        //add about page

        /****
         * Deletes the records from tables.
        ****/
        public void ClearTables(string tableToDel)
        {
            if ("all".Equals(tableToDel))
            {
                //First remove equities and then the companies
                dbContext.Equities.RemoveRange(dbContext.Equities);
                dbContext.Companies.RemoveRange(dbContext.Companies);
            }
            else if ("Companies".Equals(tableToDel))
            {
                //Remove only those that don't have Equity stored in the Equitites table
                dbContext.Companies.RemoveRange(dbContext.Companies
                                                         .Where(c => c.Equities.Count == 0)
                                                                      );
            }
            else if ("Charts".Equals(tableToDel))
            {
                dbContext.Equities.RemoveRange(dbContext.Equities);
            }
            dbContext.SaveChanges();
        }

        /****
         * Returns the ViewModel CompaniesEquities based on the data provided.
         ****/
        public CompaniesEquities getCompaniesEquitiesModel(List<Equity> equities)
        {
            List<Company> companies = dbContext.Companies.ToList();

            if (equities.Count == 0)
            {
                return new CompaniesEquities(companies, null, "", "", "", 0, 0);
            }

            Equity current = equities.Last();
            string dates = string.Join(",", equities.Select(e => e.date));
            string prices = string.Join(",", equities.Select(e => e.high));
            string volumes = string.Join(",", equities.Select(e => e.volume / 1000000)); //Divide vol by million
            float avgprice = equities.Average(e => e.high);
            double avgvol = equities.Average(e => e.volume) / 1000000; //Divide volume by million
            return new CompaniesEquities(companies, equities.Last(), dates, prices, volumes, avgprice, avgvol);
        }

    }
}

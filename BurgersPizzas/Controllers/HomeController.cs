using BurgersPizzas.Models;
using Microsoft.AspNetCore.Mvc;
using BurgersPizzas.DataAccess;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization.Json;

namespace BurgersPizzas.Controllers
{
    public class HomeController : Controller
    {
        HttpClient httpClient;

        //static string BASE_URL = "https://developer.nps.gov/api/v1";
        //static string API_KEY = "mdBybOievMdeX3eYSC0MhFu3U7xRV18xHAPG04qb"; //Add your API key here inside ""


        static string BURGERS_BASE_URL = "https://burgers1.p.rapidapi.com/burgers";
        static string BURGERS_API_KEY = "5a7f563dd0msh635408e9a35c696p12b244jsn633d591c0ec3";
        static string BURGERS_X_Host = "burgers1.p.rapidapi.com";

        static string PIZZAS_BASE_URL = "https://new-pizza-api.p.rapidapi.com/Pizzas";
        static string PIZZAS_API_KEY = "f0c80d2a20msh4aa5b1003ed1d27p14a0f1jsneab211879b82";
        static string PIZZAS_X_Host = "new-pizza-api.p.rapidapi.com";

        static string NUTRIENTS_BASE_URL = "https://calorieninjas.p.rapidapi.com/v1/nutrition?query=sugar";
        static string NUTRIENTS_API_KEY = "3ac31c34b3msh338b3e5ec340784p117e15jsn06d6ed3ffe76";
        static string NUTRIENTS_X_HOST = "calorieninjas.p.rapidapi.com";
        //Add your API key here inside ""
        // Obtaining the API key is easy. The same key should be usable across the entire
        // data.gov developer network, i.e. all data sources on data.gov.
        // https://www.nps.gov/subjects/developer/get-started.htm

        public ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                if (dbContext != null)
                {
                    if (dbContext.BurgersDb.Count()== 0)
                    {
                       
                            httpClient = new HttpClient();
                            httpClient.DefaultRequestHeaders.Accept.Clear();
                            httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", BURGERS_API_KEY);
                            httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", BURGERS_X_Host);
                            httpClient.DefaultRequestHeaders.Accept.Add(
                                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            //  string NATIONAL_PARK_API_PATH = BASE_URL + "/parks?limit=20";
                            string BURGERS_API_PATH = BURGERS_BASE_URL;
                            string burgersData = "";

                            BurgerDb burgers = null;

                            //httpClient.BaseAddress = new Uri(NATIONAL_PARK_API_PATH);
                            httpClient.BaseAddress = new Uri(BURGERS_API_PATH);

                            //HttpResponseMessage response = httpClient.GetAsync(NATIONAL_PARK_API_PATH)
                            //                                        .GetAwaiter().GetResult();
                            HttpResponseMessage response = httpClient.GetAsync(BURGERS_API_PATH)
                                                                    .GetAwaiter().GetResult();



                            if (response.IsSuccessStatusCode)
                            {
                                burgersData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            }
                            List<Item> bg;
                            if (!burgersData.Equals(""))
                            {
                                // JsonConvert is part of the NewtonSoft.Json Nuget package

                                dbContext.CategoryDb.Add(new CategoryDb { Category = "burger" });
                                dbContext.CategoryDb.Add(new CategoryDb { Category = "pizza" });
                                await dbContext.SaveChangesAsync();
                                bg = JsonConvert.DeserializeObject<List<Item>>(burgersData);
                                foreach (Item b in bg)
                                {
                                    string ingredientsstring = string.Empty;
                                    foreach (string str in b.Ingredients)
                                    {
                                        ingredientsstring = ingredientsstring + str + ",";
                                    }
                                    CategoryDb category = dbContext.CategoryDb.Where(c => c.Category == "burger").First();
                                    dbContext.BurgersDb.Add(new BurgerDb { Name = b.Name.ToLower(), Description = b.Description, Ingredients = ingredientsstring, Web = b.Web, Category = category });
                                }
                                await dbContext.SaveChangesAsync();
                            }

                    }

                    if (dbContext.PizzasDb.Count() ==0)
                    {
                       
                            httpClient = new HttpClient();
                            httpClient.DefaultRequestHeaders.Accept.Clear();
                            httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", PIZZAS_API_KEY);
                            httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", PIZZAS_X_Host);
                            httpClient.DefaultRequestHeaders.Accept.Add(
                                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            //  string NATIONAL_PARK_API_PATH = BASE_URL + "/parks?limit=20";
                            string PIZZAS_API_PATH = PIZZAS_BASE_URL;
                            string pizzasData = "";

                            PizzaDb pizzas = null;

                            //httpClient.BaseAddress = new Uri(NATIONAL_PARK_API_PATH);
                            httpClient.BaseAddress = new Uri(PIZZAS_BASE_URL);


                            //HttpResponseMessage response = httpClient.GetAsync(NATIONAL_PARK_API_PATH)
                            //                                        .GetAwaiter().GetResult();
                            HttpResponseMessage response_ = httpClient.GetAsync(PIZZAS_BASE_URL)
                                                                    .GetAwaiter().GetResult();



                            if (response_.IsSuccessStatusCode)
                            {
                                pizzasData = response_.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            }
                            List<Item> bg_1;
                            if (!pizzasData.Equals(""))
                            {
                                // JsonConvert is part of the NewtonSoft.Json Nuget package
                                bg_1 = JsonConvert.DeserializeObject<List<Item>>(pizzasData);
                                foreach (Item b in bg_1)
                                {
                                    string ingredientsstring = string.Empty;
                                    foreach (string str in b.Ingredients)
                                    {
                                        ingredientsstring = ingredientsstring + str + ",";
                                    }
                                    CategoryDb category = dbContext.CategoryDb.Where(c => c.Category == "pizza").First();
                                    dbContext.PizzasDb.Add(new PizzaDb { Name = b.Name.ToLower(), Description = b.Description, Ingredients = ingredientsstring, Web = b.Web, Category = category });
                                }
                                await dbContext.SaveChangesAsync();
                            }
                    }
                }
            }


            catch (Exception e)
            {
                // This is a useful place to insert a breakpoint and observe the error message
                Console.WriteLine(e.Message);
            }

            return View();


        }

        public async Task<IActionResult> Burger()
        {
            var burgers = dbContext.BurgersDb.Include(c => c.Category).ToList();
            List<ItemData> l = new List<ItemData>();
            foreach (var b in burgers)
            {
                ItemData item = new ItemData() { Id = b.Id, Name = b.Name, Ingredients = b.Ingredients, Web = b.Web, Description = b.Description};
                l.Add(item);
            }
            TempData["isBurger"] = true;
            return View(l);
        }

        public async Task<IActionResult> Pizza()
        {
            var burgers = dbContext.PizzasDb.Include(c => c.Category).ToList();
            List<ItemData> l = new List<ItemData>();
            foreach (var b in burgers)
            {
                ItemData item = new ItemData() { Id = b.Id, Name = b.Name, Ingredients = b.Ingredients, Web = b.Web, Description = b.Description};
                l.Add(item);
            }
            TempData["isBurger"] = false;
            return View("~/Views/Home/Burger.cshtml", l);
        }

        [HttpPost]
        public ActionResult AddItem(ItemData item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Convert.ToBoolean(TempData["isBurger"]) == true)
                    {
                        if (item.isDisabled == false)
                        {
                            var burgers = dbContext.BurgersDb.Where(x => x.Name == item.Name.Trim().ToLower()).Count();
                            if (burgers == 0)
                            {
                                BurgerDb burger = new BurgerDb() { Name = item.Name, Ingredients = item.Ingredients, Web = item.Web, Description = item.Description };
                                burger.Category = dbContext.CategoryDb.Where(c => c.Category == "burger").First();
                                dbContext.BurgersDb.Add(burger);
                                dbContext.SaveChanges();
                                //var burgers = dbContext.BurgersDb.ToList();
                                //List<ItemData> l = new List<ItemData>();
                                //foreach (var b in burgers)
                                //{
                                //    ItemData item_ = new ItemData() { Id = b.Id, Name = b.Name, Ingredients = b.Ingredients, Web = b.Web, Description = b.Description };
                                //    l.Add(item_);
                                //}

                                return RedirectToAction("burger");
                            }
                            else
                            {
                                ViewData["Message"] = "Name already in use.Try using a different name";
                                return View("~/Views/Home/Update.cshtml", item);
                            }
                        }
                        else
                        {
                            BurgerDb burger_=dbContext.BurgersDb.Where(x => x.Name == item.Name).ToList()[0];
                            burger_.Name = item.Name;
                            burger_.Ingredients = item.Ingredients;
                            burger_.Web = item.Web;
                            burger_.Description = item.Description;
                            burger_.Category = dbContext.CategoryDb.Where(c => c.Category == "burger").First();
                            dbContext.SaveChanges();
                            return RedirectToAction("burger");
                        }
                    }
                    else
                    {
                        

                        if (item.isDisabled == false)
                        {
                            var burgers = dbContext.PizzasDb.Where(x => x.Name == item.Name.Trim().ToLower()).Count();
                            if (burgers == 0)
                            {
                                PizzaDb burger_ = new PizzaDb() { Name = item.Name, Ingredients = item.Ingredients, Web = item.Web, Description = item.Description };
                                burger_.Category = dbContext.CategoryDb.Where(c => c.Category == "burger").First();
                                dbContext.PizzasDb.Add(burger_);
                                dbContext.SaveChanges();
                                //var burgers = dbContext.BurgersDb.ToList();
                                //List<ItemData> l = new List<ItemData>();
                                //foreach (var b in burgers)
                                //{
                                //    ItemData item_ = new ItemData() { Id = b.Id, Name = b.Name, Ingredients = b.Ingredients, Web = b.Web, Description = b.Description };
                                //    l.Add(item_);
                                //}

                                return RedirectToAction("Pizza");
                            }
                            else
                            {
                                ViewData["Message"] = "Name already in use.Try using a different name";
                                return View("~/Views/Home/Update.cshtml", item);
                            }
                        }
                        else
                        {
                            PizzaDb burger_ = dbContext.PizzasDb.Where(x => x.Name == item.Name).ToList()[0];
                            burger_. Name = item.Name;
                            burger_.Ingredients = item.Ingredients;
                            burger_.Web = item.Web;
                            burger_.Description = item.Description;
                            burger_.Category = dbContext.CategoryDb.Where(c => c.Category == "pizza").First();
                            dbContext.SaveChanges();
                            return RedirectToAction("Pizza");
                        }

                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception  /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                
            }
            return View();

        }
        [HttpPost]

        public async Task<ActionResult> DeleteBurger(string burgername)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BurgerDb burger = dbContext.BurgersDb.Where(x => x.Name == burgername).ToList()[0];
                    dbContext.BurgersDb.Remove(burger);
                    dbContext.SaveChanges();
                    CategoryDb category = dbContext.CategoryDb.Include(c => c.Burgers).Where(c => c.Category == "burgers").First();
                    var burgerslist = category.Burgers;
                    ViewData["data"] = burgerslist;

                }
            }
            catch (Exception  /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View("burgers");
        }

        [HttpPost]

        public async Task<ActionResult> EditBurger(BurgerDb burger)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BurgerDb burger_ = dbContext.BurgersDb.Where(x => x.Name == burger.Name).ToList()[0];
                    dbContext.BurgersDb.Remove(burger_);
                    dbContext.SaveChanges();
                    CategoryDb category = dbContext.CategoryDb.Include(c => c.Burgers).Where(c => c.Category == "burgers").First();
                    var burgerslist = category.Burgers;
                    ViewData["data"] = burgerslist;

                }
            }
            catch (Exception  /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View("burgers");
        }

        public ActionResult Add()
        {
            ItemData item = new ItemData() { isDisabled=false};
            return View("~/Views/Home/Update.cshtml",item);
        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult UpdaterForm(string Name)
        {
           if(Convert.ToBoolean(TempData["isburger"])==true)
            {
                TempData.Keep();
                
               var burger= dbContext.BurgersDb.Where(x => x.Name == Name).ToList()[0];
                ItemData itm = new ItemData() {Ingredients=burger.Ingredients,Web=burger.Web,Description=burger.Description,Name=burger.Name,isDisabled=true};
                return View("~/Views/Home/Update.cshtml", itm);
            }
            else
            {
                TempData.Keep();
                var pizza = dbContext.PizzasDb.Where(x => x.Name == Name).ToList()[0];
                ItemData itm = new ItemData() { Ingredients = pizza.Ingredients, Web = pizza.Web, Description = pizza.Description, Name = pizza.Name ,isDisabled=true};
                return View("~/Views/Home/Update.cshtml", itm);
            }
        }
        [HttpGet]
        public ActionResult Delete(string Name)
        {
            int success=0;
            try
            {
                if (ModelState.IsValid)
                {
                    if (Convert.ToBoolean(TempData["isBurger"]) == true)
                    {
                        BurgerDb burger = dbContext.BurgersDb.Where(x=>x.Name==Name).ToList()[0];
                        burger.Category = dbContext.CategoryDb.Where(c => c.Category == "burger").First();
                        dbContext.BurgersDb.Remove(burger);
                        success=dbContext.SaveChanges();
                        //var burgers = dbContext.BurgersDb.ToList();
                        //List<ItemData> l = new List<ItemData>();
                        //foreach (var b in burgers)
                        //{
                        //    ItemData item_ = new ItemData() { Id = b.Id, Name = b.Name, Ingredients = b.Ingredients, Web = b.Web, Description = b.Description };
                        //    l.Add(item_);
                        //}
                       
                        return RedirectToAction("Burger");
                    }
                    else
                    {
                        int count = dbContext.PizzasDb.Count();
                        PizzaDb burger_ = dbContext.PizzasDb.Where(x => x.Name == Name).ToList()[0];
                        burger_.Category = dbContext.CategoryDb.Where(c => c.Category == "pizza").First();
                        dbContext.PizzasDb.Remove(burger_);
                        
                        
                        //var burgers = dbContext.BurgersDb.ToList();
                        //List<ItemData> l = new List<ItemData>();
                        //foreach (var b in burgers)
                        //{
                        //    ItemData item_ = new ItemData() { Id = b.Id, Name = b.Name, Ingredients = b.Ingredients, Web = b.Web, Description = b.Description };
                        //    l.Add(item_);
                        //}
                        success = dbContext.SaveChanges();
                        int count_ = dbContext.PizzasDb.Count();
                        return RedirectToAction("Pizza");


                    }
                }
            }
            catch (Exception  /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return RedirectToAction("Burger");

        }

        [HttpGet]
        public ActionResult GetNutrients(string Name)
        {
              string NUTRIENTS_BASE_URL = "https://calorieninjas.p.rapidapi.com/v1/nutrition";
         string NUTRIENTS_API_KEY = "3ac31c34b3msh338b3e5ec340784p117e15jsn06d6ed3ffe76";
        string NUTRIENTS_X_HOST = "calorieninjas.p.rapidapi.com";
             httpClient = new HttpClient();
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", NUTRIENTS_API_KEY);
                        httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", NUTRIENTS_X_HOST);
                        httpClient.DefaultRequestHeaders.Accept.Add(
                            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //  string NATIONAL_PARK_API_PATH = BASE_URL + "/parks?limit=20";
            string NUTRIENTS_API_PATH = NUTRIENTS_BASE_URL + "/?query=" + Name;
                        string burgersData = "";

                        BurgerDb burgers = null;

                        //httpClient.BaseAddress = new Uri(NATIONAL_PARK_API_PATH);
                        httpClient.BaseAddress = new Uri(NUTRIENTS_API_PATH);

                        //HttpResponseMessage response = httpClient.GetAsync(NATIONAL_PARK_API_PATH)
                        //                                        .GetAwaiter().GetResult();
                        HttpResponseMessage response = httpClient.GetAsync(NUTRIENTS_API_PATH)
                                                                .GetAwaiter().GetResult();



                        if (response.IsSuccessStatusCode)
                        {
                            burgersData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        }
                        List<Item> bg;
                        if (!burgersData.Equals(""))
                        {
                            // JsonConvert is part of the NewtonSoft.Json Nuget package

                           //Initialize model and call view
                        }
            return View();

        }

        [HttpGet]
        public JsonResult GetAjax(string param)
        {
            string NUTRIENTS_BASE_URL = "https://calorieninjas.p.rapidapi.com/v1/nutrition";
            string NUTRIENTS_API_KEY = "3ac31c34b3msh338b3e5ec340784p117e15jsn06d6ed3ffe76";
            string NUTRIENTS_X_HOST = "calorieninjas.p.rapidapi.com";
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", NUTRIENTS_API_KEY);
            httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", NUTRIENTS_X_HOST);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //  string NATIONAL_PARK_API_PATH = BASE_URL + "/parks?limit=20";
            string NUTRIENTS_API_PATH = NUTRIENTS_BASE_URL + "?query=" + param;
            string burgersData = "";

            BurgerDb burgers = null;

            //httpClient.BaseAddress = new Uri(NATIONAL_PARK_API_PATH);
            httpClient.BaseAddress = new Uri(NUTRIENTS_API_PATH);

            //HttpResponseMessage response = httpClient.GetAsync(NATIONAL_PARK_API_PATH)
            //                                        .GetAwaiter().GetResult();
            HttpResponseMessage response = httpClient.GetAsync(NUTRIENTS_API_PATH)
                                                    .GetAwaiter().GetResult();



            if (response.IsSuccessStatusCode)
            {
                burgersData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            //List<nutirents> bg_;
            Items nutirents = new Items();
            var bg_ = JsonConvert.DeserializeObject<Application>(burgersData);
            if (bg_.items.Count!=0)
            {
                // JsonConvert is part of the NewtonSoft.Json Nuget package
                Items bg = bg_.items[0];
                nutirents.sugar_g = bg.sugar_g;
                nutirents.calories = bg.calories;
                nutirents.fiber_g = bg.fiber_g;
                nutirents.fat_saturated_g = bg.fat_saturated_g;
                nutirents.fat_total_g = bg.fat_total_g;
                nutirents.serving_size_g = bg.serving_size_g;
                nutirents.sodium_mg = bg.sodium_mg;
                nutirents.potassium_mg = bg.potassium_mg;
                nutirents.protein_g = bg.protein_g;
                nutirents.cholesterol_mg = bg.cholesterol_mg;
                nutirents.carbohydrates_total_g = bg.carbohydrates_total_g;
                //Initialize model and call view
            }
            else {
                nutirents.sugar_g = "---";
                nutirents.calories = "---";
                nutirents.fiber_g = "---";
                nutirents.fat_saturated_g = "---";
                nutirents.fat_total_g = "---";
                nutirents.serving_size_g = "---";
                nutirents.sodium_mg = "---";
                nutirents.potassium_mg = "---";
                nutirents.protein_g = "---";
                nutirents.cholesterol_mg = "---";
                nutirents.carbohydrates_total_g = "---";
            }
            return Json(nutirents);

        }

        [HttpGet]
        public ActionResult AboutUs()

        {
            
            ViewBag.BurgerCount = dbContext.BurgersDb.Count(); ;
            ViewBag.PizzaCount = dbContext.PizzasDb.Count(); ;
            return View();
        }
        [HttpGet]
        public ActionResult GetInfo()
        {
            return View();


        }
    }




    public class Rootobject
    {
        public Meta meta { get; set; }
        public object[][] data { get; set; }
    }

    public class Meta
    {
        public View view { get; set; }
    }

    public class View
    {
        public string id { get; set; }
        public string name { get; set; }
        public string assetType { get; set; }
        public string attribution { get; set; }
        public string attributionLink { get; set; }
        public int averageRating { get; set; }
        public string category { get; set; }
        public int createdAt { get; set; }
        public string description { get; set; }
        public string displayType { get; set; }
        public int downloadCount { get; set; }
        public bool hideFromCatalog { get; set; }
        public bool hideFromDataJson { get; set; }
        public string licenseId { get; set; }
        public bool newBackend { get; set; }
        public int numberOfComments { get; set; }
        public int oid { get; set; }
        public string provenance { get; set; }
        public bool publicationAppendEnabled { get; set; }
        public int publicationDate { get; set; }
        public int publicationGroup { get; set; }
        public string publicationStage { get; set; }
        public int rowsUpdatedAt { get; set; }
        public string rowsUpdatedBy { get; set; }
        public int tableId { get; set; }
        public int totalTimesRated { get; set; }
        public int viewCount { get; set; }
        public int viewLastModified { get; set; }
        public string viewType { get; set; }
        public Approval[] approvals { get; set; }
        public Column[] columns { get; set; }
        public Grant[] grants { get; set; }
        public License license { get; set; }
        public Metadata metadata { get; set; }
        public Owner owner { get; set; }
        public Query query { get; set; }
        public string[] rights { get; set; }
        public Tableauthor tableAuthor { get; set; }
        public string[] tags { get; set; }
        public string[] flags { get; set; }
    }

    public class License
    {
        public string name { get; set; }
        public string termsLink { get; set; }
    }

    public class Metadata
    {
        public Custom_Fields custom_fields { get; set; }
        public string[] availableDisplayTypes { get; set; }
    }

    public class Custom_Fields
    {
        public DataQuality DataQuality { get; set; }
        public CommonCore CommonCore { get; set; }
    }

    public class DataQuality
    {
        public string UpdateFrequency { get; set; }
        public string GeographicCoverage { get; set; }
    }

    public class CommonCore
    {
        public string ContactEmail { get; set; }
        public string Footnotes { get; set; }
        public string ContactName { get; set; }
        public string ProgramCode { get; set; }
        public string Publisher { get; set; }
        public string BureauCode { get; set; }
    }

    public class Owner
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string screenName { get; set; }
        public string type { get; set; }
        public string[] flags { get; set; }
    }

    public class Query
    {
    }

    public class Tableauthor
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string screenName { get; set; }
        public string type { get; set; }
        public string[] flags { get; set; }
    }

    public class Approval
    {
        public int reviewedAt { get; set; }
        public bool reviewedAutomatically { get; set; }
        public string state { get; set; }
        public int submissionId { get; set; }
        public string submissionObject { get; set; }
        public string submissionOutcome { get; set; }
        public int submittedAt { get; set; }
        public int workflowId { get; set; }
        public Submissiondetails submissionDetails { get; set; }
        public Submissionoutcomeapplication submissionOutcomeApplication { get; set; }
        public Submitter submitter { get; set; }
    }

    public class Submissiondetails
    {
        public string permissionType { get; set; }
    }

    public class Submissionoutcomeapplication
    {
        public int endedAt { get; set; }
        public int failureCount { get; set; }
        public int startedAt { get; set; }
        public string status { get; set; }
    }

    public class Submitter
    {
        public string id { get; set; }
        public string displayName { get; set; }
    }

    public class Column
    {
        public int id { get; set; }
        public string name { get; set; }
        public string dataTypeName { get; set; }
        public string fieldName { get; set; }
        public int position { get; set; }
        public string renderTypeName { get; set; }
        public Format format { get; set; }
        public string[] flags { get; set; }
        public string description { get; set; }
        public int tableColumnId { get; set; }
        public Cachedcontents cachedContents { get; set; }
    }

    public class Format
    {
        public string view { get; set; }
        public string align { get; set; }
    }

    public class Cachedcontents
    {
        public string non_null { get; set; }
        public object largest { get; set; }
        public string _null { get; set; }
        public Top[] top { get; set; }
        public object smallest { get; set; }
        public string cardinality { get; set; }
    }

    public class Top
    {
        public object item { get; set; }
        public string count { get; set; }
    }

    public class Grant
    {
        public bool inherited { get; set; }
        public string type { get; set; }
        public string[] flags { get; set; }
    }

    public class Item
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Restaurant { get; set; }
        public string Web { get; set; }
        public string Description { get; set; }
        public string[] Ingredients { get; set; }
    }
    public class nutirents
    {
        public string sugar { get; set; }
        public string fiber { get; set; }

        public string serving_size { get; set; }

        public string sodium_mg { get; set; }

        public string name { get; set; }

        public string potassium_mg { get; set; }

        public string fat_saturated_g { get; set; }

        public string fat_total_g { get; set; }

        public string calories { get; set; }

        public string cholesterol_mg { get; set; }

        public string protein_g { get; set; }

        public string carbohydrates_total_g { get; set; }




    }
    public class Items
    {
        public string sugar_g { get; set; }
        public string fiber_g { get; set; }
        public string serving_size_g { get; set; }
        public string sodium_mg { get; set; }
        public string name { get; set; }
        public string potassium_mg { get; set; }
        public string fat_saturated_g { get; set; }
        public string fat_total_g { get; set; }
        public string calories { get; set; }
        public string cholesterol_mg { get; set; }
        public string protein_g { get; set; }
        public string carbohydrates_total_g { get; set; }

    }
    public class Application
    {
        public IList<Items> items { get; set; }

    }
}


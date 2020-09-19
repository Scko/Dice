using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DiceMVC.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace DiceMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public class ProbabilityModel
        {
            public double Win { get; set; }
            public double Lose { get; set; }
            public double Tie { get; set; }
        }
        public async Task<IActionResult> IndexAsync(DiceViewModel diceVM)
        {
            if (diceVM == null) 
            {
                diceVM = new DiceViewModel();
            }
            ProbabilityModel probabilityModel = null;
            if (diceVM.dice1 != 0 && diceVM.dice2 != 0) 
            {
                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage result = await httpClient.GetAsync($"https://localhost:44393/api/Dice?Dice1={diceVM.dice1}&Dice2={diceVM.dice2}&Sides=6");
                    var responseBody = await result.Content.ReadAsStringAsync();
                    probabilityModel = JsonConvert.DeserializeObject<ProbabilityModel>(responseBody);
                }
                diceVM.Win = probabilityModel.Win;
                diceVM.Lose = probabilityModel.Lose;
                diceVM.Tie = probabilityModel.Tie;
            }
            
            return View(diceVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

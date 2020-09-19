using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DiceMVC.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace DiceMVC.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(DiceViewModel diceVM)
        {
            if (diceVM == null)
            {
                diceVM = new DiceViewModel();
            }
            ProbabilityModel probabilityModel = null;
            if (diceVM.Dice1 != 0 && diceVM.Dice2 != 0)
            {
                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage result = await httpClient.GetAsync($"https://diceservice.azurewebsites.net/api/Dice?Dice1={diceVM.Dice1}&Dice2={diceVM.Dice2}&Sides=6");
                    var responseBody = await result.Content.ReadAsStringAsync();
                    probabilityModel = JsonConvert.DeserializeObject<ProbabilityModel>(responseBody);
                }
                diceVM.Win = probabilityModel.Win;
                diceVM.Lose = probabilityModel.Lose;
                diceVM.Tie = probabilityModel.Tie;
            }

            return View(diceVM);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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

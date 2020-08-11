using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CustomTagHelpers.Models;

namespace CustomTagHelpers.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            UserInfo u = new UserInfo();
            u.Info = "Price is 100"; 
            return View(u);
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

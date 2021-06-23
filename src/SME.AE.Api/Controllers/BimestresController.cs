using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class BimestresController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

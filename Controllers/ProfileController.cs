﻿using Microsoft.AspNetCore.Mvc;

namespace Sinder.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
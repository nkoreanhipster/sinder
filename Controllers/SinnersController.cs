using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sinder.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder.Controllers
{
    public class SinnersController : Controller
    {
        // GET: SinnersController
        public async Task<IActionResult> Index()
        {
            var cookies = Request.Cookies["token"];
            if (cookies == null || cookies == "null")
                return Redirect("/login");
            string email = SecurityHelper.GetLoggedInUser(cookies);
            UserModel user = await Dataprovider.Instance.ReadUserByEmail(email);
            return View(user);
        }

        // GET: SinnersController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SinnersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SinnersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SinnersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SinnersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SinnersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SinnersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

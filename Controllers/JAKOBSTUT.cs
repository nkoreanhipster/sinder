using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sinder.Controllers
{
    public class JAKOBSTUT : Controller
    {
        // GET: JAKOBSTUT
        public ActionResult Index()
        {
            return View();
        }

        // GET: JAKOBSTUT/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: JAKOBSTUT/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: JAKOBSTUT/Create
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

        // GET: JAKOBSTUT/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: JAKOBSTUT/Edit/5
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

        // GET: JAKOBSTUT/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: JAKOBSTUT/Delete/5
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today,
                ActivityId = 2
            };
            SetupActivitiesSelectListItems();
            return View(entry);
        }

 

        [HttpPost]
        public ActionResult Add(Entry entry)
        {//if there rnt any duration field validation error then make sure duration is positive
            ValidateEntry(entry);
            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);
                return RedirectToAction("Index");
            }
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
            return View(entry);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get requested entry from repository
            Entry entry = _entriesRepository.GetEntry((int)id);
            //return a status of not found if entry wasnt found
            if (entry == null)
            {
                return HttpNotFound();
            }
            //pass entry into the view
            SetupActivitiesSelectListItems();
            return View(entry);
        }
        [HttpPost]
        public ActionResult Edit(Entry entry)
        {//validate entry
            //if entry is valid, use repository to update teh entry, redirect user 
            ValidateEntry(entry);
            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);
                return RedirectToAction("Index");
            }
            //entries to list page, populate the activities select list items viewbag property
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //retrieve entry for the provided id parameter value
            Entry entry = _entriesRepository.GetEntry((int)id);
            //return not found if entry wasnt found
            if (entry == null)
            {
                return HttpNotFound();
            }
            //pass entry to the view
            return View(entry);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            //delet entry, 
            _entriesRepository.DeleteEntry(id);
            //redirect user to the entries list page
            return RedirectToAction("Index");
        }
        private void ValidateEntry(Entry entry)
        {
            if (ModelState.IsValidField("Duration") && entry.Duration < 0)
            {
                ModelState.AddModelError("Duration", "The Duration field value must be greater den 0");
            }
        }
        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
        }
    }
}
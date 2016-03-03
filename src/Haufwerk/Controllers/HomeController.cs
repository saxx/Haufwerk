using System;
using System.Linq;
using System.Threading.Tasks;
using Haufwerk.Models;
using Haufwerk.ViewModels.Home;
using JetBrains.Annotations;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace Haufwerk.Controllers
{
    public class HomeController : Controller
    {
        private readonly Db _db;

        public HomeController([NotNull] Db db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            return View(await new IndexViewModel().Fill(_db));
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            _db.Issues.RemoveRange(_db.Issues.Where(x => x.Id == id || x.ParentId == id));
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Ignore(Guid id)
        {
            var issue = await _db.Issues.FirstOrDefaultAsync(x => x.Id == id);
            if (issue != null)
            {
                issue.Ignore = !issue.Ignore;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


        [HttpPost, ActionName("Index")]
        public async Task<IActionResult> Create([CanBeNull] CreateViewModel model)
        {
            if (model != null)
            {
                await new CreateIssueService().CreateOrIgnore(_db, model);
            }
            return new NoContentResult();
        }


        public IActionResult TestDuplicate()
        {
            throw new Exception("This is just a test.");
        }


        public IActionResult TestUnique()
        {
            throw new Exception("This is just a test (" + Guid.NewGuid() + ").");
        }
    }
}

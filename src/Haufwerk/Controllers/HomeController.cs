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
                var issue = new Issue
                {
                    CreationDateTime = DateTime.UtcNow,
                    Ignore = false,
                    Message = model.Message,
                    AdditionalInfo = model.AdditionalInfo,
                    Source = model.Source,
                    StackTrace = model.StackTrace,
                    User = model.User
                };

                // lets see if we find a duplicate
                var duplicate = await _db.Issues.FirstOrDefaultAsync(x =>
                    x.ParentId == null
                    && x.Message == issue.Message
                    && (issue.StackTrace == null || x.StackTrace == issue.StackTrace)
                );

                if (duplicate != null && duplicate.CreationDateTime >= DateTime.UtcNow.AddMinutes(-1))
                {
                    // don't log the error again if we already logged it a few seconds ago
                    return new NoContentResult();
                }
                if (duplicate != null)
                {
                    issue.ParentId = duplicate.Id;
                }

                _db.Issues.Add(issue);
                await _db.SaveChangesAsync();
            }

            return new NoContentResult();
        }

        [Route("~/error/500")]
        public IActionResult Error500()
        {
            return Redirect("~/");
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

using System;
using System.Linq;
using System.Threading.Tasks;
using Haufwerk.Client;
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
                if (duplicate != null)
                {
                    issue.ParentId = duplicate.Id;
                }

                _db.Issues.Add(issue);
                await _db.SaveChangesAsync();
            }

            return new NoContentResult();
        }


        public async Task<IActionResult> TestDuplicate()
        {
            var haufwerk = GetHaufwerkForCurrentInstance();
            await haufwerk.Post("Haufwerk Test", "This is just a test", null, "This\n  is\n  the\n  stacktrace", "And some\nAdditional info.");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> TestUnique()
        {
            var haufwerk = GetHaufwerkForCurrentInstance();
            await haufwerk.Post("Haufwerk Test", "This is just a test (" + Guid.NewGuid() + ")", null, "This\n  is\n  the\n  stacktrace", "And some\nAdditional info.");
            return RedirectToAction("Index");
        }

        private IHaufwerk GetHaufwerkForCurrentInstance()
        {
            var url = Request.Scheme + "://" + Request.Host + Url.Content("~/");
            return new Client.Haufwerk(url);
        }
    }
}

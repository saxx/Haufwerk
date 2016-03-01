using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Haufwerk.FriendlyAsyncTest;
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


        [Route("~/error/{errorCode}")]
        public IActionResult Error(int errorCode)
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


        public async Task<IActionResult> TestFriendlyAsync()
        {
            var model = "";
            try
            {
                var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(3)
                };
                await client.GetAsync("http://some_url_that_does_not_exist_112233.com/");
            }
            catch (Exception ex)
            {
                try
                {
                    model = ex.ToAsyncString();
                }
                catch (Exception innerEx)
                {
                    model = "Unable to build friendly stack trace.\n\n" + innerEx;
                }
            }

            return View("TestFriendlyAsync", model);
        }
    }
}

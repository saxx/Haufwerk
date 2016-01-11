using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Haufwerk.Models;
using JetBrains.Annotations;
using System.Linq;

namespace Haufwerk.ViewModels.Home
{
    public class IndexViewModel
    {

#pragma warning disable 1998
        public async Task<IndexViewModel> Fill([NotNull] Db db)
#pragma warning restore 1998
        {
            var issues = from issue in db.Issues
                         where issue.ParentId == null
                         select new
                         {
                             Issue = issue,
                             // for some reason, Count() does not work. Probably a bug, this workaround is quite heavy, but works fine.
                             DuplicatesCount = db.Issues.Where(x => x.ParentId == issue.Id || x.Id == issue.Id).Select(x => x.Id).ToList(),
                             DuplicatesDate = db.Issues.Where(x => x.ParentId == issue.Id || x.Id == issue.Id).Max(x => x.CreationDateTime)
                         };

            Issues = new List<Issue>();
            foreach (var i in issues)
            {
                var issue = new Issue
                {
                    Id = i.Issue.Id,
                    StackTrace = i.Issue.StackTrace,
                    Message = i.Issue.Message,
                    CountWithDuplicates = i.DuplicatesCount.Count,
                    CreationDateTimeWithDuplicates = i.DuplicatesDate,
                    Ignore = i.Issue.Ignore,
                    AdditionalInfo = i.Issue.AdditionalInfo,
                    Source = i.Issue.Source,
                    User = i.Issue.User,
                    FormattedCreationDateTimeWithDuplicates = i.DuplicatesDate.ToString("dd-MMM, hh:mm:ss")
                };

                if (issue.CreationDateTimeWithDuplicates.Date == DateTime.UtcNow.Date)
                {
                    var differenceInSeconds = (DateTime.UtcNow - issue.CreationDateTimeWithDuplicates).TotalSeconds;
                    if (differenceInSeconds < 10)
                    {
                        issue.FormattedCreationDateTimeWithDuplicates = "just now";
                    }
                    else if (differenceInSeconds < 120)
                    {
                        issue.FormattedCreationDateTimeWithDuplicates = differenceInSeconds.ToString("####") + "s ago";
                    }
                    else if (differenceInSeconds < 60*120)
                    {
                        issue.FormattedCreationDateTimeWithDuplicates = (differenceInSeconds / 60).ToString("####") + "m ago";
                    }
                    else
                    {
                        issue.FormattedCreationDateTimeWithDuplicates = i.DuplicatesDate.ToString("hh:mm:ss");
                    }
                }


                Issues.Add(issue);
            }

            Issues = Issues.OrderByDescending(x => x.CreationDateTimeWithDuplicates).ToList();
            return this;
        }

        public IList<Issue> Issues { get; set; }

        public class Issue
        {
            public Guid Id { get; set; }
            public string Message { get; set; }
            public string Source { get; set; }
            public string User { get; set; }
            public string StackTrace { get; set; }
            public string AdditionalInfo { get; set; }
            public bool Ignore { get; set; }
            public int CountWithDuplicates { get; set; }
            public DateTime CreationDateTimeWithDuplicates { get; set; }
            public string FormattedCreationDateTimeWithDuplicates { get; set; }
        }
    }
}

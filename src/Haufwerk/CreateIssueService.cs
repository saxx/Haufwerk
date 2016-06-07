using System;
using System.Linq;
using System.Threading.Tasks;
using Haufwerk.Models;
using Haufwerk.ViewModels.Home;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Haufwerk
{
    public class CreateIssueService
    {

        public async Task CreateOrIgnore([NotNull] Db db, [NotNull] CreateViewModel model)
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

            var duplicate = await FindDuplicate(db, issue);
            if (duplicate == null)
            {
                db.Issues.Add(issue);
                await db.SaveChangesAsync();
                return;
            }

            if (duplicate.CreationDateTime < DateTime.UtcNow.AddMinutes(-1) || (duplicate.StackTrace ?? "").Length != (issue.StackTrace ?? "").Length)
            {
                // if the duplicates stacktrace contains less info than the new stacktrace, switch the stacktraces so that the parent has the "best" stacktrace
                if ((issue.StackTrace ?? "").Length > (duplicate.StackTrace ?? "").Length)
                {
                    var s = duplicate.StackTrace;
                    duplicate.StackTrace = issue.StackTrace;
                    issue.StackTrace = s;
                }

                issue.ParentId = duplicate.Id;
                db.Issues.Add(issue);
                await db.SaveChangesAsync();
            }
        }


        [ItemCanBeNull]
        private async Task<Issue> FindDuplicate([NotNull] Db db, [NotNull] Issue issue)
        {
            // lets see if we find a duplicate
            var possibleDuplicates = await db.Issues.Where(x => x.ParentId == null && x.Message == issue.Message).ToListAsync();

            foreach (var duplicate in possibleDuplicates)
            {
                // if both stackstraces are empty > we got a duplicate
                if (string.IsNullOrWhiteSpace(duplicate.StackTrace) && string.IsNullOrWhiteSpace(issue.StackTrace))
                {
                    return duplicate;
                }
                // if only one stackstrace is empty > no duplicate
                if (string.IsNullOrWhiteSpace(duplicate.StackTrace) || string.IsNullOrWhiteSpace(issue.StackTrace))
                {
                    return null;
                }

                // identical stackstrace > duplicate
                if (duplicate.StackTrace.Equals(issue.StackTrace, StringComparison.OrdinalIgnoreCase))
                {
                    return duplicate;
                }

                // if one stacktrace is a subset of the other > duplicate
                if (duplicate.StackTrace.StartsWith(issue.StackTrace, StringComparison.OrdinalIgnoreCase) || issue.StackTrace.StartsWith(duplicate.StackTrace, StringComparison.OrdinalIgnoreCase))
                {
                    return duplicate;
                }
            }

            return null;
        }

    }
}

using System;
using JetBrains.Annotations;

namespace Haufwerk.Models
{
    public class Issue
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        [CanBeNull]
        public string Message { get; set; }
        [CanBeNull]
        public string Source { get; set; }
        [CanBeNull]
        public string User { get; set; }
        [CanBeNull]
        public string StackTrace { get; set; }
        [CanBeNull]
        public string AdditionalInfo { get; set; }
        public DateTime CreationDateTime { get; set; }
        public bool Ignore { get; set; }
    }
}

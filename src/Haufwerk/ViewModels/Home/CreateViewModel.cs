using JetBrains.Annotations;

namespace Haufwerk.ViewModels.Home
{
    public class CreateViewModel
    {
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
    }
}

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Haufwerk.Client
{
    public interface IHaufwerk
    {
        [NotNull]
        HaufwerkOptions Options { get; }

        [NotNull]
        Task Post(
            [NotNull] string message,
            [CanBeNull] string source = null,
            [CanBeNull] string user = null,
            [CanBeNull] string stackTrace = null,
            [CanBeNull] string additionalInfo = null
        );

        [NotNull]
        Task Post(
            [NotNull] string message,
            [CanBeNull] string source = null,
            [CanBeNull] string user = null,
            [CanBeNull] Exception exception = null,
            [CanBeNull] string additionalInfo = null
        );

        [NotNull]
        Task Post(
            [NotNull] Exception exception,
            [CanBeNull] string source = null,
            [CanBeNull] string user = null,
            [CanBeNull] string additionalInfo = null
        );
    }
}

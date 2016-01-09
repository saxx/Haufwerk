using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Haufwerk.Client
{
    public interface IHaufwerk
    {
        Task Post(
            [NotNull] string source,
            [NotNull] string message,
            [CanBeNull] string user = null,
            [CanBeNull] string stackTrace = null,
            [CanBeNull] string additionalInfo = null
        );

        Task Post(
            [NotNull] string source,
            [NotNull] string message,
            [CanBeNull] string user = null,
            [CanBeNull] Exception exception = null,
            [CanBeNull] string additionalInfo = null
    );
    }
}

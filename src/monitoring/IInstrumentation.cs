using System.Diagnostics;

namespace monitoring;

public interface IInstrumentation : IDisposable
{
    ActivitySource ActivitySource { get; }
}
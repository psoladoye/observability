using Microsoft.AspNetCore.Mvc;

namespace web.Controllers;

[ApiController]
[Route("[controller]")]
public class NameGenerator : ControllerBase
{
    private static readonly string[] Names = {
        "Paul", "James", "Chilly", "Lenny", "Oscar", "Bane", "Pluto", "Nash", "Gordon", "Aaron"
    };
    
    private readonly ILogger<NameGenerator> _logger;

    public NameGenerator(ILogger<NameGenerator> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<string> GetNames()
    {
        _logger.LogInformation("Executing {Method} in Controller", nameof(GetNames));
        var rng = new Random();

        return Enumerable.Range(1, 5).Select(s => Names[rng.Next(Names.Length)])
            .ToArray();
    }
}
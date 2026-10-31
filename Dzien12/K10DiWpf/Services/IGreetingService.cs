using Microsoft.Extensions.Options;

namespace K10DiWpf.Services;

public interface IGreetingService
{
    string Greet(string name, DateTime now);
}

public class ClassicGreet : IGreetingService
{
    private readonly GreetingOptions _options;

    public ClassicGreet(IOptions<GreetingOptions> options)
    {
        _options = options.Value;
    }

    public string Greet(string name, DateTime now)
    {
        string timeOfDay = _options.Language == "en"
            ? (now.Hour < 12 ? "Good morning" : "Good evening")
            : (now.Hour < 12 ? "Dzień dobry" : "Dobry wieczór");

        return $"{timeOfDay}, {_options.UserName}!";
    }
}
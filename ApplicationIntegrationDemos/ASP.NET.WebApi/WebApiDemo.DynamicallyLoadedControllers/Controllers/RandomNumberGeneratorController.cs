using Microsoft.AspNetCore.Mvc;


namespace WebApiDemo.DynamicallyLoadedControllers.Controllers;

[ApiController]
[Route("[controller]")]
public class RandomNumberGeneratorController : ControllerBase
{
    private readonly IRandomNumberGenerator _randomNumberGenerator;

    public RandomNumberGeneratorController(IRandomNumberGenerator randomNumberGenerator)
    {
        _randomNumberGenerator = randomNumberGenerator;
    }
    
    [HttpGet("random", Name = "random")]
    public object GetAllCompanies()
    {
        return _randomNumberGenerator.Generate();
    }
}
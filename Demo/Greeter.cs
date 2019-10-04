using Ambient;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Demo
{
    [Route("/"), AmbientContext]    
    public class GreeterController
    {
        public GreeterController(Greeter greeter) => Greeter = greeter;
        Greeter Greeter { get; }

        [HttpGet]
        public async Task<string> Get() => await Greeter.GreetAsync();
    }

    public class Greeter
    {
        public async Task<string> GreetAsync()
        {
            using (new Op())
            {
                Op.Trace("Started waiting");
                await Task.Delay(100, Cancellation.Token);
                Op.Trace("Ready");

                return "Hello World";
            }
        }
    }
}

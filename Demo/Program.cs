using Ambient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using static System.Console;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Op.Log.Subscribe(WriteLine);
            new WebHostBuilder()
                .UseKestrel()
                .Configure(ab => ab.UseMvc())
                .ConfigureServices(sc => sc.AddMvc())
                .ConfigureServices(sc => sc.AddScoped<Greeter>())
                .Build()
                .Run();
        }
    }
}

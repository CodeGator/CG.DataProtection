using CG.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System;

namespace CG.DataProtection.QuickStart
{
    public class TestOptions : OptionsBase
    {
        public string A { get; set; }

        [ProtectedProperty]
        public string B { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Standard config stuff .NET does for us.
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            // Standard config binding stuff .NET does for us.
            var options = new TestOptions();
            configuration.Bind(options);

            // OK, now to demonstrate:


            // Example: Protect any decorated property values.
            DataProtector.Instance().ProtectProperties(options);

            // Notice that we never touched the non-decorated property.
            Console.WriteLine($"property A is: {options.A}");

            // Notice that we protected the decorated property.
            Console.WriteLine($"property B is: {options.B}");
            
            // Example: Unprotect any protected properties.
            DataProtector.Instance().UnprotectProperties(options);

            // Notice that we never touched the non-decorated property.
            Console.WriteLine($"property A is: {options.A}");

            // Notice that we protected the decorated property.
            Console.WriteLine($"property B is: {options.B}");



            // We're done!
            Console.WriteLine("Done - press any key to exit");
            Console.ReadKey();
        }
    }
}

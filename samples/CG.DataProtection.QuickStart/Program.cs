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

        [ProtectedProperty(Optional = true)]
        public string C { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var options = new TestOptions()
            {
                A = "secret 1",
                B = "secret 2",
                C = "secret 3"
            };

            // Example: Protect any decorated property values.
            DataProtector.Instance().ProtectProperties(options);

            // Notice that we never touched the non-decorated property.
            Console.WriteLine($"property A is: {options.A}");

            // Notice that we protected the decorated property.
            Console.WriteLine($"property B is: {options.B}");

            // Notice that we protected the decorated property.
            Console.WriteLine($"property C is: {options.C}");

            // Let's unprotect C, to demonstrate the 'optional' feature.
            options.C = "plain text secret";

            // Example: Unprotect any protected properties.
            DataProtector.Instance().UnprotectProperties(options);

            // Notice that we never touched the non-decorated property.
            Console.WriteLine($"property A is: {options.A}");

            // Notice that we unprotected the decorated property.
            Console.WriteLine($"property B is: {options.B}");

            // Notice that we the optional property, since it held plain text.
            Console.WriteLine($"property C is: {options.C}");

            // We're done!
            Console.WriteLine("Done - press any key to exit");
            Console.ReadKey();
        }
    }
}

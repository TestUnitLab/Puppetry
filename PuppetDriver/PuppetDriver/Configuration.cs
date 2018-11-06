using System;
using System.Collections.Generic;

namespace Puppetry.PuppetDriver
{
    internal static class Configuration
    {
        internal static Dictionary<string, string> ProcessComandLineArguments(string[] args)
        {
            var result = new Dictionary<string, string>();

            for (var i = 0; i < args.Length; i = i + 2)
            {
                if (args[i].Length > 2 && args[i].StartsWith("-"))
                {
                    if (args.Length >= i + 2)
                        result.Add(args[i].Substring(1, args[i].Length - 1), args[i + 1]);
                    else
                        Console.WriteLine($"Parameter for {args[i].Substring(1, args[i].Length - 1)} was not provided");
                }
                else
                {
                    Console.WriteLine($"Illegal Parameter: {args[i].Substring(1, args[i].Length - 1)} was sent");
                }
            }

            return result;
        }
    }
}

using System;
using System.Reflection;

namespace ShowVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("!define PRODUCT_VERSION \"{0}\"", AssemblyVersion);
        }

        static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}
using BlogSystem.Shared.Helpers;
using Microsoft.Extensions.Configuration;

namespace DevTools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: DevTools.exe <password>");
                return;
            }

            var password = args[0];
            
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();
            var configuration = builder.Build();

            try 
            {
                AuthHelper authHelper = new AuthHelper(configuration);
                var hashedPassword = authHelper.HashPassword(password);
                Console.WriteLine($"Hashed password: {hashedPassword}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
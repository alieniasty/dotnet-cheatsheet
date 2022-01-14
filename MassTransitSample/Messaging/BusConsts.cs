namespace MassTransitSample.Messaging
{
    using System;
    using Microsoft.Extensions.Configuration;

    public static class BusConsts
    {
        private static readonly IConfigurationRoot Config;

        static BusConsts()
        {
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile($"appsettings.{environmentName}.json", false)
                .Build();
        }

        public static string Hostname => Config["BusConfiguration:Hostname"];
        public static string Username => Config["BusConfiguration:Username"];
        public static string Password => Config["BusConfiguration:Password"];
    }
}

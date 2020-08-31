﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IdentityServerCenter
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //#if RELEASE
            //             Log.Logger = new LoggerConfiguration()
            //                .MinimumLevel.Debug()
            //                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //                .MinimumLevel.Override("System", LogEventLevel.Warning)
            //                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            //                .Enrich.FromLogContext()
            //                // uncomment to write to Azure diagnostics stream
            //                .WriteTo.File(
            //                    @"C:\Log\identityserver.txt",
            //                    fileSizeLimitBytes: 1_000_000,
            //                    rollOnFileSizeLimit: true,
            //                    shared: true,
            //                    flushToDiskInterval: TimeSpan.FromSeconds(1))
            //                //.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
            //                .CreateLogger();

            //              ServiceFabricConfig.ServiceFabricProgram.MainFunc(typeof(Startup),
            //                "C:/https证书/SHA256withRSA_zwovo.xyz.pfx",
            //                "C:/https证书/证书密码.txt");
            //#else

            //IdentityServer4.Models.Message<IdentityServer4.Models.ConsentResponse> message = new IdentityServer4.Models.Message<IdentityServer4.Models.ConsentResponse>
            //    (new IdentityServer4.Models.ConsentResponse(), DateTime.Now);

            CreateHostBuilder(args).Build().Run();
//#endif
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

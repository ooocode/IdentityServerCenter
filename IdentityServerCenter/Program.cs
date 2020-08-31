// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Castle.Components.DictionaryAdapter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace IdentityServerCenter
{
    public class Test
    {
        public string UserName  { get; set; }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {


            //Console.WriteLine("UserName : " + config.UserName);
            //Console.WriteLine("IPAddress : " + config.IPAddress);
            //Console.WriteLine("Numbers : " + string.Join(", ", config.Numbers));
            //Console.WriteLine("Sum : " + config.Sum);




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

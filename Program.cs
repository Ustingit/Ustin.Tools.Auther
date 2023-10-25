using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TextCopy;

namespace Authorizer
{
	class Program
	{
		// dotnet publish -r win-x64 -c Release --self-contained
		static void Main(string[] args)
		{
			#if DEBUG
				Console.WriteLine($"Authorizer started at {DateTime.Now} .");		
			#endif
			
			using IHost host = CreateHostBuilder(args).Build();
			var settings = host.Services.GetRequiredService<IOptions<AppSettings>>();
			var authorizer = host.Services.GetRequiredService<IAuthorizer>();
			var notificationService = host.Services.GetRequiredService<INotificationService>();

			var valueToCopy = authorizer.GetAuthorizationToken(CancellationToken.None).GetAwaiter().GetResult();

			// Put the result in the clipboard
			ClipboardService.SetText(valueToCopy);
			
			//MessageBox.Show("Operation completed. Result copied to clipboard.");
			if (settings?.Value?.NotificationOnDone is true)
			{
				notificationService.TryShowSimpleNotification(new Notification
				{
					Value = valueToCopy,
					Settings = new NotificationSettings
					{
						Position = new ScreenPosition
						{
							X = 100,
							Y = 100
						}
					}
				});
			}

			#if DEBUG
				Console.WriteLine($"Authorizer ended ar {DateTime.Now} .");
			#endif
			
			Environment.Exit(0);
		}

		#region Configuration

		static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					config.SetBasePath(AppContext.BaseDirectory);
					config.AddJsonFile("appsettings.json", optional: true);
				})
				.ConfigureLogging((hostingContext, logging) =>
				{
					logging.ClearProviders();
					logging.AddConsole();
					//logging.Services.AddSingleton<ILoggerProvider, CustomNullLoggerProvider>();
				})
				.ConfigureServices((hostContext, services) =>
				{
					services.Configure<AppSettings>(hostContext.Configuration.GetSection(nameof(AppSettings)));
					services.AddSingleton<IAuthorizer, Authorizer>();
					services.AddScoped<INotificationService, WindowsNotificationService>();
				});

		#endregion
	}
}

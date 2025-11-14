using Microsoft.Extensions.Logging;
using RoomReservation_Dumadapat_IT13.Services;

namespace RoomReservation_Dumadapat_IT13
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<DatabaseService>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(1000);
                    var dbService = app.Services.GetRequiredService<DatabaseService>();
                    await dbService.SeedUsersAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error seeding users: {ex.Message}");
                }
            });

            return app;
        }
    }
}

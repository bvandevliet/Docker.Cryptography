using Docker.Cryptography.Services;

namespace Docker.Cryptography;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddSingleton<SecretKeyManager>();

        var app = builder.Build();

        app.MapControllers();

        app.Run();
    }
}
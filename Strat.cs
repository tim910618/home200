public class Startup
{
    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        Environment = env;
    }

    

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IWebHostEnvironment>(Environment);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseStaticFiles();//這裡

        // 設定 IWebHostEnvironment.WebRootPath
        var configuration = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
        env.WebRootPath = Path.Combine(env.ContentRootPath, configuration["UploadPath"]);
    }
    
    

}
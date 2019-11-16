namespace Platform.WebF

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.SpaServices
open VueCliMiddleware
open Platform.Domain.DomainServices
open Platform.Domain.Services
open Platform.Fatabase
open Platform.Fodels
open Platform.WebF

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        services.AddControllers()
        
        StartupExtensions().RegisterServices(services)
        
        services.AddSpaStaticFiles(fun opt -> opt.RootPath <- "ClientApp/dist")

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =

        app.UseHttpsRedirection() |> ignore
//        if (env.IsDevelopment()) then
        app.UseDeveloperExceptionPage() |> ignore
//        else
//            app.UseExceptionHandler("/api/System/Error") |> ignore
//            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//            app.UseHsts() |> ignore

        app.UseRouting() |> ignore
        
        app.UseAuthentication() |> ignore
        app.UseAuthorization() |> ignore
        
        app.UseSpaStaticFiles() |> ignore

        app.UseEndpoints(fun endpoints ->
            
            endpoints.MapControllers()
            let spaOptions = new SpaOptions()
            spaOptions.SourcePath <- "ClientApp"
            endpoints.MapToVueCliProxy("{*path}", spaOptions)
            ()
        ) |> ignore

    member val Configuration : IConfiguration = null with get, set

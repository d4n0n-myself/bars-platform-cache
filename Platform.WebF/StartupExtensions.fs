namespace Platform.WebF

open System
open System.IO
open System.Reflection
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Authorization.Infrastructure
open Microsoft.Extensions.DependencyInjection
open System.Runtime.CompilerServices
open System.Security.Claims
open System.Text
open Microsoft.IdentityModel.Tokens
open Microsoft.OpenApi.Models
open Platform.Domain.Common
open Platform.Domain.DomainServices
open Platform.Domain.Services
open Platform.Fatabase
open Platform.Fodels
open Platform.Services.Swagger

[<Extension>]
type StartupExtensions() =

    [<DefaultValue>]
    val mutable internal SwaggerConfigurationName: string

    member this.AddJwtAuthentication(services: IServiceCollection) =
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(fun options ->
                options.RequireHttpsMetadata <- false
                options.SaveToken <- true
                options.TokenValidationParameters <- this.MakeValidationParameters()
                )

    member this.AddPoliciesAuthorization(services: IServiceCollection) =
        services.AddAuthorization (fun options ->
            options.AddPolicy ("PlatformUser", (fun builder ->
                builder.Requirements.Add (
                    new AssertionRequirement(
                            fun context -> context.User.HasClaim(fun claim -> claim.Type = ClaimTypes.Name))
                        )
                    )
                )
            )

    member this.RegisterServices(services: IServiceCollection) =
        services.AddSingleton<ApplicationConfiguration>() |> ignore
        services.AddTransient<ApplicationDbContext>() |> ignore
        services.AddSingleton(typeof<IRepository<_>>, typeof<BaseRepository<_>>) |> ignore
        services.AddSingleton<PasswordCheckerService>() |> ignore
        services.AddSingleton<TokenService>() |> ignore
        services.AddSingleton<UserDomainService>()

    member this.RegisterSwagger(services: IServiceCollection) =
        services.AddSwaggerGen(fun c ->

            c.SwaggerDoc (this.SwaggerConfigurationName,
                         new OpenApiInfo(
                                            Title = "Platform Swagger API",
                                            Version = this.SwaggerConfigurationName
                                        ))
            c.DocumentFilter<PlatformSwaggerDocumentFilter>();
            c.AddSecurityDefinition ("Bearer",
                new OpenApiSecurityScheme(
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT in next format: Bearer *token*",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                )
            )

            let req = new OpenApiSecurityRequirement()

            req.Add (
                new OpenApiSecurityScheme(
                    Reference = new OpenApiReference(
                            Type = Nullable (ReferenceType.SecurityScheme),
                            Id = "Bearer"
                        )
                    ),
                [||]
            )
            c.AddSecurityRequirement(req)

            let assemblyName = Assembly.GetExecutingAssembly().GetName().Name
            let xmlFile = assemblyName + ".xml"
            let xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath)
    )

    member private this.MakeValidationParameters() =
        let param = new TokenValidationParameters()
        param.ValidateIssuer <- true
        param.ValidIssuer <- JwtOptions.Issuer
        param.ValidateAudience <- false
        param.ValidateLifetime <- true
        param.IssuerSigningKey <- new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtOptions.Key))
        param.ValidateIssuerSigningKey <- true
        param

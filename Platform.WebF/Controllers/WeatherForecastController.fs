namespace Platform.WebF.Controllers

open Microsoft.AspNetCore.Http
open System
open System.Collections.Generic
open System.Linq
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Platform.Fodels.Models
open Platform.Fodels.Interfaces;

[<Route("api/[controller]")>]
type WeatherForecastController(logger: ILogger<WeatherForecastController>) =
    inherit Controller()
    member private this.Logger = logger

    member private this.Summaries =
        [ "Freezing"; "Bracing"; "Chilly"; "Cool"; "Mild"; "Warm"; "Balmy"; "Hot"; "Sweltering"; "Scorching" ]

    [<HttpGet>]
    [<ProducesResponseType(StatusCodes.Status200OK)>]
    member this.Get(): IEnumerable<WeatherForecast> =
        let rng = new Random()
        Enumerable.Range(1, 5).Select(fun x -> float x).Select(
            fun index ->
                let temp =
                    new WeatherForecast(
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = this.Summaries.Item(rng.Next(this.Summaries.Length))
                        )
                (temp :> IPlatformModel).Id <- int index
                temp
        )

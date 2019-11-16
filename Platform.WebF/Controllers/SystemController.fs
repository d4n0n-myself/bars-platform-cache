namespace Platform.WebF.Controllers

open Microsoft.AspNetCore.Diagnostics
open Microsoft.AspNetCore.Mvc

[<Route("api/[controller]/[action]")>]
type SystemController() =
    inherit Controller()

    [<AcceptVerbs("GET", "POST", "PUT")>]
    member this.Error() =
        let error = this.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        let returnObj = {| Message = error.Error.Message; RequestPath = error.Path |}
        this.Conflict(returnObj)

namespace Platform.WebF.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Platform.Domain.DomainServices
open System.ComponentModel.DataAnnotations;
open Microsoft.AspNetCore.Authorization;
open Microsoft.AspNetCore.Http;
open Microsoft.AspNetCore.Mvc;
open Microsoft.CodeAnalysis.CSharp
open Platform.Domain.Common;
open Platform.Domain.DomainServices;

[<Route("api/[controller]/[action]")>]
type UsersController(domainService: UserDomainService) =
    inherit Controller()

    [<HttpGet>]
    member this.LogIn() =
        let result : OperationResult
        try 
            result = domainService.LogIn(login, password)
        with
			| System.Exception -> raise new ArgumentException()
			try
			{
				result = _userDomainService.LogIn(login, password);
			}
			catch (AuthorizationException exception)
			{
				return NotFound(new
				{
					Message = exception.Message,
					ParameterName = exception.ParameterName
				});
			}

			return result.Success ? (IActionResult) Ok(result) : Conflict(result);

    [<HttpPost>]
    member this.Register() =
        0

    [<HttpGet>]
    member this.CheckLoginUsed() =
        0

    [<HttpGet>]
    member this.CheckEmailUsed() =
        0

namespace aspnetcore3.Controllers

open System
open System.Collections.Generic
open Microsoft.AspNetCore.Mvc

[<Route("api/first")>]
type FirstController() =
    inherit Controller()

    [<HttpGet>]
    member this.Get() =
        ["x"; "z"]

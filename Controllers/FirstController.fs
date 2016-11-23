namespace aspnetcore3.Controllers

open System
open System.Collections.Generic
open Microsoft.AspNetCore.Mvc
open DomainTypes
open DomainOperations
open Persistence

[<Route("api/first")>]
// TODO Rename to TemplateController
// TODO Unit tests
type FirstController() =
    inherit Controller()

    [<HttpGet>]
    member this.Get(id: System.Guid) =
        // TODO Global exception handling to avoid try/with on every function; 
        // TODO Global logging of each function call would be nice too - create some middleware?
        // TODO json serializer to handle option in return (with null conversion)?
        try
            Logger.info ("FirstController.Get(" + id.ToString() + ")")
            DomainOperations.GetTemplate id (new TemplateRepository())
        with
        | _ as ex -> Logger.error (sprintf "%O" ex); None

    [<HttpPut>]
    member this.Create([<FromBody>]template: Template) =
        try
            Logger.info (sprintf "FirstController.Create(%O,%O)" template.Id template.Name)
            DomainOperations.CreateTemplate template (new TemplateCommandHandler())
        with
        | _ as ex -> Logger.error (sprintf "%O" ex)

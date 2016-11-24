namespace aspnetcore3.Controllers

open System
open System.Collections.Generic
open Microsoft.AspNetCore.Routing
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Routing
open DomainTypes
open DomainOperations.Template
open Persistence

[<Route("api/template")>]
// TODO Unit tests
type TemplateController() =
    inherit Controller()

    [<HttpGet>]
    member this.Get(id: System.Guid) : IActionResult =
        // TODO Global exception handling to avoid try/with on every function; 
        // TODO Global logging of each function call would be nice too - create some middleware?
        // TODO json serializer to handle option in return (with null conversion)?
        try
            Logger.info ("TemplateController.Get(" + id.ToString() + ")")
            match GetTemplate id (new TemplateRepository()) with //TODO DI TemplateRepository into class
            | Some template -> this.Json(template) :> IActionResult
            | None -> this.NotFound() :> IActionResult
        with
        | _ as ex -> Logger.error (sprintf "%O" ex); this.BadRequest() :> IActionResult

    [<HttpPut>]
    member this.Create([<FromBody>]template: Template) : IActionResult =
        try
            Logger.info (sprintf "TemplateController.Create(%O,%O)" template.Id template.Name)
            CreateTemplate template (new TemplateCommandHandler())

            let url = new UrlActionContext (Controller = "Template", Action = "Get", Values = new RouteValueDictionary(dict [("id", box template.Id)]))
            this.Created((this.Url.Action url), "") :> IActionResult
        with
        | _ as ex -> Logger.error (sprintf "%O" ex); this.BadRequest() :> IActionResult

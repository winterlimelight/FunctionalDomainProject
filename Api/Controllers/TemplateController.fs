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
        match GetTemplate id (new TemplateRepository()) with //TODO DI TemplateRepository into class
        | Some template -> this.Json(template) :> IActionResult
        | None -> this.NotFound() :> IActionResult

    [<HttpPut>]
    member this.Create([<FromBody>]template: Template) : IActionResult =       
        CreateTemplate template (new TemplateCommandHandler())
        let url = new UrlActionContext (Controller = "Template", Action = "Get", Values = new RouteValueDictionary(dict [("id", box template.Id)]))
        this.Created((this.Url.Action url), "") :> IActionResult
        

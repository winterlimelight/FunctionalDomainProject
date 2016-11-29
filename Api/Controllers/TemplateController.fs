namespace aspnetcore3.Controllers

open System
open System.Collections.Generic
open Microsoft.AspNetCore.Routing
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Routing
open DomainTypes
open DomainOperations.Template
open Commands.Template
open Persistence

[<Route("api/template")>]
// TODO Unit tests
type TemplateController() =
    inherit Controller()

    [<HttpGet>]
    member this.Get(id: System.Guid) : IActionResult =
        match GetTemplate id (new TemplateRepository()) with //TODO inject TemplateRepository
        | Some template -> this.Json(template) :> IActionResult
        | None -> this.NotFound() :> IActionResult

    [<HttpPut>]
    member this.Create([<FromBody>]template: Template) : IActionResult =       
        let cmd = TemplateCommand.Create(template)
        TemplateCommandHandler.Execute cmd (new TemplateWriteRepository()) //TODO inject TemplateCommandHandler, TemplateWriteRepository
        let url = new UrlActionContext (Controller = "Template", Action = "Get", Values = new RouteValueDictionary(dict [("id", box template.Id)]))
        this.Created((this.Url.Action url), "") :> IActionResult
        

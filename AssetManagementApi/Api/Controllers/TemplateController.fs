namespace AssetManagementApi.Api.Controllers

open System
open System.Collections.Generic
open Microsoft.AspNetCore.Routing
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Routing

open AssetManagementApi
open AssetManagementApi.DomainTypes
open AssetManagementApi.DomainOperations.Template
open AssetManagementApi.Commands.Template
open AssetManagementApi.Persistence
open AssetManagementApi.Railway

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

        let result = TemplateCommandHandler.Execute cmd (new TemplateWriteRepository()) //TODO inject TemplateCommandHandler, TemplateWriteRepository
        match result with
        | Failure (err: TemplateCommandError) ->
            Logger.warn (sprintf "TemplateController.Create bad request: %O" err)
            this.BadRequest(err) :> IActionResult 
        | Success _ ->
            let url = new UrlActionContext (Controller = "Template", Action = "Get", Values = new RouteValueDictionary(dict [("id", box template.Id)]))
            this.Created((this.Url.Action url), "") :> IActionResult


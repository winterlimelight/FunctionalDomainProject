namespace AmApi.Api.Controllers

open System

(*open System.Collections.Generic
open Microsoft.AspNetCore.Routing
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Routing

open AmApi
open AmApi.DomainTypes
open AmApi.DomainOperations.Asset
open AmApi.Commands.Asset
open AmApi.Persistence
open AmApi.Railway

[<Route("api/asset")>]

type AssetController() =
    inherit Controller()

    [<HttpGet>]
    member this.Get(id: System.Guid) : IActionResult =
        match GetAsset id (new AssetReadRepository()) with //TODO inject AssetReadRepository
        | Some asset -> this.Json(asset) :> IActionResult
        | None -> this.NotFound() :> IActionResult

    [<HttpPut>]
    member this.Create([<FromBody>]asset: Asset) : IActionResult =       
        let cmd = AssetCommand.Create(asset)

        let result = AssetCommandHandler.Execute cmd (new AssetWriteRepository()) (new TemplateReadRepository()) //TODO inject AssetCommandHandler, AssetWriteRepository, TemplateReadRepository
        match result with
        | Failure (err: AssetCommandError) ->
            Logger.warn (sprintf "AssetController.Create error: %A" err)
            match err with
            | DuplicateId -> this.StatusCode(409) :> IActionResult
            | _ -> this.BadRequest(err) :> IActionResult 
        | Success _ ->
            let url = new UrlActionContext (Controller = "Asset", Action = "Get", Values = new RouteValueDictionary(dict [("id", box asset.Id)]))
            this.Created((this.Url.Action url), "") :> IActionResult

*)
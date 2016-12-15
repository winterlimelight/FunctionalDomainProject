module AmApi.Api.Asset

open System
open Suave
open Suave.Successful
open Suave.RequestErrors

open AmApi
open AmApi.Util
open AmApi.Railway
open AmApi.Commands.Asset
open AmApi.Operations.Asset
open AmApi.Persistence

let private saveAsset cmd onSuccess =
    let result = AssetCommandHandler.Execute cmd (new AssetWriteRepository()) (new TemplateReadRepository())//TODO inject AssetCommandHandler, TemplateReadRepository - args? or should controllers have access to a resolution context (global)?
    match result with
    | Failure (err: AssetCommandError) ->
        Logger.Warn (sprintf "AssetController.Create error: %A" err)
        match err with
        | DuplicateId -> CONFLICT (sprintf "%A" err)
        | NotFound -> NOT_FOUND ""
        | _ -> BAD_REQUEST (sprintf "%A" err)
    | Success _ -> onSuccess

let createAsset (asset:DomainTypes.Asset) =
    saveAsset (AssetCommand.Create(asset)) 
    <| CREATED (sprintf Path.Assets.assetById (string asset.Id))

let updateAsset (asset:DomainTypes.Asset) (id:Guid) =
    if asset.Id <> id then 
        (BAD_REQUEST "Url parameter and asset id must match")
    else
        saveAsset (AssetCommand.Update(asset)) 
        <| OK ""

let getAsset (id:Guid) =
    match GetAsset id (new AssetReadRepository()) with //TODO inject AssetReadRepository
    | Some asset -> Common.jsonResponse asset
    | None -> NOT_FOUND ""
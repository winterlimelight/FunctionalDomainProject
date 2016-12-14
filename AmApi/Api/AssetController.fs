module AmApi.Api.Asset

open System
open Suave
open Suave.Successful
open Suave.RequestErrors

open AmApi.Util
open AmApi.Railway
open AmApi.Commands.Asset
open AmApi.Operations.Asset
open AmApi.Persistence

let createAsset asset =
    let cmd = AssetCommand.Create(asset)
    let result = AssetCommandHandler.Execute cmd (new AssetWriteRepository()) (new TemplateReadRepository())//TODO inject AssetCommandHandler, TemplateReadRepository - args? or should controllers have access to a resolution context (global)?

    match result with
    | Failure (err: AssetCommandError) ->
        Logger.Warn (sprintf "AssetController.Create error: %A" err)
        match err with
        | DuplicateId -> CONFLICT (sprintf "%A" err)
        | _ -> BAD_REQUEST (sprintf "%A" err)
    | Success _ ->
        CREATED (sprintf Path.Assets.assetById (string asset.Id))

let getAsset (id:Guid) =
    match GetAsset id (new AssetReadRepository()) with //TODO inject AssetReadRepository
    | Some asset -> Common.jsonResponse asset
    | None -> NOT_FOUND ""
﻿module AmApi.Api.Template

open System
open Suave
open Suave.Successful
open Suave.RequestErrors

open AmApi.Util
open AmApi.Railway
open AmApi.Commands.Template
open AmApi.Operations.Template
open AmApi.Persistence

let createTemplate template =
    let cmd = TemplateCommand.Create(template)
    let result = TemplateCommandHandler.Execute cmd (new TemplateWriteRepository()) //TODO inject TemplateCommandHandler, TemplateWriteRepository - args? or should controllers have access to a resolution context (global)?

    match result with
    | Failure (err: TemplateCommandError) ->
        Logger.Warn (sprintf "TemplateController.Create error: %A" err)
        match err with
        | DuplicateId -> CONFLICT (sprintf "%A" err)
        | _ -> BAD_REQUEST (sprintf "%A" err)
    | Success _ ->
        CREATED (sprintf Path.Assets.templateById (string template.Id))

let getTemplate (id:Guid) =
    match GetTemplate id (new TemplateReadRepository()) with //TODO inject TemplateReadRepository
    | Some template -> Common.jsonResponse template
    | None -> NOT_FOUND ""
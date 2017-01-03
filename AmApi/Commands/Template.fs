module AmApi.Commands.Template

open AmApi
open AmApi.Railway

type TemplateCommand =
    | Create of DomainTypes.Template
    | Update of DomainTypes.Template

type TemplateCommandError =
    | InvalidTemplate of string
    | DuplicateId

let IsValid (template: DomainTypes.Template) =
    match template with
    | { Fields = f } when isNull (box f) -> Failure (InvalidTemplate "Template must include a list")
    | { Fields = [] } -> Failure (InvalidTemplate "Template may not have an empty list")
    | { Id = id } when id = System.Guid.Empty -> Failure (InvalidTemplate "Template must have an Id")
    | _ -> Success ()
    //TODO validate field ids (must be globally unique) - will require template read repo.

let Execute findById saveTemplate (cmd: TemplateCommand) =
    match cmd with

    | Create(template) ->
        railway {
            do! IsValid template

            let foundTemplate = findById template.Id
            let! isDuplicate =
                match foundTemplate with 
                | Some _ -> Failure DuplicateId
                | None -> Success ()

            saveTemplate template
            return! Success ()
        }

    | Update(template) -> Success ()
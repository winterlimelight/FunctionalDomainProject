module Commands.Template

open Railway
open DomainInterfaces

type TemplateCommand =
    | Create of DomainTypes.Template
    | Update of DomainTypes.Template

type TemplateCommandError =
    | InvalidTemplate of string
    | DuplicateId

type ITemplateCommandHandler =
    abstract Execute: TemplateCommand -> ITemplateWriteRepository -> Result<unit,TemplateCommandError>

let IsValid (template: DomainTypes.Template) =
    match template with
    | { Fields = f } when (Util.isNull f) -> Failure (InvalidTemplate "Template must include a list")
    | { Fields = [] } -> Failure (InvalidTemplate "Template may not have an empty list")
    | { Id = id } when id = System.Guid.Empty -> Failure (InvalidTemplate "Template must have an Id")
    | _ -> Success ()

let TemplateCommandHandler = {
    new ITemplateCommandHandler with
        member this.Execute (cmd: TemplateCommand) (repo: ITemplateWriteRepository) =
            match cmd with

            | Create(template) ->
                railway {
                    do! IsValid template

                    let foundTemplate = repo.FindById template.Id
                    let! isDuplicate =
                        match foundTemplate with 
                        | Some _ -> Failure DuplicateId
                        | None -> Success ()

                    repo.Save template
                    return! Success ()
                }

            | Update(template) -> Success ()
}
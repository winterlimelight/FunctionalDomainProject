module Commands.Template

open DomainInterfaces

type TemplateCommand =
    | Create of DomainTypes.Template
    | Update of DomainTypes.Template

exception InvalidTemplate of string
exception DuplicateId

type ITemplateCommandHandler =
    abstract Execute: TemplateCommand -> ITemplateWriteRepository -> unit

let IsValid (template: DomainTypes.Template) =
    match template with
    | { Fields = f } when (Util.isNull f) -> (false, "Template must include a list")
    | { Fields = [] } -> (false, "Template may not have an empty list")
    | { Id = id } when id = System.Guid.Empty -> (false, "Template must have an Id")
    | _ -> (true, "")

let TemplateCommandHandler = {
    new ITemplateCommandHandler with
        member this.Execute (cmd: TemplateCommand) (repo: ITemplateWriteRepository) =
            match cmd with

            | Create(template) ->

                match IsValid template with
                | (false, reason) -> 
                    Logger.warn reason
                    raise (InvalidTemplate reason)
                | _ ->
                    match repo.FindById template.Id with
                    | Some _ -> 
                        Logger.warn (sprintf "Attempt to create template with existing id %O" template.Id)
                        raise (DuplicateId)
                    | None ->
                        repo.Save template

            | Update(template) -> ()
}
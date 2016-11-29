module Commands.Template

open DomainInterfaces

type TemplateCommand =
    | Create of DomainTypes.Template
    | Update of DomainTypes.Template

type ITemplateCommandHandler =
    abstract Execute: TemplateCommand -> ITemplateWriteRepository -> unit

let IsValid (template: DomainTypes.Template) =
    match template with
    | { Fields = f } when (Util.isNull f) -> Logger.warn "Template must include a list"; false;
    | { Fields = [] } -> Logger.warn "Template may not have an empty list"; false
    | _ -> true
    //TODO must have id

let TemplateCommandHandler = {
    new ITemplateCommandHandler with
        member this.Execute (cmd: TemplateCommand) (repo: ITemplateWriteRepository) =
            match cmd with
            | Create(template) ->
                if not (IsValid template) then failwith "Invalid template" // TODO typed exceptions
                //TODO test if id exists
                repo.Save template
            | Update(template) -> ()
}
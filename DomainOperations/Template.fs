namespace DomainOperations

open DomainTypes
open DomainInterfaces

module Template =

    let IsValid (template: Template) =
        match template with
        | { Fields = f } when (Util.isNull f) -> Logger.warn "Template must include a list"; false;
        | { Fields = [] } -> Logger.warn "Template may not have an empty list"; false
        | _ -> true


    let GetTemplate (id: System.Guid) (templateRepository: ITemplateRepository) =
        templateRepository.FindById id

    let CreateTemplate (template: Template) (templateCommandHandler: ITemplateCommandHandler) =
        if not (IsValid template) then failwith "Invalid template" // TODO typed exceptions
        
        templateCommandHandler.Save template


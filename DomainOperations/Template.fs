namespace DomainOperations

open DomainTypes
open DomainInterfaces

module Template =

    let IsValid (template: Template) =
        true

    let GetTemplate (id: System.Guid) (templateRepository: ITemplateRepository) =
        templateRepository.FindById id

    let CreateTemplate (template: Template) (templateCommandHandler: ITemplateCommandHandler) =
        if not (IsValid template) then failwith "Invalid template" // TODO typed exceptions
        
        templateCommandHandler.Save template


module DomainOperations

open DomainTypes
open DomainInterfaces

let GetTemplate (id: System.Guid) (templateRepository: ITemplateRepository) =
    templateRepository.FindById id

let CreateTemplate (template: Template) (templateCommandHandler: ITemplateCommandHandler) =
    templateCommandHandler.Save template

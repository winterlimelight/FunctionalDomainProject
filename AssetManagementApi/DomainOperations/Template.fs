namespace DomainOperations

open DomainTypes
open DomainInterfaces

module Template =

    let GetTemplate (id: System.Guid) (templateRepository: ITemplateReadRepository) =
        templateRepository.FindById id


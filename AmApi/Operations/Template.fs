namespace AmApi.Operations

open AmApi.DomainTypes
open AmApi.DomainInterfaces

module Template =

    let GetTemplate (id: System.Guid) (templateRepository: ITemplateReadRepository) =
        templateRepository.FindById id


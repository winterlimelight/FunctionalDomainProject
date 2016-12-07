namespace AssetManagementApi.DomainOperations

open AssetManagementApi.DomainTypes
open AssetManagementApi.DomainInterfaces

module Template =

    let GetTemplate (id: System.Guid) (templateRepository: ITemplateReadRepository) =
        templateRepository.FindById id


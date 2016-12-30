namespace AmApi.Operations

open AmApi.DomainTypes
open AmApi.DomainInterfaces

module Template =
    let GetTemplate (findById:FindTemplateById) id = 
        findById id
    
    //let GetTemplate (id: System.Guid) (templateRepository: ITemplateReadRepository) =
    //    templateRepository.FindById id


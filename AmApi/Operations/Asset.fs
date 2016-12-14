namespace AmApi.Operations

open AmApi.DomainTypes
open AmApi.DomainInterfaces

module Asset =

    let GetAsset (id: System.Guid) (assetRepository: IAssetReadRepository) =
        assetRepository.FindById id

    //TODO GetAssetByName... etc.


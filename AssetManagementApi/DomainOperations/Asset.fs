namespace AssetManagementApi.DomainOperations

open AssetManagementApi.DomainTypes
open AssetManagementApi.DomainInterfaces

module Asset =

    let GetAsset (id: System.Guid) (assetRepository: IAssetReadRepository) =
        assetRepository.FindById id

    //TODO GetAssetByName... etc.


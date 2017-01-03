namespace AmApi.Operations

open AmApi.DomainTypes

module Asset =
    let GetAsset findById (id: System.Guid) =
        findById id


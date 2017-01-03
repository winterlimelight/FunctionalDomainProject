module AmApi.Persistence.Asset

open AmApi.DomainTypes

let mutable fakeAssetStore = new System.Collections.Generic.Dictionary<System.Guid, Asset>()

module AssetReadRepo =
    let findById dc id =
        if fakeAssetStore.ContainsKey(id) then Some(fakeAssetStore.[id]) else None
           
module AssetWriteRepo =
    let save dc (asset: Asset) =
        if fakeAssetStore.ContainsKey asset.Id then 
            fakeAssetStore.Remove asset.Id |> ignore // our lazy fakeStore 'Update'
        fakeAssetStore.Add(asset.Id, asset)


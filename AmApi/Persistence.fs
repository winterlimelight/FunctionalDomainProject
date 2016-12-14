module AmApi.Persistence

open DomainInterfaces
open DomainTypes

let mutable fakeTemplateStore = new System.Collections.Generic.Dictionary<System.Guid, Template>()
let mutable fakeAssetStore = new System.Collections.Generic.Dictionary<System.Guid, Asset>()

type TemplateReadRepository() =
    interface ITemplateReadRepository with
        member this.FindById id =
            if fakeTemplateStore.ContainsKey(id) then Some(fakeTemplateStore.[id]) else None

            
type TemplateWriteRepository() =
    interface ITemplateWriteRepository with
        member this.FindById id =
            if fakeTemplateStore.ContainsKey(id) then Some(fakeTemplateStore.[id]) else None

        member this.Save (template: Template) =
            if fakeTemplateStore.ContainsKey template.Id then 
                fakeTemplateStore.Remove template.Id |> ignore // our lazy fakeStore 'Update'
            fakeTemplateStore.Add(template.Id, template)


type AssetReadRepository() =
    interface IAssetReadRepository with
        member this.FindById id =
            if fakeAssetStore.ContainsKey(id) then Some(fakeAssetStore.[id]) else None

            
type AssetWriteRepository() =
    interface IAssetWriteRepository with
        member this.FindById id =
            if fakeAssetStore.ContainsKey(id) then Some(fakeAssetStore.[id]) else None

        member this.Save (asset: Asset) =
            if fakeAssetStore.ContainsKey asset.Id then 
                fakeAssetStore.Remove asset.Id |> ignore // our lazy fakeStore 'Update'
            fakeAssetStore.Add(asset.Id, asset)
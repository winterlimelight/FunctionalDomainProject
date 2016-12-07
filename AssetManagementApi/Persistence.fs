module AssetManagementApi.Persistence

open DomainInterfaces
open DomainTypes

let mutable fakeTemplateStore = new System.Collections.Generic.Dictionary<System.Guid, Template>()

type TemplateRepository() =
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



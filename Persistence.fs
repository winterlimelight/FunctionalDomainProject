module Persistence

open DomainInterfaces
open DomainTypes

let mutable fakeStore = new System.Collections.Generic.Dictionary<System.Guid, Template>()

type TemplateRepository() =
    interface ITemplateReadRepository with
        member this.FindById id =
            if fakeStore.ContainsKey(id) then Some(fakeStore.[id]) else None

            
type TemplateWriteRepository() =
    interface ITemplateWriteRepository with
        member this.FindById id =
            if fakeStore.ContainsKey(id) then Some(fakeStore.[id]) else None

        member this.Save (template: Template) =
            if fakeStore.ContainsKey template.Id then 
                fakeStore.Remove template.Id |> ignore // our lazy fakeStore 'Update'
            fakeStore.Add(template.Id, template)



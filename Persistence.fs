module Persistence

open DomainInterfaces
open DomainTypes

let mutable fakeStore = new System.Collections.Generic.Dictionary<System.Guid, Template>()

type TemplateRepository() =
    interface ITemplateRepository with
        member this.FindById id =
            if fakeStore.ContainsKey(id) then Some(fakeStore.[id]) else None

            

//TODO actual command pattern... i.e. pass a command tuple (cmd-enum, data) not just the template
type TemplateCommandHandler() =
    interface ITemplateCommandHandler with
        member this.Save (template: Template) =
            do fakeStore.Item(template.Id) = template



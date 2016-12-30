module AmApi.CompositionRoot

open Suave
open AmApi.DomainInterfaces

//module WriteRepos

module ReadRepos =
    let findTemplateById dc = Persistence.TemplateReadRepo.findById dc

//module Commands

module Operations =
    let getTemplate : (DbContext -> System.Guid -> DomainTypes.Template option) = AmApi.Operations.Template.GetTemplate ReadRepos.findTemplateById

module ApiMethods =
    let getTemplate dc = Api.Template.getTemplate (Operations.getTemplate dc)

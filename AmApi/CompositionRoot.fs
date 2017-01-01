module AmApi.CompositionRoot

open Suave
open AmApi.DomainInterfaces

//module Commands

module Operations =
    let getTemplate dc = AmApi.Operations.Template.GetTemplate (Persistence.TemplateReadRepo.findById dc)

module ApiMethods =
    let getTemplate dc = Api.Template.getTemplate (Operations.getTemplate dc)

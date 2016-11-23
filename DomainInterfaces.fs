module DomainInterfaces

open DomainTypes

type ITemplateRepository =
    abstract member FindById: System.Guid -> Template option

type ITemplateCommandHandler =
    abstract member Save: Template -> unit
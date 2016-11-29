module DomainInterfaces

open DomainTypes

type ITemplateReadRepository =
    abstract member FindById: System.Guid -> Template option

type ITemplateWriteRepository =
    abstract member FindById: System.Guid -> Template option
    abstract member Save: Template -> unit

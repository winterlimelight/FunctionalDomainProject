module AmApi.DomainInterfaces

open DomainTypes

type ITemplateReadRepository =
    abstract member FindById: System.Guid -> Template option

type ITemplateWriteRepository =
    abstract member FindById: System.Guid -> Template option
    abstract member Save: Template -> unit

type IAssetReadRepository =
    abstract member FindById: System.Guid -> Asset option

type IAssetWriteRepository =
    abstract member FindById: System.Guid -> Asset option
    abstract member Save: Asset -> unit
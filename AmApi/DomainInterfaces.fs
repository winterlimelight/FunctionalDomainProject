module AmApi.DomainInterfaces

open AmApi.Util
open AmApi.DomainTypes
open FSharp.Data.Sql

type Sql = SqlDataProvider<
            ConnectionString="Server=.;Database=AssetManager;Trusted_Connection=True;MultipleActiveResultSets=true", 
            DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER,
            UseOptionTypes=true>
type DbContext = Sql.dataContext

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
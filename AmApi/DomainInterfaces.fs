module AmApi.DomainInterfaces

open AmApi.Util
open AmApi.DomainTypes
open FSharp.Data.Sql

type Sql = SqlDataProvider<
            ConnectionString="Server=.;Database=AssetManager;Trusted_Connection=True;MultipleActiveResultSets=true", 
            DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER,
            UseOptionTypes=true>
type DbContext = Sql.dataContext

module RequestContext = 
    type T = {
        dc: Sql.dataContext
        logger: AmApi.Util._Logger
    }
    let create logger = { dc = Sql.GetDataContext(); logger = logger }
    let set amCtx httpContext = Suave.Writers.setUserData "amapiRequestContext" amCtx httpContext
    let get (httpContext:Suave.Http.HttpContext) = httpContext.userState.Item "amapiRequestContext" :?> T
    (* if required....
//context: (HttpContext -> WebPart) -> WebPart
let getContext f : WebPart = 
    context(fun httpContext -> 
        let amCtx = AmApi.DomainInterfaces.RequestContext.get httpContext
        f amCtx
    )
*)

// Replaced by the below, but still in use in Asset create
type ITemplateReadRepository =
    abstract member FindById: System.Guid -> Template option
// In the OO world DbContext (or factory) would have come in with the repo class. In the FN world, it therefore becomes an arg.
type FindTemplateById = System.Guid -> Template option // domain/persistence 
type GetTemplate = System.Guid -> Template option // api/domain
// I'm thinking when all this is done we'll just be deleting the above types...



type ITemplateWriteRepository =
    abstract member FindById: System.Guid -> Template option
    abstract member Save: Template -> unit
//type FindTemplateByIdForValidation = System.Guid -> Template option
//type SaveTemplate = Template -> unit

type IAssetReadRepository =
    abstract member FindById: System.Guid -> Asset option

type IAssetWriteRepository =
    abstract member FindById: System.Guid -> Asset option
    abstract member Save: Asset -> unit
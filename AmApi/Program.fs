module AmApi.App

open System

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors
open Newtonsoft.Json

open AmApi
open AmApi.DomainInterfaces
open AmApi.Util //TODO remove?
open AmApi.CompositionRoot
open AmApi.Api //TODO remove.

let readJson<'TRequestDto> fSuccess =
    request (fun req ->
        let mutable errors = ""
        let settings = new JsonSerializerSettings(Error=(fun _ err -> 
            errors <- sprintf "%s\r\n%s" errors err.ErrorContext.Error.Message
            err.ErrorContext.Handled <- true
        ))

        let requestJsonStr = UTF8.toString req.rawForm
        let jsonObj:'TRequestDto = JsonConvert.DeserializeObject<'TRequestDto>(requestJsonStr, settings)

        match box jsonObj with
        | null -> BAD_REQUEST errors
        | _ -> fSuccess jsonObj
    )

let readGuid fSuccess id =
    let result, guid = Guid.TryParse(id)
    match result with
    | true -> fSuccess guid
    | false -> BAD_REQUEST (sprintf "%s is not a guid" id)


let route dc =
    choose [
        path Path.Assets.template >=> choose [
            PUT >=> readJson Template.createTemplate
        ]
        path Path.Assets.asset >=> choose [
            PUT >=> readJson Asset.createAsset
        ]

        pathScan Path.Assets.templateById (readGuid (ApiMethods.getTemplate dc))

        pathScan Path.Assets.assetById (fun guid -> 
            choose [
                GET >=> readGuid Asset.getAsset guid
                PUT >=> readJson (fun json -> readGuid (Asset.updateAsset json) guid)
        ])
        
        NOT_FOUND "No handler found"
    ]

let handleRequest (createRequestContext:unit -> RequestContext.T) : WebPart<HttpContext> = 
    fun httpContext -> async {
        let amCtx = createRequestContext()
        amCtx.logger.Info (sprintf "Request: %O\r\n rawForm: %s" httpContext.request.url (System.Text.Encoding.UTF8.GetString httpContext.request.rawForm))
        let setAmContext = fun _ -> RequestContext.set amCtx httpContext

        return! httpContext |> (
            setAmContext 
            >=> route amCtx.dc
        )
    }    

[<EntryPoint>]
let main argv = 

    let globalLog = _Logger()
    let createRequestContext = fun () -> RequestContext.create globalLog

    FSharp.Data.Sql.Common.QueryEvents.SqlQueryEvent.Add (fun msg -> globalLog.Debug (sprintf "Executing SQL: %s" msg)) // log SqlProvider queries
   
    let defaultLog = Suave.Logging.Targets.create Suave.Logging.LogLevel.Info
    let logger = Suave.Logging.CombiningTarget([ defaultLog; Util.SuaveLoggerAdapter() ])

    let config = { defaultConfig with 
                    bindings = [ HttpBinding.createSimple HTTP "127.0.0.1" 5000 ]
                    logger = logger
    }

    globalLog.Info "Starting server"
    startWebServer config (handleRequest createRequestContext)
    0 // exit code

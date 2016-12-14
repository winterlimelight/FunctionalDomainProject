module AmApi.App

open System

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors
open Newtonsoft.Json

open AmApi
open AmApi.Util
open AmApi.Api


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
    

let route =
    choose [
        path Path.Assets.template >=> choose [
            PUT >=> readJson Template.createTemplate
        ]
        path Path.Assets.asset >=> choose [
            PUT >=> readJson Asset.createAsset
        ]
        pathScan Path.Assets.templateById (readGuid Template.getTemplate)
        pathScan Path.Assets.assetById (readGuid Asset.getAsset)
        NOT_FOUND "No handler found"
    ]

let handleRequest:WebPart<HttpContext> = 
    Logger.Info "Starting server"

    fun httpContext -> async {
        Logger.Info (sprintf "Request: %O\r\n rawForm: %s" httpContext.request.url (System.Text.Encoding.UTF8.GetString httpContext.request.rawForm))
        let resultContext = route httpContext
        return! resultContext
    }    

[<EntryPoint>]
let main argv = 
   
    let defaultLog = Suave.Logging.Targets.create Suave.Logging.LogLevel.Info
    let logger = Suave.Logging.CombiningTarget([ defaultLog; Util.SuaveLoggerAdapter() ])

    let config = { defaultConfig with 
                    bindings = [ HttpBinding.createSimple HTTP "127.0.0.1" 5000 ]
                    logger = logger
    }

    startWebServer config handleRequest
    0 // exit code

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
        pathScan Path.Assets.templateById (readGuid Template.getTemplate)

        path Path.Assets.template >=> choose [
                PUT >=> OK "Asset PUT"
            ]
        pathScan Path.Assets.assetById (fun guid -> OK (sprintf "Get Asset by Id: %s" guid))
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
    printfn "%A" argv

    let config = { defaultConfig with bindings = [ HttpBinding.createSimple HTTP "127.0.0.1" 5000 ]}

    startWebServer config handleRequest
    0 // return an integer exit code

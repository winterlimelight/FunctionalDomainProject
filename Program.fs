open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.Formatters.Json
open Microsoft.AspNetCore.Diagnostics
open Microsoft.Extensions.DependencyInjection
open Newtonsoft.Json.Serialization

let onJsonError (sender:obj) (args:ErrorEventArgs) =
    Logger.error (args.ToString())
    ()


type Startup(env: IHostingEnvironment) =

    member this.ConfigureServices(services: IServiceCollection) =
        let mvc = services.AddMvcCore()
        mvc.AddJsonFormatters(fun serializerSettings -> 
            serializerSettings.Error <- System.EventHandler<ErrorEventArgs>(onJsonError)
        ) |> ignore

    member this.Configure (app: IApplicationBuilder) =
        app.UseDeveloperExceptionPage() |> ignore
        app.UseMvc() |> ignore


[<EntryPoint>]
let main argv = 
    printfn "Starting"
    Logger.info "Startup"
    let host = WebHostBuilder().UseKestrel().UseContentRoot(Directory.GetCurrentDirectory()).UseStartup<Startup>().Build()
    host.Run()
    0 //exit code

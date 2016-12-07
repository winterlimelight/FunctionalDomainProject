open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.Formatters.Json
open Microsoft.AspNetCore.Diagnostics
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Newtonsoft.Json.Serialization
open AssetManagementApi
open AssetManagementApi.Api.Filters

type Startup(env: IHostingEnvironment) =

    member this.ConfigureServices(services: IServiceCollection) =
        let mvc = services.AddMvcCore()
        mvc.AddMvcOptions(fun mvcOptions -> mvcOptions.Filters.Add(new GlobalExceptionFilter())) |> ignore
        mvc.AddMvcOptions(fun mvcOptions -> mvcOptions.Filters.Add(new GeneralActionFilter())) |> ignore
        mvc.AddJsonFormatters() |> ignore

    member this.Configure (app: IApplicationBuilder, loggerFactory: ILoggerFactory) =
        loggerFactory.AddConsole().AddDebug() |> ignore
        app.UseMvc() |> ignore


[<EntryPoint>]
let main argv = 
    let mvcRoot = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Api")
    Logger.info ("Starting. mvcRoot = " + mvcRoot)
    let host = WebHostBuilder().UseKestrel().UseContentRoot(mvcRoot).UseStartup<Startup>().Build()
    host.Run()
    0 //exit code

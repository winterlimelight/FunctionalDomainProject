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

type Startup(env: IHostingEnvironment) =

    member this.ConfigureServices(services: IServiceCollection) =
        let mvc = services.AddMvcCore()
        mvc.AddMvcOptions(fun mvcOptions -> mvcOptions.Filters.Add(new Api.Filters.GlobalExceptionFilter())) |> ignore
        mvc.AddMvcOptions(fun mvcOptions -> mvcOptions.Filters.Add(new Api.Filters.GeneralActionFilter())) |> ignore
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

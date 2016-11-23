open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection

type Startup(env: IHostingEnvironment) =

    member this.ConfigureServices(services: IServiceCollection) =
        services.AddMvc() |> ignore

    member this.Configure (app: IApplicationBuilder) =
        app.UseMvc() |> ignore

[<EntryPoint>]
let main argv = 
    printfn "Starting"
    Logger.info "Startup"
    let host = WebHostBuilder().UseKestrel().UseContentRoot(Directory.GetCurrentDirectory()).UseStartup<Startup>().Build()
    host.Run()
    0 //exit code

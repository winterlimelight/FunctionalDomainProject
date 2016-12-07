module AssetManagementApi.Api.Filters

open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters
open Microsoft.AspNetCore.Mvc.ModelBinding
open AssetManagementApi

type GlobalExceptionFilter() =
    interface IExceptionFilter with
        member this.OnException (context: ExceptionContext) =
            Logger.error (sprintf "%O" context.Exception)

type GeneralActionFilter() = 
    interface IActionFilter with

        member this.OnActionExecuting (context: ActionExecutingContext) =
            if not context.ModelState.IsValid then
                let errors = 
                    context.ModelState.Values 
                    |> Seq.collect (fun (value: ModelStateEntry) -> value.Errors)
                    |> Seq.map (fun (modelError: ModelError) -> sprintf "%s" modelError.Exception.Message)
                    |> String.concat "\n\t  "
                
                Logger.error (sprintf "Called %s. Error: Invalid model state\n\tException messages: \n\t  %s" context.ActionDescriptor.DisplayName errors)
                context.Result <- new BadRequestObjectResult(context.ModelState)
            else
                let args = [ for kvp in context.ActionArguments -> sprintf "%s %A" kvp.Key kvp.Value ] |> String.concat "\n\t"
                Logger.info (sprintf "Called %s with: \n\t%s" context.ActionDescriptor.DisplayName args)

        member this.OnActionExecuted (context: ActionExecutedContext) = ()
            


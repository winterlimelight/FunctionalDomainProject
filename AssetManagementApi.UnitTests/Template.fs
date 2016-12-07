module AssetManagementApi.UnitTests.Template

// warning FS0059: The type 'Railway.Result<unit,TemplateCommandError>' does not have any proper subtypes and need not be used as the target of a static coercion. 
// ignored because without the intermediate cast :> Railway.Result<unit,TemplateCommandError> :> obj then we get an exception because xUnit can't cast obj to Railway.Result<unit,TemplateCommandError>
#nowarn "59" 

open System
open Xunit
open AssetManagementApi
open AssetManagementApi.DomainInterfaces
open AssetManagementApi.Commands.Template

let basicTemplate() : DomainTypes.Template = { 
    Id = System.Guid.NewGuid()
    Name = "templateName"
    Fields = [{ Id =  System.Guid.NewGuid(); Name = "strFieldName"; Field = DomainTypes.StringField("strFieldValue") }] 
    MaintenanceProgramId = System.Guid.Empty 
}

type TemplateValidationTestData() =
        inherit Util.BaseTestData()
        override this.data = 
            Seq.ofList [
                [|
                    { basicTemplate() with Fields = [] } :> obj
                    Railway.Failure (InvalidTemplate "Template may not have an empty list") :> Railway.Result<unit,TemplateCommandError> :> obj
                |]; [|
                    { basicTemplate() with Id = System.Guid.Empty } :> obj
                    Railway.Failure (InvalidTemplate "Template must have an Id") :> Railway.Result<unit,TemplateCommandError> :> obj
                |]
            ]

type TemplateDomainOperationsTests() =

    let mockRepo = { new ITemplateWriteRepository with
        member this.FindById id = Some({basicTemplate() with Id = id})
        member this.Save (template: DomainTypes.Template) = ()
    }

    [<Theory>]
    [<ClassData(typeof<TemplateValidationTestData>)>]
    let ``Template validation`` (input: DomainTypes.Template) (expected: Railway.Result<unit,TemplateCommandError>) =

        let cmd = TemplateCommand.Create(input)
        let result : Railway.Result<unit,TemplateCommandError> = TemplateCommandHandler.Execute cmd mockRepo
        Assert.Equal(result, expected)
        ()


module AssetManagementApi.UnitTests.Template

open System
open Xunit
open AssetManagementApi
open AssetManagementApi.DomainInterfaces
open AssetManagementApi.Commands.Template

type TemplateValidationTestData() =
        inherit Util.BaseTestData()
        override this.data = 
            Seq.ofList [
                [|
                    ({ Id = System.Guid.NewGuid(); Name = "abc"; Fields = []; MaintenanceProgramId = System.Guid.Empty } : DomainTypes.Template) :> obj
                    (Railway.Failure (InvalidTemplate "Template may not have an empty list")) :> Railway.Result<unit,TemplateCommandError> :> obj
                |] 
            ]

type TemplateDomainOperationsTests() =

    let basicTemplate: DomainTypes.Template = { 
        Id = System.Guid.NewGuid()
        Name = "templateName"
        Fields = [{ Id =  System.Guid.NewGuid(); Name = "strFieldName"; Field = DomainTypes.StringField("strFieldValue") }] 
        MaintenanceProgramId = System.Guid.Empty 
    }

    let mockRepo = { new ITemplateWriteRepository with
        member this.FindById id = Some({basicTemplate with Id = id})
        member this.Save (template: DomainTypes.Template) = ()
    }

    [<Theory>]
    [<ClassData(typeof<TemplateValidationTestData>)>]
    let ``Template validation`` (input: DomainTypes.Template) (expected: Railway.Result<unit,TemplateCommandError>) =
        
        let cmd = TemplateCommand.Create(input)
        let result : Railway.Result<unit,TemplateCommandError> = TemplateCommandHandler.Execute cmd mockRepo
        Assert.Equal(result, expected)
        ()


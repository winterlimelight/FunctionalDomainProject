module AmApi.UnitTests.Template

open AmApi
open AmApi.DomainInterfaces
open AmApi.Commands.Template
open NUnit.Framework
open FsUnit

let basicTemplate() : DomainTypes.Template = { 
    Id = System.Guid.NewGuid()
    Name = "templateName"
    Fields = [{ Id =  System.Guid.NewGuid(); Name = "strFieldName"; Field = DomainTypes.StringField("strFieldValue") }] 
    MaintenanceProgramId = System.Guid.Empty 
}

let emptyRepo = { new ITemplateWriteRepository with
    member this.FindById id = None
    member this.Save (template: DomainTypes.Template) = ()
}

let templateExistsRepo = { new ITemplateWriteRepository with
    member this.FindById id = Some({basicTemplate() with Id = id})
    member this.Save (template: DomainTypes.Template) = ()
}

type CreateTemplateValidationData = { Tpl: DomainTypes.Template; Expected: Railway.Result<unit,TemplateCommandError> }
let CreateTemplateValidationData1 = [|  {
                                            Tpl = basicTemplate()
                                            Expected = Railway.Success()
                                    };  {
                                            Tpl = { basicTemplate() with Fields = [] }
                                            Expected = Railway.Failure (InvalidTemplate "Template may not have an empty list")
                                    };  {
                                            Tpl = { basicTemplate() with Id = System.Guid.Empty }
                                            Expected = Railway.Failure (InvalidTemplate "Template must have an Id")
                                    }|]

[<TestCaseSource("CreateTemplateValidationData1")>]
let ``Create template validation`` (data: CreateTemplateValidationData) =
    let cmd = TemplateCommand.Create(data.Tpl)
    let result : Railway.Result<unit,TemplateCommandError> = TemplateCommandHandler.Execute cmd emptyRepo
    result |> should equal data.Expected

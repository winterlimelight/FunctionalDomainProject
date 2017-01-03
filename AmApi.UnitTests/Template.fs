module AmApi.UnitTests.Template

open AmApi
open AmApi.Commands.Template
open NUnit.Framework
open FsUnit

let basicTemplate() : DomainTypes.Template = { 
    Id = System.Guid.NewGuid()
    Name = "templateName"
    Fields = [{ Id = System.Guid.NewGuid(); Name = "strFieldName"; Field = DomainTypes.StringField("strFieldValue") }] 
    MaintenanceProgramId = None
}

// Persistence layer stubs
let findByIdEmptyRepo id = None
let findByIdExisting id = Some({basicTemplate() with Id = id})
let saveToEmptyRepo (template: DomainTypes.Template) = ()

type CreateTemplateValidationData = { Tpl: DomainTypes.Template; Expected: Railway.Result<unit,TemplateCommandError> }
let CreateTemplateValidationData1 = 
    [|  {
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
    let result : Railway.Result<unit,TemplateCommandError> = AmApi.Commands.Template.Execute findByIdEmptyRepo saveToEmptyRepo cmd
    result |> should equal data.Expected

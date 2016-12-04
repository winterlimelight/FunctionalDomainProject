module AssetManagementApi.UnitTests.Template

open System
open Xunit

type TemplateDomainOperationsTests() =

    [<Fact>]
    let passingTest() =
        Assert.True(true)

    [<Fact>]
    let failingTest() =
        Assert.True(false)
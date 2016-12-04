module AssetManagementApi.UnitTests.Template

open System
open Xunit

[<TestFixture>]
type TemplateDomainOperationsTests() =
    [<Fact>]
    let passingTest =
        Assert.That(true, Is.True)
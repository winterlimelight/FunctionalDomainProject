module AmApi.Api.Path

type StrPath = PrintfFormat<(string -> string),unit,string,string,string>

module Assets =
    let templateById : StrPath = "/template/%s"
    let template = "/template"
    let assetById : StrPath = "/asset/%s"
    let asset = "/asset"


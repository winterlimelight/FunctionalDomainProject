module AmApi.Api.Common

open Suave
open Suave.Successful
open Suave.Operators
open Newtonsoft.Json

let jsonResponse o : WebPart = 
    let serialized = JsonConvert.SerializeObject(o)
    OK serialized >=> Writers.setMimeType "application/json; charset=utf-8"

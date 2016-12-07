module AssetManagementApi.Util
open System
open System.Collections
open System.Collections.Generic

let inline isNull (x:^T when ^T : not struct) = obj.ReferenceEquals (x, null)

[<AbstractClass>]
type BaseTestData() =
    abstract member data: seq<obj[]>
    interface IEnumerable<obj[]> with 
        member this.GetEnumerator() : IEnumerator<obj[]> = this.data.GetEnumerator()
        member this.GetEnumerator() : IEnumerator = this.data.GetEnumerator() :> IEnumerator
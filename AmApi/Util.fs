module AmApi.Util
open System
open System.Collections
open System.Collections.Generic

type _Logger() = 
    let log = new System.Diagnostics.TraceSource("Log")

    let _log (eventType:Diagnostics.TraceEventType) (msg:string) =
        log.TraceEvent(eventType, 0, (sprintf "%s: %s" (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff K")) msg))

    override this.Finalize() = 
        log.Flush()
        log.Close()

    member this.Info msg = _log Diagnostics.TraceEventType.Information msg
    member this.Warn msg = _log Diagnostics.TraceEventType.Warning msg
    member this.Error msg = _log Diagnostics.TraceEventType.Error msg

let Logger = _Logger()


[<AbstractClass>]
type BaseTestData() =
    abstract member data: seq<obj[]>
    interface IEnumerable<obj[]> with 
        member this.GetEnumerator() : IEnumerator<obj[]> = this.data.GetEnumerator()
        member this.GetEnumerator() : IEnumerator = this.data.GetEnumerator() :> IEnumerator
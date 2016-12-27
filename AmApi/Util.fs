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

    member this.Debug msg = _log Diagnostics.TraceEventType.Verbose msg
    member this.Info msg = _log Diagnostics.TraceEventType.Information msg
    member this.Warn msg = _log Diagnostics.TraceEventType.Warning msg
    member this.Error msg = _log Diagnostics.TraceEventType.Error msg

    member this.SuaveLog msg (level:Suave.Logging.LogLevel) = 
        let traceEventType = match level with
                                | Suave.Logging.LogLevel.Verbose -> Diagnostics.TraceEventType.Verbose
                                | Suave.Logging.LogLevel.Debug   -> Diagnostics.TraceEventType.Verbose
                                | Suave.Logging.LogLevel.Info    -> Diagnostics.TraceEventType.Information
                                | Suave.Logging.LogLevel.Warn    -> Diagnostics.TraceEventType.Warning
                                | Suave.Logging.LogLevel.Error   -> Diagnostics.TraceEventType.Error
                                | Suave.Logging.LogLevel.Fatal   -> Diagnostics.TraceEventType.Critical
        _log traceEventType msg

let Logger = _Logger()


type SuaveLoggerAdapter() =
    let _log (msg:Suave.Logging.Message) = 
        use strWriter = new System.IO.StringWriter()
        let txt = Suave.Logging.TextWriterTarget(Suave.Logging.LogLevel.Verbose, strWriter) :> Suave.Logging.Logger
        txt.logSimple msg
        Logger.SuaveLog (strWriter.ToString()) msg.level

    interface Suave.Logging.Logger with
        member __.logSimple msg = _log msg
        member __.log level msgFactory = _log (msgFactory level)
        member __.logWithAck level msgFactory = async { do _log (msgFactory level) }


[<AbstractClass>]
type BaseTestData() =
    abstract member data: seq<obj[]>
    interface IEnumerable<obj[]> with 
        member this.GetEnumerator() : IEnumerator<obj[]> = this.data.GetEnumerator()
        member this.GetEnumerator() : IEnumerator = this.data.GetEnumerator() :> IEnumerator
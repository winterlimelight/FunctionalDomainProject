module AmApi.Util
open System
open System.Collections
open System.Collections.Generic

type _Logger(name:string) = 
    let log = new System.Diagnostics.TraceSource(name)

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


type SuaveLoggerAdapter(logger:_Logger) =
    let suaveLogName = [||]

    let _log (msgFactory:Suave.Logging.LogLevel->Suave.Logging.Message) (level:Suave.Logging.LogLevel) =
        use strWriter = new System.IO.StringWriter()
        let txt = Suave.Logging.TextWriterTarget(suaveLogName, Suave.Logging.LogLevel.Verbose, strWriter) :> Suave.Logging.Logger
        txt.log level msgFactory |> ignore
        logger.SuaveLog (strWriter.ToString()) level

    interface Suave.Logging.Logger with
        member __.name = suaveLogName
        member __.log level msgFactory = async { do _log msgFactory level }
        member __.logWithAck level msgFactory = async { do _log msgFactory level }

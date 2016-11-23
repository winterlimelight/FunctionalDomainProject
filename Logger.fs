module Logger 

open Microsoft.Extensions.Logging

let private loggerFactory = (new LoggerFactory()).AddConsole().AddDebug()
let private logger = loggerFactory.CreateLogger "GlobalLogger"

let info = logger.LogInformation
let error = logger.LogError
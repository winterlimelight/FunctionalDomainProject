module Logger 

open Microsoft.Extensions.Logging

let private loggerFactory = (new LoggerFactory()).AddConsole().AddDebug()
let private logger = loggerFactory.CreateLogger "FunctionalDomainProject-Logger"

let info = logger.LogInformation
let warn = logger.LogWarning
let error = logger.LogError
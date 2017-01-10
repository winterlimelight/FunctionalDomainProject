module AmApi.Store

open FSharp.Data.Sql

[<Literal>]
let resPath = __SOURCE_DIRECTORY__ + @"../../packages/MySql.Data/lib/net45"

type Sql = SqlDataProvider<
            ConnectionString="Server=192.168.56.101;Database=AssetManager;User=mysql1;Password=mypass1", 
            DatabaseVendor=Common.DatabaseProviderTypes.MYSQL,
            ResolutionPath = resPath,
            UseOptionTypes=true>
type DbContext = Sql.dataContext
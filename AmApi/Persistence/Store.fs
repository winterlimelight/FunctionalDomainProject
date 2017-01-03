module AmApi.Store

open FSharp.Data.Sql

type Sql = SqlDataProvider<
            ConnectionString="Server=.;Database=AssetManager;Trusted_Connection=True;MultipleActiveResultSets=true", 
            DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER,
            UseOptionTypes=true>
type DbContext = Sql.dataContext
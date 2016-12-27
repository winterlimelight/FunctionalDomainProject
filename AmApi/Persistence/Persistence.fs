module AmApi.Persistence

open System.Transactions
open FSharp.Data.Sql
open Microsoft.FSharp.Linq
open FSharpx.Option
open DomainInterfaces
open DomainTypes

//let mutable fakeTemplateStore = new System.Collections.Generic.Dictionary<System.Guid, Template>()
let mutable fakeAssetStore = new System.Collections.Generic.Dictionary<System.Guid, Asset>()

type private TemplateQueryResultSet = DbContext.``dbo.TemplateEntity`` * DbContext.``dbo.FieldDefinitionEntity`` * DbContext.``dbo.FieldValueEntity``

let private mapSingleTemplate (rows:TemplateQueryResultSet list) : Template =
    let fields = [ for row in rows do
                    let (_, defn, value) = row
                    if defn.FieldDefinitionId <> System.Guid.Empty then // empty guid means template has no fields
                        yield {
                            Id = defn.FieldDefinitionId
                            Name = defn.Name
                            Field = match value.ValueType with
                                    | 1uy -> StringField(value.StringValue.Value)
                                    | 2uy -> DateField(value.DateValue.Value)
                                    | 3uy -> NumericField(float value.NumericValue.Value)
                                    | _ -> failwith "Unknown field type"
                        }]
    let (templateCols, _, _) = rows.Head
    {
        Id = templateCols.TemplateId
        Name = templateCols.Name
        Fields = fields
        MaintenanceProgramId = templateCols.MaintenanceProgramId
    }

let private templateByIdQuery (dc:DbContext) id : System.Linq.IQueryable<TemplateQueryResultSet> =
    query { 
        for template in dc.Dbo.Template do
        // (!!) means left outer join
        for fieldDef in (!!) template.``dbo.FieldDefinition by TemplateId`` do
        for fieldVal in (!!) fieldDef.``dbo.FieldValue by FieldDefinitionId`` do
        where (template.TemplateId = id && fieldVal.AssetId.IsNone)
        select (template, fieldDef, fieldVal)
    }

let private templateById id : Template option =
    let dc:DbContext = Sql.GetDataContext()  //TODO pass-in
    let query = templateByIdQuery dc id
    let rows = Seq.toList query
    if [] = rows then None else Some (mapSingleTemplate rows)

type TemplateReadRepository() =
    interface ITemplateReadRepository with
        member this.FindById id = templateById id
            
type TemplateWriteRepository() =
    interface ITemplateWriteRepository with
        member this.FindById id = templateById id
        member this.Save (template: Template) =
            let dc:DbContext = Sql.GetDataContext()  //TODO pass-in
            ()
            // UP TO HERE - we tested get-template works ok with existing data - now we want to create first, then test on that.
            (*use scope = new TransactionScope()

            // delete using the find query (this would be problem if our FKs had delete integrity)

            // then add
            
            let newInvoice = ctx.Dbo.xxx.Create(customerId, invoiceDate)    
            ctx.SubmitUpdates()

            ctx.Dbo.xxx.Create(amount, description, newInvoice.InvoiceId)
            ctx.SubmitUpdates()

            scope.Complete()  //Everything is OK
            *)


type AssetReadRepository() =
    interface IAssetReadRepository with
        member this.FindById id =
            if fakeAssetStore.ContainsKey(id) then Some(fakeAssetStore.[id]) else None

            
type AssetWriteRepository() =
    interface IAssetWriteRepository with
        member this.FindById id =
            if fakeAssetStore.ContainsKey(id) then Some(fakeAssetStore.[id]) else None

        member this.Save (asset: Asset) =
            if fakeAssetStore.ContainsKey asset.Id then 
                fakeAssetStore.Remove asset.Id |> ignore // our lazy fakeStore 'Update'
            fakeAssetStore.Add(asset.Id, asset)
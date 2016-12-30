module AmApi.Persistence

open System.Transactions
open FSharp.Data.Sql
open Microsoft.FSharp.Linq
open FSharpx.Option
open DomainInterfaces
open DomainTypes

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

let private templateById dc id : Template option =
    let rows = templateByIdQuery dc id |> Seq.toList
    if [] = rows then None else Some (mapSingleTemplate rows)


module TemplateReadRepo =
    let findById dc id = 
        templateById dc id

// Replaced by the above, but still in use in Asset create
type TemplateReadRepository() =
    interface ITemplateReadRepository with
        member this.FindById id = 
            let dc = Sql.GetDataContext()  //TODO pass-in
            templateById dc id
            
type TemplateWriteRepository() =
    interface ITemplateWriteRepository with
        member this.FindById id = 
            let dc = Sql.GetDataContext()  //TODO pass-in
            templateById dc id

        member this.Save (template: Template) =
            let dc:DbContext = Sql.GetDataContext()  //TODO pass-in
            
            // This is not great as we could make a lot of db calls within this transaction which would limit sql concurrency.
            // This might be better dealt with by a sproc.
            use scope = new TransactionScope()

            // delete using the find query (this would be problem if our FKs had delete integrity)
            let rows = templateByIdQuery dc template.Id |> Seq.toList
            if not rows.IsEmpty then
                rows |> Seq.iter (fun (_, defn, value) -> defn.Delete(); value.Delete())
                let (template, _, _) = rows.Head
                template.Delete()
                dc.SubmitUpdates()

            // add new template
            let mutable newTemplate = dc.Dbo.Template.Create(template.Name)
            newTemplate.TemplateId <- template.Id
            newTemplate.MaintenanceProgramId <- template.MaintenanceProgramId
            dc.SubmitUpdates()

            // add field definitions
            template.Fields 
            |> List.iter (fun field -> 
                let mutable newDefn = dc.Dbo.FieldDefinition.Create(field.Name, template.Id) 
                newDefn.FieldDefinitionId <- field.Id
                dc.SubmitUpdates()

                let mutable newValue = dc.Dbo.FieldValue.Create(field.Id, 0uy)
                newValue.FieldValueId <- System.Guid.NewGuid()
                newValue.FieldDefinitionId <- field.Id
                newValue.ValueType <- match field.Field with
                                      | StringField s -> newValue.StringValue <- Some s; 1uy
                                      | DateField d -> newValue.DateValue <- Some d; 2uy
                                      | NumericField n -> newValue.NumericValue <- Some (float32 n); 3uy
            )

            dc.SubmitUpdates()
            scope.Complete()
            ()


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
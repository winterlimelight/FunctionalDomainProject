module AmApi.Persistence.Template

open System
open System.Transactions
open FSharp.Data.Sql
open Microsoft.FSharp.Linq
open FSharpx.Option
open AmApi.DomainTypes
open AmApi.Store


type private TemplateQueryResultSet = DbContext.``AssetManager.TemplateEntity`` * DbContext.``AssetManager.FieldDefinitionEntity`` * DbContext.``AssetManager.FieldValueEntity``

let private mapSingleTemplate (rows:TemplateQueryResultSet list) : Template =
    let fields = [ for row in rows do
                    let (_, defn, value) = row
                    let abc = defn.FieldDefinitionId
                    let fieldDefnId = Guid.Parse(abc)
                    if fieldDefnId <> Guid.Empty then // empty guid means template has no fields
                        yield {
                            Id = fieldDefnId
                            Name = defn.Name
                            Field = match value.ValueType with
                                    | 1y -> StringField(value.StringValue.Value)
                                    | 2y -> DateField(value.DateValue.Value)
                                    | 3y -> NumericField(float value.NumericValue.Value)
                                    | _ -> failwith "Unknown field type"
                        }]
    let (templateCols, _, _) = rows.Head
    {
        Id = Guid.Parse(templateCols.TemplateId)
        Name = templateCols.Name
        Fields = fields
        MaintenanceProgramId = Option.map Guid.Parse templateCols.MaintenanceProgramId
    }

let private templateByIdQuery (dc:DbContext) (id:Guid) : System.Linq.IQueryable<TemplateQueryResultSet> =
    query { 
        for template in dc.AssetManager.Template do
        // (!!) means left outer join
        for fieldDef in (!!) template.``AssetManager.FieldDefinition by TemplateId`` do
        for fieldVal in (!!) fieldDef.``AssetManager.FieldValue by FieldDefinitionId`` do
        where (template.TemplateId = (string id) && fieldVal.AssetId.IsNone)
        select (template, fieldDef, fieldVal)
    }

let private templateById dc id : Template option =
    let rows = templateByIdQuery dc id |> Seq.toList
    if [] = rows then None else Some (mapSingleTemplate rows)


module TemplateReadRepo =
    let findById dc id = 
        templateById dc id

module TemplateWriteRepo =
    let Save dc (template: Template) =
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
        let mutable newTemplate = dc.AssetManager.Template.Create(template.Name)
        newTemplate.TemplateId <- string template.Id
        newTemplate.MaintenanceProgramId <- Option.map string template.MaintenanceProgramId
        dc.SubmitUpdates()

        // add field definitions
        template.Fields 
        |> List.iter (fun field -> 
            let mutable newDefn = dc.AssetManager.FieldDefinition.Create(field.Name, (string template.Id)) 
            newDefn.FieldDefinitionId <- string field.Id
            dc.SubmitUpdates()

            let mutable newValue = dc.AssetManager.FieldValue.Create(string field.Id, 0y)
            newValue.FieldValueId <- string (Guid.NewGuid())
            newValue.FieldDefinitionId <- string field.Id
            newValue.ValueType <- match field.Field with
                                    | StringField s -> newValue.StringValue <- Some s; 1y
                                    | DateField d -> newValue.DateValue <- Some d; 2y
                                    | NumericField n -> newValue.NumericValue <- Some (float32 n); 3y
        )

        dc.SubmitUpdates()
        scope.Complete()
        ()
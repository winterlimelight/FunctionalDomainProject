module AssetManagementApi.DomainTypes

type MaintenanceSummary = MaintenanceSummary of string
type MaintenanceDetails = MaintenanceDetails of string

/// <remarks>MaintenanceProgram must be immutable in data store so that maintenance records referencing it have accurate information.</remarks>
type MaintenanceProgram = {
    Id: System.Guid
    Summary: MaintenanceSummary
    Period: System.TimeSpan
    Details: MaintenanceDetails
}

type MaintenanceRecordSummary = 
    | MaintenanceProgramId of System.Guid
    | Summary of MaintenanceSummary

type MaintenanceRecord = {
    Id: System.Guid
    Summary: MaintenanceRecordSummary
    DateComplete: System.DateTime
    Details: MaintenanceDetails
    Cost: decimal
}

type FieldValue = 
    | StringField of string
    | DateField of System.DateTime
    | NumericField of float

type FieldDefinition = {
    Id: System.Guid
    Name: string
    Field: FieldValue
}

type Field = {
    FieldDefinitionId: System.Guid
    Value: FieldValue
}

[<CLIMutable>]
type Template = {
    Id: System.Guid
    Name: string
    Fields: FieldDefinition list
    // I considered making MaintenanceProgram compositional, but that might be limiting for organizations that want a simple set
    // of scheduled maintenance periods (and no details specific to the type of maintenance)
    MaintenanceProgramId: System.Guid
}

type Asset = {
    Id: System.Guid
    Name: string
    Commissioned: System.DateTime
    Cost: decimal
    Fields: Field list
    // TemplateId is referenced rather than composed because it will belong to many Assets. Also composing template would 
    // result in field definitions being referenced twice - via template as well as fields.
    TemplateId: System.Guid 
    Subassets: Asset list
}
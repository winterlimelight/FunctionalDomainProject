use AssetManager

declare @templateIds table(id uniqueidentifier NOT NULL);
insert into @templateIds values ('11c43ee8-b9d3-4e51-b73f-bd9dda66e29c')

delete val
from FieldValue as val
inner join FieldDefinition def on def.FieldDefinitionId = val.FieldDefinitionId
where def.TemplateId in (select id from @templateIds)
	
delete from FieldDefinition
where TemplateId in (select id from @templateIds)

delete from Template
where TemplateId in (select id from @templateIds)
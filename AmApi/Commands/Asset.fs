module AmApi.Commands.Asset

open AmApi
open AmApi.Railway
open AmApi.DomainInterfaces

type AssetCommand =
    | Create of DomainTypes.Asset
    | Update of DomainTypes.Asset

type AssetCommandError =
    | InvalidAsset of string
    | DuplicateId
    | NotFound

type IAssetCommandHandler =
    abstract Execute: AssetCommand -> IAssetWriteRepository -> ITemplateReadRepository -> Result<unit,AssetCommandError>

let IsValid (asset: DomainTypes.Asset) (templateRepo: ITemplateReadRepository) =
    match asset with
    | { Fields = f } when isNull (box f) -> Failure (InvalidAsset "Asset fields must be a list, but may be empty to use template defaults")
    | { Subassets = f } when isNull (box f) -> Failure (InvalidAsset "Asset subassets must be a list, but may be empty if there are no sub-assets")
    | { Id = id } when id = System.Guid.Empty -> Failure (InvalidAsset "Asset must have an id")
    | { TemplateId = templateId } ->
        match templateId with
        | tid when tid = System.Guid.Empty -> Failure (InvalidAsset "Asset must have a template id")
        | tid when (None = templateRepo.FindById tid) -> Failure (InvalidAsset "Asset's template id must exist")
        | _ -> Success()
    | _ -> Success () 
    
// TODO - it is possible to create asset hierarchy in single hit... which leaves us with interesting challenges at a transactional level (e.g. parent creates ok, child is dupid)?...
// but is that something the domain should worry about, or is that a persistence concern only? Or does it mean our design is poor, and we should be talking about
// asset references and making the controller manage the saga? is entity vs aggregate question really. asset is an entity and an aggregate, sub-asset is a type of asset.
// I think the persistence can handle that. If SQL it uses a transaction; if Doc store it calls it a document/versions one.

let AssetCommandHandler = {
    new IAssetCommandHandler with
        member this.Execute (cmd: AssetCommand) (repo: IAssetWriteRepository) (templateRepo: ITemplateReadRepository) =
            match cmd with

            | Create(asset) ->
                railway {
                    do! IsValid asset templateRepo

                    let foundAsset = repo.FindById asset.Id
                    let! isDuplicate =
                        match foundAsset with 
                        | Some _ -> Failure DuplicateId
                        | None -> Success ()

                    repo.Save asset
                    return! Success ()
                }

            | Update(asset) -> 
                railway {
                    do! IsValid asset templateRepo

                    let foundAsset = repo.FindById asset.Id
                    let! exists = 
                        match foundAsset with
                        | None -> Failure NotFound
                        | Some _ -> Success()

                    repo.Save asset
                    return! Success ()
                }
}
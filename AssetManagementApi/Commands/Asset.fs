module AssetManagementApi.Commands.Asset

open AssetManagementApi
open AssetManagementApi.Railway
open AssetManagementApi.DomainInterfaces

type AssetCommand =
    | Create of DomainTypes.Asset
    | Update of DomainTypes.Asset

type AssetCommandError =
    | InvalidAsset of string
    | DuplicateId

type IAssetCommandHandler =
    abstract Execute: AssetCommand -> IAssetWriteRepository -> Result<unit,AssetCommandError>

(*let IsValid (template: DomainTypes.Template) =
    match template with
    | { Fields = f } when (Util.isNull f) -> Failure (InvalidTemplate "Template must include a list")
    | { Fields = [] } -> Failure (InvalidTemplate "Template may not have an empty list")
    | { Id = id } when id = System.Guid.Empty -> Failure (InvalidTemplate "Template must have an Id")
    // TODO validate template id - means we'll need template read repo.
    | _ -> Success () *)
    
// TODO - it is possible to create asset hierarchy in single hit... which leaves us with interesting challenges at a transactional level (e.g. parent creates ok, child is dupid)?...
// but is that something the domain should worry about, or is that a persistence concern only? Or does it mean our design is poor, and we should be talking about
// asset references and making the controller manage the saga? is entity vs aggregate question really. asset is an entity and an aggregate, sub-asset is a type of asset.

let AssetCommandHandler = {
    new IAssetCommandHandler with
        member this.Execute (cmd: AssetCommand) (repo: IAssetWriteRepository) =
            match cmd with

            | Create(asset) ->
                railway {
                    //do! IsValidStructure asset

                    let foundAsset = repo.FindById asset.Id
                    let! isDuplicate =
                        match foundAsset with 
                        | Some _ -> Failure DuplicateId
                        | None -> Success ()

                    repo.Save asset
                    return! Success ()
                }

            | Update(asset) -> Success ()
}
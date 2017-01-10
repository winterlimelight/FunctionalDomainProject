module AmApi.CompositionRoot

module Commands =
    let executeTemplateCommand dc = AmApi.Commands.Template.Execute (Persistence.Template.TemplateReadRepo.findById dc) (Persistence.Template.TemplateWriteRepo.Save dc)
    let executeAssetCommand dc = AmApi.Commands.Asset.Execute (Persistence.Asset.AssetReadRepo.findById dc) (Persistence.Template.TemplateReadRepo.findById dc) (Persistence.Asset.AssetWriteRepo.save dc)

module Operations =
    let getTemplate dc = AmApi.Operations.Template.GetTemplate (Persistence.Template.TemplateReadRepo.findById dc)
    let getAsset dc = AmApi.Operations.Asset.GetAsset (Persistence.Asset.AssetReadRepo.findById dc)

module ApiMethods =
    let getTemplate dc = Api.Template.getTemplate (Operations.getTemplate dc)
    let createTemplate logger dc = Api.Template.createTemplate logger (Commands.executeTemplateCommand dc)
    let getAsset dc = Api.Asset.getAsset (Operations.getAsset dc)
    let createAsset logger dc = Api.Asset.createAsset logger (Commands.executeAssetCommand dc)
    let updateAsset logger dc = Api.Asset.updateAsset logger (Commands.executeAssetCommand dc)
    


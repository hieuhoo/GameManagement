using AntDesign;
using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.Model.EditModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System;
using System.ComponentModel.Design;
using static AntDesign.JSInteropConstants.ObserverConstants;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.GameInformation
{
    public partial class GameDetail : ComponentBase
    {
        [Parameter] public EventCallback<int> ReSize { get; set; }

        GameEditModel EditModel { get; set; } = new();
        int Size => (EditModel.ImagePath.IsNotNullOrEmpty())
                          ? 12 : 0;
        List<SelectItem> NationalOptions { get; set; } = new();
        List<SelectItem> StatusGameOptions { get; set; } = new();
        List<SelectItem> GameSoldStatusOptions { get; set; } = new();
        List<SelectItem> PlatformOptions { get; set; } = new();


        IList<IBrowserFile> TemplateBrowserFiles = new List<IBrowserFile>();
        List<UploadFileItem> IdentityTemplateFiles { get; set; } = new();
        InputWatcher inputWatcher;
        string idCardUpload = null;
        string tempIdentityPathFile;
        string character;
        protected override void OnInitialized()
        {
            try
            {
                EditModel = new GameEditModel();
                EditModel.ReadOnly = false;
                idCardUpload = ObjectExtentions.GenerateGuid();
                NationalOptions = Enum.GetValues(typeof(National)).Cast<National>()
                     .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
                StatusGameOptions = Enum.GetValues(typeof(GameStatus)).Cast<GameStatus>()
                    .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
                GameSoldStatusOptions = Enum.GetValues(typeof(SoldStatus)).Cast<SoldStatus>()
                   .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
                PlatformOptions = Enum.GetValues(typeof(PlatformSystem)).Cast<PlatformSystem>()
                   .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task ReadImageAsync(InputFileChangeEventArgs e)
        {
            try
            {
                var file = e.File;
                TemplateBrowserFiles.Clear();
                TemplateBrowserFiles.Add(e.File);
                IdentityTemplateFiles = TemplateBrowserFiles.Select(file => new UploadFileItem
                {
                    FileName = file.Name,
                    Size = file.Size
                }).ToList();
                var pathFolder = AttachPath(character, GlobalVariant.TempFolder);
                tempIdentityPathFile = Path.Combine(pathFolder, file.Name);
                if (!Directory.Exists(Path.GetDirectoryName(tempIdentityPathFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(tempIdentityPathFile));
                }
                await using FileStream fs = new(tempIdentityPathFile, FileMode.Create);
                var task = e.File.OpenReadStream(GlobalVariant.MaxFileSize).CopyToAsync(fs);
                await Task.WhenAll(task);
                fs.Close();
                EditModel.ImageName = file.Name;
                EditModel.ImagePath = AttachPath(character, GlobalVariant.TempFolderResource, file.Name);
                EditModel.IsInUnsafeUpload = false;
                await ReSize.InvokeAsync(Size);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string AttachPath(string character, string baseFolder = null, string fileName = null)
        {
            var path = Path.Combine(baseFolder, character, "Files", "Game", "Image", "Attach");
            if (fileName.IsNotNullOrEmpty())
            {
                path = Path.Combine(path, fileName);
            }
            return path;
        }

    }
}

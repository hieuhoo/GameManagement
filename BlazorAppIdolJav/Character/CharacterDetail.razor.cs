using AntDesign;
using BlazorAppIdolJav.CoreConfig;
using BlazorAppIdolJav.CoreConfig.Extensions;
using BlazorAppIdolJav.Share.Model.EditModel;
using BlazorAppIdolJav.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System;
using System.ComponentModel.Design;
using static AntDesign.JSInteropConstants.ObserverConstants;
using static BlazorAppIdolJav.Share.Extension.EnumExtension;

namespace BlazorAppIdolJav.Character
{
    public partial class CharacterDetail : ComponentBase
    {
        [Parameter] public EventCallback<int> ReSize { get; set; }

        ActressEditModel EditModel { get; set; } = new();
        int Size => (EditModel.ImagePath.IsNotNullOrEmpty())
                          ? 12 : 0;
        List<SelectItem> NationalOptions { get; set; } = new();
        List<SelectItem> GenderOptions { get; set; } = new();

        IList<IBrowserFile> TemplateBrowserFiles = new List<IBrowserFile>();
        List<UploadFileItem> IdentityTemplateFiles { get; set; } = new();
        InputWatcher inputWatcher;
        string idCardUpload = null;
        string tempIdentityPathFile;
        string character => "Actress";
        protected override void OnInitialized()
        {
            try
            {
                EditModel = new ActressEditModel();
                EditModel.ReadOnly = false;
                idCardUpload = ObjectExtentions.GenerateGuid();
                NationalOptions = Enum.GetValues(typeof(National)).Cast<National>()
                     .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
                GenderOptions = Enum.GetValues(typeof(CharacterGender)).Cast<CharacterGender>()
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
            var path = Path.Combine(baseFolder, character, "Files", "Idol", "Image", "Attach");
            if (fileName.IsNotNullOrEmpty())
            {
                path = Path.Combine(path, fileName);
            }
            return path;
        }

    }
}

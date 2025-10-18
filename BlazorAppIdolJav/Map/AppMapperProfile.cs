using AutoMapper;
using BlazorAppIdolJav.Share.ClassData;
using BlazorAppIdolJav.Share.Model.EditModel;
using BlazorAppIdolJav.Share.Model.ViewModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorAppIdolJav.Map
{
    public class AppMapperProfile : Profile
    {
        public static JsonSerializerOptions options = new JsonSerializerOptions();
        public static JsonSerializerOptions optionIgnoreNulls = new JsonSerializerOptions();
        public AppMapperProfile()
        {
            optionIgnoreNulls.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            #region Đăng ký các mapping giữa Data class và DB class
            var assembly = Assembly.GetAssembly(typeof(AppMapperProfile));
            var classes = assembly.ExportedTypes
                 .Where(a => a.Name.EndsWith("") && !a.Name.EndsWith("Data"))
                 .Where(a => a.Namespace != null && a.Namespace.Contains("ClassDB"))
                 .ToList();
            classes.RemoveAll(c => c.Name.Contains("PagedResult"));
            foreach (Type type in classes)
            {
                //CreateMap(type, type).ReverseMap();
                var dataClass = assembly.ExportedTypes.FirstOrDefault(c => c.Name == type.Name + "Data");
                if (dataClass != null)
                {
                    CreateMap(type, dataClass).ReverseMap();
                    var editModelClass = assembly.ExportedTypes.FirstOrDefault(c => c.Name == type.Name + "EditModel");
                    if (editModelClass != null)
                    {
                        CreateMap(dataClass, editModelClass).ReverseMap();
                        CreateMap(type, editModelClass).ReverseMap();
                    }
                    var viewModelClass = assembly.ExportedTypes.FirstOrDefault(c => c.Name == type.Name + "ViewModel");
                    if (viewModelClass != null)
                    {
                        CreateMap(viewModelClass, viewModelClass).ReverseMap();
                        CreateMap(dataClass, viewModelClass).ReverseMap();
                    }
                    var searchModelClass = assembly.ExportedTypes.FirstOrDefault(c => c.Name == type.Name + "Search");
                    var filterModelClass = assembly.ExportedTypes.FirstOrDefault(c => c.Name == type.Name + "FilterEditModel");
                    if (searchModelClass != null && filterModelClass != null)
                    {
                        CreateMap(searchModelClass, filterModelClass).ReverseMap();
                    }
                    var docxModelClass = assembly.ExportedTypes.FirstOrDefault(c => c.Name == type.Name + "DocxModel");
                    if (docxModelClass != null)
                    {
                        CreateMap(dataClass, docxModelClass).ReverseMap();
                        if (editModelClass != null)
                        {
                            CreateMap(editModelClass, docxModelClass).ReverseMap();
                        }
                    }
                    var excelModelClass = assembly.ExportedTypes.FirstOrDefault(c => c.Name == type.Name + "ExportExcel");
                    if (excelModelClass != null)
                    {
                        CreateMap(dataClass, excelModelClass).ReverseMap();
                        if (editModelClass != null)
                        {
                            CreateMap(editModelClass, excelModelClass).ReverseMap();
                        }
                    }
                }
            }
            #endregion
        }
    }
}


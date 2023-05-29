using Newtonsoft.Json;
using Skybrud.Essentials.Reflection.Extensions;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models {

    public class PropertyEditorItem {

        [JsonProperty("alias")]
        public string Alias { get; }

        [JsonProperty("icon")]
        public string Icon { get; }

        [JsonProperty("group")]
        public string Group { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("deprecated")]
        public bool IsDeprecated { get; }

        [JsonProperty("editorType")]
        public string? EditorType { get; }

        [JsonProperty("valueType")]
        public string? ValueType { get; }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("assembly")]
        public PropertyEditorAssembly Assembly { get; }

        public PropertyEditorItem(IDataEditor editor) {

            var type = editor.GetType();
            var assembly = type.Assembly;

            Alias = editor.Alias;
            Icon = editor.Icon;
            Group = editor.Group;
            Name = editor.Name;
            IsDeprecated = editor.IsDeprecated;
            Type = type.FullName ?? type.Name;
            Assembly = new PropertyEditorAssembly(assembly);
            EditorType = editor.Type.ToString();
            ValueType = type.GetCustomAttribute<DataEditorAttribute>()?.ValueType;

        }

    }

}
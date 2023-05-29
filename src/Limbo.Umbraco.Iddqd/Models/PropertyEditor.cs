using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Reflection.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models {

    public class PropertyEditor {

        [JsonIgnore]
        public IDataEditor Editor { get; }

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

        [JsonProperty("duplicates")]
        public List<PropertyEditorItem> Duplicates { get; } = new();

        [JsonProperty("dataTypes")]
        public List<JObject> DataTypes { get; } = new();

        public PropertyEditor(IDataEditor editor, Dictionary<string, IGrouping<string, IDataType>> dataTypesByEditorAlias) {

            var type = editor.GetType();
            var assembly = type.Assembly;

            Editor = editor;
            Alias = editor.Alias;
            Icon = editor.Icon;
            Group = editor.Group;
            Name = editor.Name;
            IsDeprecated = editor.IsDeprecated;
            Type = type.FullName ?? type.Name;
            Assembly = new PropertyEditorAssembly(assembly);
            EditorType = editor.Type.ToString();
            ValueType = type.GetCustomAttribute<DataEditorAttribute>()?.ValueType;

            if (dataTypesByEditorAlias.TryGetValue(Alias, out IGrouping<string, IDataType>? dataTypes)) {
                foreach (var dataType in dataTypes) {
                    DataTypes.Add(DataTypeToJson(dataType));
                }
            }

        }

        private static JObject DataTypeToJson(IDataType dataType) {
            return new JObject {
                {"id", dataType.Id},
                {"key", dataType.Key},
                {"name", dataType.Name},
                {"editUrl", $"/umbraco/#/settings/dataTypes/edit/{dataType.Id}"}
            };
        }

    }

}
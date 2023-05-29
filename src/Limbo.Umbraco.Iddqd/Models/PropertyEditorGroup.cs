using System.Collections.Generic;
using Newtonsoft.Json;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models {

    public class PropertyEditorGroup {

        [JsonProperty("name")]
        public string? Name { get; }

        [JsonProperty("editors")]
        public IEnumerable<PropertyEditor> Editors { get; }

        public PropertyEditorGroup(string? name, IEnumerable<PropertyEditor> editors) {
            Name = name;
            Editors = editors;
        }

    }

}
using System.Collections.Generic;
using Newtonsoft.Json;
using Skybrud.Essentials.Collections;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Enums;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Models {

    public class PropertyEditorListResult {

        [JsonProperty("sortField")]
        [JsonConverter(typeof(EnumCamelCaseConverter))]
        public PropertyEditorField SortField { get; }

        [JsonProperty("sortOrder")]
        [JsonConverter(typeof(EnumCamelCaseConverter))]
        public SortOrder SortOrder { get; }

        [JsonProperty("groupBy")]
        [JsonConverter(typeof(EnumCamelCaseConverter))]
        public PropertyEditorField GroupBy { get; }

        [JsonProperty("groups")]
        public IEnumerable<PropertyEditorGroup> Groups { get; }

        public PropertyEditorListResult(PropertyEditorField sortField, SortOrder sortOrder, PropertyEditorField groupBy, IEnumerable<PropertyEditorGroup> groups) {
            SortField = sortField;
            SortOrder = sortOrder;
            GroupBy = groupBy;
            Groups = groups;
        }

    }

}
using System.Collections.Generic;
using Limbo.Umbraco.Iddqd.Models.Assemblies;
using Limbo.Umbraco.Iddqd.Models.DataTypes;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Iddqd.Factories;

public class IddqdModelFactory {

    public virtual IddqdDataType CreateDataType(IDataType dataType, List<object> path) {

        IddqdDataEditor? editor = dataType.Editor is null ? null : CreateDataEditor(dataType.Editor, dataType);

        return new IddqdDataType {
            Id = dataType.Id,
            Key = dataType.Key,
            Name = dataType.Name!,
            Path = path,
            EditorAlias = dataType.EditorAlias,
            EditorUiAlias = dataType.EditorUiAlias,
            DatabaseType = dataType.DatabaseType.ToString(),
            Editor = editor,
            CreateDate = dataType.CreateDate,
            UpdateDate = dataType.UpdateDate
        };

    }

    public virtual IddqdDataEditor? CreateDataEditor(IDataEditor editor, IDataType dataType) {

        if (editor is MissingPropertyEditor) return null;

        IDataValueEditor valueEditor = editor.GetValueEditor(dataType.ConfigurationObject);

        return new IddqdDataEditor {
            Alias = editor.Alias,
            Type = editor.GetType().ToString(),
            IsReadOnly = valueEditor.IsReadOnly,
            ValueType = valueEditor.ValueType,
            DatabaseType = ValueTypes.ToStorageType(valueEditor.ValueType).ToString(),
            Assembly = new IddqdAssembly(editor.GetType().Assembly)
        };

    }

}
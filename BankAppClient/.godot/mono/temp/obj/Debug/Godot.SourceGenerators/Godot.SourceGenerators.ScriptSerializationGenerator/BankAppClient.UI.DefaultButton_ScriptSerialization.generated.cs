using Godot;
using Godot.NativeInterop;

namespace BankAppClient.UI {

partial class DefaultButton
{
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void SaveGodotObjectData(global::Godot.Bridge.GodotSerializationInfo info)
    {
        base.SaveGodotObjectData(info);
        info.AddProperty(PropertyName.@nodePath, global::Godot.Variant.From<string>(this.@nodePath));
        info.AddProperty(PropertyName.@testButton, global::Godot.Variant.From<global::Godot.Button>(this.@testButton));
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void RestoreGodotObjectData(global::Godot.Bridge.GodotSerializationInfo info)
    {
        base.RestoreGodotObjectData(info);
        if (info.TryGetProperty(PropertyName.@nodePath, out var _value_nodePath))
            this.@nodePath = _value_nodePath.As<string>();
        if (info.TryGetProperty(PropertyName.@testButton, out var _value_testButton))
            this.@testButton = _value_testButton.As<global::Godot.Button>();
    }
}

}

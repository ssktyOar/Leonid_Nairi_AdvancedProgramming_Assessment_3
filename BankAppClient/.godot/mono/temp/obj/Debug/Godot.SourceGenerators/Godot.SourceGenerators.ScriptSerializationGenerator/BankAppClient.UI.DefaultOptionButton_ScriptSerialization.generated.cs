using Godot;
using Godot.NativeInterop;

namespace BankAppClient.UI {

partial class DefaultOptionButton
{
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void SaveGodotObjectData(global::Godot.Bridge.GodotSerializationInfo info)
    {
        base.SaveGodotObjectData(info);
        info.AddProperty(PropertyName.@nodePath, global::Godot.Variant.From<string>(this.@nodePath));
        info.AddProperty(PropertyName.@form, global::Godot.Variant.From<global::BankAppClient.UI.BankAccountScreen.BankAccountLoader>(this.@form));
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void RestoreGodotObjectData(global::Godot.Bridge.GodotSerializationInfo info)
    {
        base.RestoreGodotObjectData(info);
        if (info.TryGetProperty(PropertyName.@nodePath, out var _value_nodePath))
            this.@nodePath = _value_nodePath.As<string>();
        if (info.TryGetProperty(PropertyName.@form, out var _value_form))
            this.@form = _value_form.As<global::BankAppClient.UI.BankAccountScreen.BankAccountLoader>();
    }
}

}

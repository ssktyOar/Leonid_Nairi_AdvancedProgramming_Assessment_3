using Godot;
using Godot.NativeInterop;

namespace BankAppClient.UI.BankAccountScreen {

partial class CreateBankAccountLoader
{
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void SaveGodotObjectData(global::Godot.Bridge.GodotSerializationInfo info)
    {
        base.SaveGodotObjectData(info);
        info.AddProperty(PropertyName.@optionButton, global::Godot.Variant.From<global::BankAppClient.UI.DefaultOptionButton>(this.@optionButton));
        info.AddProperty(PropertyName.@checkBox, global::Godot.Variant.From<global::BankAppClient.UI.DefaultOptionCheckBox>(this.@checkBox));
        info.AddProperty(PropertyName.@selectedOption, global::Godot.Variant.From<long>(this.@selectedOption));
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void RestoreGodotObjectData(global::Godot.Bridge.GodotSerializationInfo info)
    {
        base.RestoreGodotObjectData(info);
        if (info.TryGetProperty(PropertyName.@optionButton, out var _value_optionButton))
            this.@optionButton = _value_optionButton.As<global::BankAppClient.UI.DefaultOptionButton>();
        if (info.TryGetProperty(PropertyName.@checkBox, out var _value_checkBox))
            this.@checkBox = _value_checkBox.As<global::BankAppClient.UI.DefaultOptionCheckBox>();
        if (info.TryGetProperty(PropertyName.@selectedOption, out var _value_selectedOption))
            this.@selectedOption = _value_selectedOption.As<long>();
    }
}

}

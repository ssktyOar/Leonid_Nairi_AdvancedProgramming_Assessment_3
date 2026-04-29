using Godot;
using Godot.NativeInterop;

namespace BankAppClient.UI.LoadingScreen {

partial class LoadingText
{
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void SaveGodotObjectData(global::Godot.Bridge.GodotSerializationInfo info)
    {
        base.SaveGodotObjectData(info);
        info.AddProperty(PropertyName.@nodePath, global::Godot.Variant.From<string>(this.@nodePath));
        info.AddProperty(PropertyName.@sign, global::Godot.Variant.From<int>(this.@sign));
        info.AddProperty(PropertyName.@count, global::Godot.Variant.From<int>(this.@count));
        info.AddProperty(PropertyName.@deltaTime, global::Godot.Variant.From<double>(this.@deltaTime));
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void RestoreGodotObjectData(global::Godot.Bridge.GodotSerializationInfo info)
    {
        base.RestoreGodotObjectData(info);
        if (info.TryGetProperty(PropertyName.@nodePath, out var _value_nodePath))
            this.@nodePath = _value_nodePath.As<string>();
        if (info.TryGetProperty(PropertyName.@sign, out var _value_sign))
            this.@sign = _value_sign.As<int>();
        if (info.TryGetProperty(PropertyName.@count, out var _value_count))
            this.@count = _value_count.As<int>();
        if (info.TryGetProperty(PropertyName.@deltaTime, out var _value_deltaTime))
            this.@deltaTime = _value_deltaTime.As<double>();
    }
}

}

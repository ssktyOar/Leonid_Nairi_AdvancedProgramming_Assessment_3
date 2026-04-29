using Godot;
using Godot.NativeInterop;

namespace BankAppClient.UI {

partial class DefaultForm
{
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void SaveGodotObjectData(global::Godot.Bridge.GodotSerializationInfo info)
    {
        base.SaveGodotObjectData(info);
        info.AddProperty(PropertyName.@sendingRequestType, global::Godot.Variant.From<string>(this.@sendingRequestType));
        info.AddProperty(PropertyName.@recievingRequestType, global::Godot.Variant.From<string>(this.@recievingRequestType));
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void RestoreGodotObjectData(global::Godot.Bridge.GodotSerializationInfo info)
    {
        base.RestoreGodotObjectData(info);
        if (info.TryGetProperty(PropertyName.@sendingRequestType, out var _value_sendingRequestType))
            this.@sendingRequestType = _value_sendingRequestType.As<string>();
        if (info.TryGetProperty(PropertyName.@recievingRequestType, out var _value_recievingRequestType))
            this.@recievingRequestType = _value_recievingRequestType.As<string>();
    }
}

}

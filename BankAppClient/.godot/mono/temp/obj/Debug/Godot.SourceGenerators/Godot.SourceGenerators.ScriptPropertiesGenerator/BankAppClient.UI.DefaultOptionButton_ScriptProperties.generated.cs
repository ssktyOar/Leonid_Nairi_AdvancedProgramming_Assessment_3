using Godot;
using Godot.NativeInterop;

namespace BankAppClient.UI {

partial class DefaultOptionButton
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the properties and fields contained in this class, for fast lookup.
    /// </summary>
    public new class PropertyName : global::Godot.OptionButton.PropertyName {
        /// <summary>
        /// Cached name for the 'nodePath' field.
        /// </summary>
        public new static readonly global::Godot.StringName @nodePath = "nodePath";
        /// <summary>
        /// Cached name for the 'form' field.
        /// </summary>
        public new static readonly global::Godot.StringName @form = "form";
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
    {
        if (name == PropertyName.@nodePath) {
            this.@nodePath = global::Godot.NativeInterop.VariantUtils.ConvertTo<string>(value);
            return true;
        }
        if (name == PropertyName.@form) {
            this.@form = global::Godot.NativeInterop.VariantUtils.ConvertTo<global::BankAppClient.UI.BankAccountScreen.BankAccountLoader>(value);
            return true;
        }
        return base.SetGodotClassPropertyValue(name, value);
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
    {
        if (name == PropertyName.@nodePath) {
            value = global::Godot.NativeInterop.VariantUtils.CreateFrom<string>(this.@nodePath);
            return true;
        }
        if (name == PropertyName.@form) {
            value = global::Godot.NativeInterop.VariantUtils.CreateFrom<global::BankAppClient.UI.BankAccountScreen.BankAccountLoader>(this.@form);
            return true;
        }
        return base.GetGodotClassPropertyValue(name, out value);
    }
    /// <summary>
    /// Get the property information for all the properties declared in this class.
    /// This method is used by Godot to register the available properties in the editor.
    /// Do not call this method.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal new static global::System.Collections.Generic.List<global::Godot.Bridge.PropertyInfo> GetGodotPropertyList()
    {
        var properties = new global::System.Collections.Generic.List<global::Godot.Bridge.PropertyInfo>();
        properties.Add(new(type: (global::Godot.Variant.Type)4, name: PropertyName.@nodePath, hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::Godot.Variant.Type)24, name: PropertyName.@form, hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)4096, exported: false));
        return properties;
    }
#pragma warning restore CS0109
}

}

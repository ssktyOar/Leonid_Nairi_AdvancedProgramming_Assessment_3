using Godot;
using Godot.NativeInterop;

namespace BankAppClient.UI.BankAccountScreen {

partial class CreateBankAccountLoader
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the properties and fields contained in this class, for fast lookup.
    /// </summary>
    public new class PropertyName : global::BankAppClient.UI.DefaultForm.PropertyName {
        /// <summary>
        /// Cached name for the 'optionButton' field.
        /// </summary>
        public new static readonly global::Godot.StringName @optionButton = "optionButton";
        /// <summary>
        /// Cached name for the 'checkBox' field.
        /// </summary>
        public new static readonly global::Godot.StringName @checkBox = "checkBox";
        /// <summary>
        /// Cached name for the 'selectedOption' field.
        /// </summary>
        public new static readonly global::Godot.StringName @selectedOption = "selectedOption";
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
    {
        if (name == PropertyName.@optionButton) {
            this.@optionButton = global::Godot.NativeInterop.VariantUtils.ConvertTo<global::BankAppClient.UI.DefaultOptionButton>(value);
            return true;
        }
        if (name == PropertyName.@checkBox) {
            this.@checkBox = global::Godot.NativeInterop.VariantUtils.ConvertTo<global::BankAppClient.UI.DefaultOptionCheckBox>(value);
            return true;
        }
        if (name == PropertyName.@selectedOption) {
            this.@selectedOption = global::Godot.NativeInterop.VariantUtils.ConvertTo<long>(value);
            return true;
        }
        return base.SetGodotClassPropertyValue(name, value);
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
    {
        if (name == PropertyName.@optionButton) {
            value = global::Godot.NativeInterop.VariantUtils.CreateFrom<global::BankAppClient.UI.DefaultOptionButton>(this.@optionButton);
            return true;
        }
        if (name == PropertyName.@checkBox) {
            value = global::Godot.NativeInterop.VariantUtils.CreateFrom<global::BankAppClient.UI.DefaultOptionCheckBox>(this.@checkBox);
            return true;
        }
        if (name == PropertyName.@selectedOption) {
            value = global::Godot.NativeInterop.VariantUtils.CreateFrom<long>(this.@selectedOption);
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
        properties.Add(new(type: (global::Godot.Variant.Type)24, name: PropertyName.@optionButton, hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::Godot.Variant.Type)24, name: PropertyName.@checkBox, hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::Godot.Variant.Type)2, name: PropertyName.@selectedOption, hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)4096, exported: false));
        return properties;
    }
#pragma warning restore CS0109
}

}

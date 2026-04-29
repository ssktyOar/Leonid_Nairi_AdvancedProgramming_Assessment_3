using Godot;
using Godot.NativeInterop;

namespace BankAppClient.UI.LoadingScreen {

partial class LoadingText
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the properties and fields contained in this class, for fast lookup.
    /// </summary>
    public new class PropertyName : global::Godot.RichTextLabel.PropertyName {
        /// <summary>
        /// Cached name for the 'nodePath' field.
        /// </summary>
        public new static readonly global::Godot.StringName @nodePath = "nodePath";
        /// <summary>
        /// Cached name for the 'text' field.
        /// </summary>
        public new static readonly global::Godot.StringName @text = "text";
        /// <summary>
        /// Cached name for the 'sign' field.
        /// </summary>
        public new static readonly global::Godot.StringName @sign = "sign";
        /// <summary>
        /// Cached name for the 'count' field.
        /// </summary>
        public new static readonly global::Godot.StringName @count = "count";
        /// <summary>
        /// Cached name for the 'deltaTime' field.
        /// </summary>
        public new static readonly global::Godot.StringName @deltaTime = "deltaTime";
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
    {
        if (name == PropertyName.@nodePath) {
            this.@nodePath = global::Godot.NativeInterop.VariantUtils.ConvertTo<string>(value);
            return true;
        }
        if (name == PropertyName.@sign) {
            this.@sign = global::Godot.NativeInterop.VariantUtils.ConvertTo<int>(value);
            return true;
        }
        if (name == PropertyName.@count) {
            this.@count = global::Godot.NativeInterop.VariantUtils.ConvertTo<int>(value);
            return true;
        }
        if (name == PropertyName.@deltaTime) {
            this.@deltaTime = global::Godot.NativeInterop.VariantUtils.ConvertTo<double>(value);
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
        if (name == PropertyName.@text) {
            value = global::Godot.NativeInterop.VariantUtils.CreateFrom<string[]>(this.@text);
            return true;
        }
        if (name == PropertyName.@sign) {
            value = global::Godot.NativeInterop.VariantUtils.CreateFrom<int>(this.@sign);
            return true;
        }
        if (name == PropertyName.@count) {
            value = global::Godot.NativeInterop.VariantUtils.CreateFrom<int>(this.@count);
            return true;
        }
        if (name == PropertyName.@deltaTime) {
            value = global::Godot.NativeInterop.VariantUtils.CreateFrom<double>(this.@deltaTime);
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
        properties.Add(new(type: (global::Godot.Variant.Type)34, name: PropertyName.@text, hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::Godot.Variant.Type)2, name: PropertyName.@sign, hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::Godot.Variant.Type)2, name: PropertyName.@count, hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::Godot.Variant.Type)3, name: PropertyName.@deltaTime, hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)4096, exported: false));
        return properties;
    }
#pragma warning restore CS0109
}

}

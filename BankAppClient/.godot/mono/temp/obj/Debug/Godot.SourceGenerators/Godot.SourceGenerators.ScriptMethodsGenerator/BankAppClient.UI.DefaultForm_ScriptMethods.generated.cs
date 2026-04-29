using Godot;
using Godot.NativeInterop;

namespace BankAppClient.UI {

partial class DefaultForm
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the methods contained in this class, for fast lookup.
    /// </summary>
    public new class MethodName : global::Godot.Control.MethodName {
        /// <summary>
        /// Cached name for the 'TryAddField' method.
        /// </summary>
        public new static readonly global::Godot.StringName @TryAddField = "TryAddField";
        /// <summary>
        /// Cached name for the 'Submit' method.
        /// </summary>
        public new static readonly global::Godot.StringName @Submit = "Submit";
        /// <summary>
        /// Cached name for the 'Request' method.
        /// </summary>
        public new static readonly global::Godot.StringName @Request = "Request";
    }
    /// <summary>
    /// Get the method information for all the methods declared in this class.
    /// This method is used by Godot to register the available methods in the editor.
    /// Do not call this method.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal new static global::System.Collections.Generic.List<global::Godot.Bridge.MethodInfo> GetGodotMethodList()
    {
        var methods = new global::System.Collections.Generic.List<global::Godot.Bridge.MethodInfo>(3);
        methods.Add(new(name: MethodName.@TryAddField, returnVal: new(type: (global::Godot.Variant.Type)1, name: "", hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)6, exported: false), flags: (global::Godot.MethodFlags)1, arguments: new() { new(type: (global::Godot.Variant.Type)4, name: "fieldName", hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)6, exported: false), new(type: (global::Godot.Variant.Type)24, name: "field", hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)6, exported: false, className: new global::Godot.StringName("LineEdit")),  }, defaultArguments: null));
        methods.Add(new(name: MethodName.@Submit, returnVal: new(type: (global::Godot.Variant.Type)0, name: "", hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)6, exported: false), flags: (global::Godot.MethodFlags)1, arguments: null, defaultArguments: null));
        methods.Add(new(name: MethodName.@Request, returnVal: new(type: (global::Godot.Variant.Type)0, name: "", hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)6, exported: false), flags: (global::Godot.MethodFlags)1, arguments: null, defaultArguments: null));
        return methods;
    }
#pragma warning restore CS0109
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
    {
        if (method == MethodName.@TryAddField && args.Count == 2) {
            var callRet = @TryAddField(global::Godot.NativeInterop.VariantUtils.ConvertTo<string>(args[0]), global::Godot.NativeInterop.VariantUtils.ConvertTo<global::Godot.LineEdit>(args[1]));
            ret = global::Godot.NativeInterop.VariantUtils.CreateFrom<bool>(callRet);
            return true;
        }
        if (method == MethodName.@Submit && args.Count == 0) {
            @Submit();
            ret = default;
            return true;
        }
        if (method == MethodName.@Request && args.Count == 0) {
            @Request();
            ret = default;
            return true;
        }
        return base.InvokeGodotClassMethod(method, args, out ret);
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool HasGodotClassMethod(in godot_string_name method)
    {
        if (method == MethodName.@TryAddField) {
           return true;
        }
        if (method == MethodName.@Submit) {
           return true;
        }
        if (method == MethodName.@Request) {
           return true;
        }
        return base.HasGodotClassMethod(method);
    }
}

}

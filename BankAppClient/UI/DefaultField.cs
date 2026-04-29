using Godot;
using System;

namespace BankAppClient.UI
{
    public partial class DefaultField : LineEdit
    {
        public override void _Ready()
        {
            GetParent<DefaultForm>().TryAddField(Name, this);
        }

        internal void SetColor(bool isSussessful)
        {
            if (isSussessful)
            {
                AddThemeColorOverride("font_color", new("00ff00"));
                AddThemeColorOverride("font_placeholder_color", new("00ff00"));
                AddThemeColorOverride("font_uneditable_color", new("00ff00"));
            }
            else
            {
                AddThemeColorOverride("font_color", new("ff0000"));
                AddThemeColorOverride("font_placeholder_color", new("ff0000"));
                AddThemeColorOverride("font_uneditable_color", new("ff0000"));
            }
        }
    }
}
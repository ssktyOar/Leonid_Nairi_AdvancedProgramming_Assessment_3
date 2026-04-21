using System;
using Godot;

namespace BankAppClient.UI
{
    internal partial class DefaultButtonScreenSwitch : Button
    {
        private string nodePath;
        private DefaultForm form;
        
        public override void _Ready()
        {
            nodePath = GetPath();
            Pressed += DisplayMessage;
            form = GetParent<DefaultForm>();
        }
        internal virtual void DisplayMessage()
        {
            GD.Print(nodePath);
            GD.Print(Name);
            form.SwitchScreens(Name);
        }
    }
}
using System;
using BankAppClient.UI.CreateBankAccountScreen;
using Godot;

namespace BankAppClient.UI
{
    internal partial class DefaultCheckBox : CheckBox
    {
        private string nodePath;
        private CreateBankAccountLoader form;       
        public override void _Ready()
        {
            nodePath = GetPath();
            form = GetParent<CreateBankAccountLoader>();
            Toggled += Check;
            form.checkBox = this;
        }

        private void Check(bool b)
        {
            GD.Print(ButtonPressed.ToString());
        }
    }
}
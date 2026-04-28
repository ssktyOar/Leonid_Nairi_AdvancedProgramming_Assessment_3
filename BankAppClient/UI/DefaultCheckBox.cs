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
            form.checkBox = this;
        }
    }
}
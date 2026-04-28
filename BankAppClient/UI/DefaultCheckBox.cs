using System;
using BankAppClient.UI.BankAccountScreen;
using Godot;

namespace BankAppClient.UI
{
    internal partial class DefaultOptionCheckBox : CheckBox
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
using System;
using BankAppClient.UI.BankAccountScreen;
using Godot;

namespace BankAppClient.UI
{
    internal partial class DefaultOptionButton : OptionButton
    {
        private string nodePath;
        private CreateBankAccountLoader form;       
        public override void _Ready()
        {
            nodePath = GetPath();
            ItemSelected += DisplayMessage;
            form = GetParent<CreateBankAccountLoader>();
            form.optionButton = this;
        }

        internal virtual void DisplayMessage(long index)
        {
            GD.Print(nodePath);
            GD.Print(Name);
            //form.Select(index);
        }
    }
}
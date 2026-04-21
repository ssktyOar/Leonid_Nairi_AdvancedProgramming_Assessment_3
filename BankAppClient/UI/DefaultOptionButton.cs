using System;
using BankAppClient.UI.BankAccountScreen;
using Godot;

namespace BankAppClient.UI
{
    internal partial class DefaultOptionButton : OptionButton
    {
        private string nodePath;
        private BankAccountLoader form;       
        public override void _Ready()
        {
            nodePath = GetPath();
            //testButton = GetNode<Button>("TestButton");
            ItemSelected += DisplayMessage;
            form = GetParent<BankAccountLoader>();
            form.optionButton = this;
        }

        internal virtual void DisplayMessage(long index)
        {
            GD.Print(nodePath);
            GD.Print(Name);
            form.Select(index);
        }
    }
}
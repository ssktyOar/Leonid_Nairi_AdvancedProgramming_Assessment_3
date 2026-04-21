using System;
using Godot;

namespace BankAppClient.UI
{
    internal partial class DefaultButton : Button
    {
        private string nodePath;
        private DefaultForm form;
        
        public override void _Ready()
        {
            nodePath = GetPath();
            //testButton = GetNode<Button>("TestButton");
            Pressed += DisplayMessage;
            form = GetParent<DefaultForm>();
        }
        internal virtual void DisplayMessage()
        {
            GD.Print(nodePath);
            GD.Print(Name);
            form.Submit(Name);
        }
    }
}
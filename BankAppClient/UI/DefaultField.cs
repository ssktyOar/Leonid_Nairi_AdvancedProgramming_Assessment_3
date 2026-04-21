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
    }
}
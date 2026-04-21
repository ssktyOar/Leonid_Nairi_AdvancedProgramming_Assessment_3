using System;
using Godot;
using BankAppClient.UI;

namespace BankAppClient.UI.LoadingScreen
{
    internal partial class LoadingScreenLoader : DefaultForm
    {
        public override void _Ready()
        {
            UITerminal.SetScreen(this, ScreenType.Loading);
            base._Ready();
        }
    }
}
using System;
using Godot;
using BankAppClient.UI;

namespace BankAppClient.UI.LaunchScreen
{
    internal partial class LaunchScreenLoader : DefaultForm
    {
        public override void _Ready()
        {
            UITerminal.SetScreen(this, ScreenType.EnterUserAccount);
            base._Ready();
        }

        internal override void ActionOnResponse(string requestType)
        {
            response.TryGetValue("result", out string result);
            GD.Print(result);
            if (result == "success" && response.TryGetValue("cookie", out string _cookie))
            {
                cookie = _cookie;
                UITerminal.SwitchScreens(ScreenType.EditUserAccount);
            }
        }
    }
}
using System;
using Godot;
using BankAppClient.UI;

namespace BankAppClient.UI.LaunchScreen
{
    internal partial class LaunchScreenLoader : DefaultForm
    {
        internal override void ActionOnResponse(string requestType)
        {
            response.TryGetValue("result", out string result);
            GD.Print(result);
            if (result == "success" && response.TryGetValue("cookie", out string _cookie))
            {
                cookie = _cookie;
                SwitchScreens("MainPage");
            }
        }
    }
}
using System;
using Godot;
using BankAppClient.UI;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using BankAppClient.Core;
using System.Linq;

namespace BankAppClient.UI.BankAccountScreen
{
    internal partial class MainPageLoader : DefaultForm
    {
        internal override void Submit(string requestType)
        {
            base.Submit(requestType);
            UITerminal.SwitchScreens("LaunchScreen");
        }
    }
}
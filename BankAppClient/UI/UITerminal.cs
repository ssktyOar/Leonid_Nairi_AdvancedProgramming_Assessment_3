using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Godot;

namespace BankAppClient.UI
{
    internal static class UITerminal
    {
        private static readonly Dictionary<string,DefaultForm> screens = [];

        internal static void SwitchScreens(string screenName)
        {
            foreach(var key in screens.Keys)
            {
                if(key == screenName)
                {
                    screens[key].Visible = true;
                    screens[key].Activated();
                }
                else { screens[key].Visible = false; }
            }
        }

        internal static void SetScreen(DefaultForm control)
        {
            screens.Add(control.Name, control);
        }
    }
}
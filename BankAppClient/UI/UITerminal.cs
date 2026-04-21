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
        private static readonly DefaultForm[] screens = new DefaultForm[8];

        internal static void SwitchScreens(ScreenType screenType)
        {
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i] == null) { continue; }
                if(i == (int)screenType)
                {
                    screens[i].Visible = true;
                    screens[i].Activated();
                }
                else { screens[i].Visible = false; }
            }
        }

        internal static void SetScreen(DefaultForm control, ScreenType screenType)
        {
            screens[(int)screenType] = control;
        }
    }
}
using System;
using BankAppClient.Core;
using Godot;

namespace BankAppClient.UI.LoadingScreen
{
    internal partial class LoadingText : RichTextLabel
    {
        private string nodePath;

        private static readonly string messageText = "Awaiting server connection";
        private readonly string[] text = [messageText, messageText + ".", messageText + "..", messageText + "...", messageText + "....", messageText + "....."];
        
        private int sign = 1;
        private int count = 0;
        private double deltaTime = 0;
        private bool b = true;
        public override void _Ready()
        {
            nodePath = GetPath();
            GD.Print(SVTerminal.TestRequest());
        }

        public override void _PhysicsProcess(double delta)
        {
            if (b)
            {
                base._PhysicsProcess(delta);

                deltaTime += delta;
                if (deltaTime > 0.3)
                {
                    if (count + sign >= text.Length || count + sign < 0) { sign *= -1; }
                    count += sign;
                    Text = text[count];
                    deltaTime = 0;
                    if (SVTerminal.TestConnection())
                    {
                        UITerminal.SwitchScreens("LaunchScreen");
                        b = false;
                    }
                }
            }
        }
    }
}
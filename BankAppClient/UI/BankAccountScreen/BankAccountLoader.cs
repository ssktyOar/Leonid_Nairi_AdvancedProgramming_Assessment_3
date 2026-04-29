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
    internal partial class BankAccountLoader : DefaultForm
    {

        internal override void Submit(string requestType)
        {
            base.Submit(requestType);
            // UpdateFields(response);
            if(response != null && response.TryGetValue("result", out string result) && fields.TryGetValue("result", out DefaultField lineEdit))
            {
                try { if (requestType != "getInfo"){ Activated(); } } 
                catch (Exception e) { SVTerminal.PrintException(e); }

                if (result == "failure") 
                { 
                    lineEdit.Text = "Error"; 
                    lineEdit.SetColor(false);
                    if (requestType == "deleteBankAccount")
                    {
                        lineEdit.Text = "Balance must be 0";
                    }
                }
                else if (requestType != "deleteBankAccount")
                { 
                    lineEdit.Text = "Success"; 
                    lineEdit.SetColor(true);
                    GD.Print("Success"); 
                }
                else
                {
                    SwitchScreens("LaunchScreen");
                }
            } else if(fields.TryGetValue("result", out DefaultField _lineEdit)) { _lineEdit.Text = "Error"; GD.Print("Error"); }
        }

        internal override void Activated()
        {
            Submit("getInfo");
            GD.Print("Activated");
            response.TryGetValue("result", out string result);
            if (result == "success" && response.TryGetValue("data", out string data))
            {
                GD.Print("Data exists");
                Dictionary<string, string> parsedData = JsonDocument.Parse(data).RootElement.Deserialize<Dictionary<string, string>>()!;
                foreach(var s in parsedData)
                {
                    if(!response.TryAdd(s.Key, s.Value))
                    {
                        response[s.Key] = s.Value;
                    }
                    GD.Print(s.Key + ": " + s.Value);
                }
                UpdateFields(response); 
            }
        }
    }
}
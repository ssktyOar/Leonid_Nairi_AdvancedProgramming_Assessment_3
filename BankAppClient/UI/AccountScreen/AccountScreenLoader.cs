using System;
using Godot;
using BankAppClient.UI;
using System.Collections.Generic;
using System.Text.Json;

namespace BankAppClient.UI.LaunchScreen
{
    internal partial class AccountScreenLoader : DefaultForm
    {
        internal override void ActionOnResponse(string requestType)
        {
            response.TryGetValue("result", out string result);
            GD.Print(result);
            UpdateFields(response);            
            if (result == "success" && requestType == "logout")
            {
                SwitchScreens("LaunchScreen");
            }
            if(fields.TryGetValue("result", out LineEdit lineEdit))
            {
                if (result == "failure") { lineEdit.Text = "Error"; }
                //lineEdit.Text = result;
            } else { lineEdit.Text = "Success"; GD.Print("Success"); }
        }

        internal override void Activated()
        {
            GD.Print("Activated");
            Submit("getInfo");
            response.TryGetValue("result", out string result);
            if (result == "success" && response.TryGetValue("data", out string data))
            {
                GD.Print("Data exists");
                Dictionary<string, string> parsedData = JsonDocument.Parse(data).RootElement.Deserialize<Dictionary<string, string>>()!;
                if (!parsedData.TryGetValue("accountInfo", out string accountInfo))
                {
                    return;
                }
                foreach(var s in JsonDocument.Parse(accountInfo).RootElement.Deserialize<Dictionary<string, string>>()!)
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
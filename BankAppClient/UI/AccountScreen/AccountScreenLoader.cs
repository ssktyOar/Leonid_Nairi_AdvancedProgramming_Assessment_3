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
            if(fields.TryGetValue("result", out DefaultField lineEdit))
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
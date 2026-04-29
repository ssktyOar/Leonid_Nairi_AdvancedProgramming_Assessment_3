using System;
using Godot;
using BankAppClient.UI;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using BankAppClient.Core;
using System.Linq;

namespace BankAppClient.UI.TransactionScreen
{
    internal partial class TransactionScreen : DefaultForm
    {

        internal override void Submit(string requestType)
        {
            base.Submit(requestType);
            if(response != null && response.TryGetValue("result", out string result) && 
                fields.TryGetValue("result", out DefaultField lineEdit) &&
                fields.TryGetValue("pin", out DefaultField pinField))
            {
                if (requestType != "getInfo"){ Activated(); }
                if (result == "failure") 
                { 
                    lineEdit.Text = "Error"; // 6054398111621326674 
                    lineEdit.SetColor(false);
                    if (pinField.Text.Length != 4)
                    {
                        pinField.SetColor(false);
                    } else { pinField.SetColor(false); }
                }
                else 
                { 
                    lineEdit.Text = "Success"; 
                    lineEdit.SetColor(true);
                    GD.Print("Success"); 
                    pinField.SetColor(true);
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
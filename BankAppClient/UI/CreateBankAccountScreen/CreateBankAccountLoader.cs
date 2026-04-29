using System;
using Godot;
using BankAppClient.UI;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using BankAppClient.Core;
using System.Linq;

namespace BankAppClient.UI.CreateBankAccountScreen
{
    internal partial class CreateBankAccountLoader : DefaultForm
    {
        internal DefaultOptionButton optionButton;
        internal DefaultCheckBox checkBox;
        private long selectedOption = 0;

        private readonly Dictionary<string, Dictionary<string, string>> creditCards = [];

        internal override void Submit(string requestType)
        {
            data.Clear();
            foreach (var fieldText in fields)
            {
                if (string.IsNullOrEmpty(fieldText.Value.Text))
                { 
                    GD.Print("The field \"" + fieldText.Key + "\" is null or empty"); 
                    continue;
                }
                data.Add(fieldText.Key, fieldText.Value.Text);
            }
            data.Add("requestType", requestType);
            if (optionButton.Selected != -1)
            {
                data.Add("accountType", optionButton.GetItemText(optionButton.Selected));
            }
            data.Add("isBankStaff", checkBox.ButtonPressed.ToString());
            data.Add("cookie", cookie);
            response = SVTerminal.Request(data);
            if (response == null) { response = []; response.Add("result", "failure"); }
            if(response != null && fields.TryGetValue("result", out DefaultField lineEdit) && response.TryGetValue("result", out string result))
            {
                if (result == "failure" || string.IsNullOrEmpty(result)) { lineEdit.Text = "Error"; }
                else { lineEdit.Text = "Success"; GD.Print("Success"); }
                //lineEdit.Text = result;
            } else if(fields.TryGetValue("result", out DefaultField _lineEdit)) { _lineEdit.Text = "Error"; GD.Print("Error"); }
            try { ActionOnResponse(requestType); } catch (Exception e) { SVTerminal.PrintException(e); }        
        }

        internal override void ActionOnResponse(string requestType)
        {
            response.TryGetValue("result", out string result);
            GD.Print(result);
            UpdateFields(response); 
            if(response.TryGetValue("data", out string data))
            {
                var parsedData = JsonDocument.Parse(data).RootElement.Deserialize<Dictionary<string, string>>();
                if (!parsedData.TryGetValue("accountInfo", out string accountInfo))
                {
                    return;
                }
                UpdateFields(JsonDocument.Parse(accountInfo).RootElement.Deserialize<Dictionary<string, string>>()!); 
            } 
      
            if(fields.TryGetValue("result", out DefaultField lineEdit))
            {
                if (result == "failure") { lineEdit.Text = "Error"; }
                //lineEdit.Text = result;
            } else { lineEdit.Text = "Success"; GD.Print("Success"); }
        }

        internal override void Activated()
        {
            // Submit("getInfo");
            GD.Print("Activated");
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
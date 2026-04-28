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

        private readonly Dictionary<string, Dictionary<string, string>> creditCards = [];

        internal override void Submit(string requestType)
        {
            base.Submit(requestType);
            // UpdateFields(response);
            if(response != null && response.TryGetValue("result", out string result) && fields.TryGetValue("result", out LineEdit lineEdit))
            {
                if (result == "failure") { lineEdit.Text = "Error"; }
                else { lineEdit.Text = "Success"; GD.Print("Success"); }
            } else if(fields.TryGetValue("result", out LineEdit _lineEdit)) { _lineEdit.Text = "Error"; GD.Print("Error"); }
            if (requestType != "getInfo"){ base.Submit("getInfo"); }
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
      
            if(fields.TryGetValue("result", out LineEdit lineEdit))
            {
                if (result == "failure") { lineEdit.Text = "Error"; }
                //lineEdit.Text = result;
            } else { lineEdit.Text = "Success"; GD.Print("Success"); }
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
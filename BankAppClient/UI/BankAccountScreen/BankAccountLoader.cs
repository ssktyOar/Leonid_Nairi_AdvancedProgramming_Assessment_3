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
        internal DefaultOptionButton optionButton;
        private long selectedOption = 0;
        public override void _Ready()
        {
            UITerminal.SetScreen(this, ScreenType.EditBankAccount);
            //optionButton = GetChild<DefaultOptionButton>(-1);
            //GD.Print(optionButton.Name);
            base._Ready();
        }

        private readonly Dictionary<string, Dictionary<string, string>> creditCards = [];

        internal override void Submit(string requestType)
        {
            base.Submit(requestType);
            UpdateFields(response);
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
                if(parsedData.TryGetValue("bankAccounts", out string a))
                {
                    try
                    {
                        BankAccounts(JsonDocument.Parse(a).RootElement.Deserialize<Dictionary<string, string>>());
                    }
                    catch (Exception e){ SVTerminal.PrintException(e); }
                } 
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
                if(parsedData.TryGetValue("bankAccounts", out string a))
                {
                    BankAccounts(JsonDocument.Parse(a).RootElement.Deserialize<Dictionary<string, string>>());
                }   
            }
        }

        private void BankAccounts(Dictionary<string, string> bankAcc)
        {
            GD.Print("Credit cards");
            creditCards.Clear();
            optionButton.Clear();
            int count = 0;
            foreach (var s in bankAcc)
            {
                creditCards.Add(s.Key, JsonDocument.Parse(s.Value).RootElement.Deserialize<Dictionary<string, string>>());
                optionButton.AddItem(s.Key, count);
                GD.Print(s.Key + ": " + s.Value);
                count++;
            }
            if (bankAcc.Count > selectedOption)
            { optionButton.Selected = (int)selectedOption; }
            else 
            { 
                selectedOption = creditCards.Keys.First().ToInt(); 
                optionButton.Selected = (int)selectedOption;
            }
            Select(optionButton.Selected);
        }

        internal void Select(long id)
        {
            if(creditCards.TryGetValue(optionButton.GetItemText((int)id), out Dictionary<string,string> value))
            {
                selectedOption = id;
                UpdateFields(value);
                GD.Print("Item selected: " + id.ToString());
            }
        }
    }
}
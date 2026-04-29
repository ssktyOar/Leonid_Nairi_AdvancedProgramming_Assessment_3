using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using BankAppClient.Core;
using Godot;

namespace BankAppClient.UI
{
    internal partial class DefaultForm : Control
    {
        public override void _Ready()
        {
            UITerminal.SetScreen(this);
            base._Ready();
        }
        private protected static string cookie = "c";
        // "Field": "data"
        private protected readonly Dictionary<string,DefaultField> fields = [];
        private protected readonly Dictionary<string,string> data = [];
        private protected static Dictionary<string,string> response = [];
        private protected string resultResponse = "";

        internal bool TryAddField(string fieldName, DefaultField field)
        {
            return fields.TryAdd(fieldName, field);
        }

        internal virtual void SwitchScreens(string screenName)
        {
            UITerminal.SwitchScreens(screenName);
        }

        internal virtual void SwitchScreens(Control controlWithScreenName)
        {
            UITerminal.SwitchScreens(controlWithScreenName.Name);
        }

        internal virtual void Submit(string requestType)
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
            if (requestType == "withdraw" || requestType == "deposit" || requestType == "addInterest")
            {
                data.Add("requestType", "transaction");
                data.Add("transactionType", requestType);
            }
            else{ data.Add("requestType", requestType); }
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

        internal virtual void ActionOnResponse(string requestType)
        {
            
        }

        internal virtual void Activated()
        {
            
        }

        private protected void UpdateFields(Dictionary<string,string> info)
        {
            if (info == null) { GD.Print("No responce revieved"); return; }
            int count = 0;
            foreach (var f in fields.Values) 
            { 
                List<string> s = ["sum", "result", "pin"];
                if(s.Contains(f.Name)){ continue; }
                f.Text = ""; 
            }
            foreach (var s in info)
            {
                if(fields.TryGetValue(s.Key, out DefaultField inputField))
                {
                    if(s.Key == "investmentRate") { inputField.Text = (decimal.Parse(s.Value) * 100).ToString().Split(".")[0] + "%"; count++; continue; }
                    if(s.Key == "balance") { inputField.Text = "$" + s.Value; count++; continue; }
                    if(s.Key == "overdraftLimit") { inputField.Text = "$" + s.Value; count++; continue; }
                    if(s.Key == "interest") { inputField.Text = "$" + s.Value; count++; continue; }
                    inputField.Text = s.Value;
                    count += 1;
                }
            }
            GD.Print("Fields updated: " + count.ToString());
        }
    }
}
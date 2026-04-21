using System;
using Godot;

namespace BankAppClient.UI
{
    internal enum ScreenType : int
    {
        Loading = 0,
        EnterUserAccount = 1,
        EditUserAccount = 2,
        CreateBankAccount = 5,
        EditBankAccount = 6,
        ChangeBankPin = 7
    }
}
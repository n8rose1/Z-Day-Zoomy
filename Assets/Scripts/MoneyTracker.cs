using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTracker
{
    private static readonly int commonRefund = 10;
    private static readonly int uncommonRefund = 20;
    private static readonly int rareRefund = 30;
    private static readonly int epicRefund = 50;
    private static readonly int legendaryRefund = 75;

    public MoneyTracker() {
       
    }

    public static void AddMoneyToBank(int amount) {
        PlayerPrefs.SetInt(PlayerPreferences.money,
            PlayerPrefs.GetInt(PlayerPreferences.money, 0) + amount);
    }

    public static void SpendMoney(int amount) {
        PlayerPrefs.SetInt(PlayerPreferences.money,
            PlayerPrefs.GetInt(PlayerPreferences.money, 0) - amount);
    }

    public static int GetBalance() {
        return PlayerPrefs.GetInt(PlayerPreferences.money, 0);
    }

    public static void Refund(int rarity) {
        switch (rarity) {
            case 0:
                AddMoneyToBank(commonRefund);
                break;
            case 1:
                AddMoneyToBank(uncommonRefund);
                break;
            case 2:
                AddMoneyToBank(rareRefund);
                break;
            case 3:
                AddMoneyToBank(epicRefund);
                break;
            case 4:
                AddMoneyToBank(legendaryRefund);
                break;
        }
    }
}

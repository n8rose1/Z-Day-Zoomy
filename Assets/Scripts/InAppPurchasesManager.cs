using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class InAppPurchasesManager : MonoBehaviour
{
    [SerializeField] ScrollRectSnap scrollRectSnap;
    [SerializeField] GameObject restorePurchaseButton;

    private void Awake() {
        DisableRestorePurchaseButton();
    }

    public void PurchaseCharacter(Product character) {
        PlayerPrefs.SetInt(PlayerPreferences.hasCharacter + character.definition.payout.data, NewCharacters.COLLECTED);
        Debug.Log("Collected player " + character.definition.id);
        scrollRectSnap.UpdateScreen();
    }

    private void DisableRestorePurchaseButton() {
        if (Application.platform != RuntimePlatform.IPhonePlayer) {
            restorePurchaseButton.SetActive(false);
        }
    }
}

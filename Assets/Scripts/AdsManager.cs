using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener {
    [SerializeField] Button _showAdButton;
    [SerializeField] Button _showSecondLifeButton;
    [SerializeField] GameObject _showAdButtonContainer;
    [SerializeField] GameController gameController;
    [SerializeField] int rewardAmount = 10;
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] string _androidInterstitial = "Interstitial_Android";
    [SerializeField] string _iOSInterstitial = "Interstitial_iOS";
    [SerializeField] string _androidReward = "Rewarded_Android";
    [SerializeField] string _iOSReward = "Rewarded_iOS";
    [SerializeField] bool _testMode = true;
    private string _gameId;
    private string _adUnitIdInterstitial;
    private string _adUnitIdReward;

    private bool adsAreReady = false;

    private string adPurpose;
    private string secondLife = "secondLife";
    private string earnCash = "earnCash";

    void Awake() {

        InitializeAds();
/*
#if UNITY_IOS
        _adUnitIdReward = _iOSReward;
        _adUnitIdInterstitial = _iOSInterstitial;
#elif UNITY_ANDROID
        _adUnitIdReward = _androidReward;
        _adUnitIdInterstitial = _androidInterstitial;
#endif */

        _showAdButton.interactable = false;
        _showSecondLifeButton.interactable = false;
    }

    
    public void InitializeAds() {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _gameId = _iOSGameId;
            _adUnitIdInterstitial = _iOSInterstitial;
            _adUnitIdReward = _iOSReward;
        } else {
            _gameId = _androidGameId;
            _adUnitIdInterstitial = _androidInterstitial;
            _adUnitIdReward = _androidReward;
        }
        Advertisement.Initialize(_gameId, _testMode, this);
    } 

    public void LoadInterstitialAd() {
        Debug.Log("Loading Ad: " + _adUnitIdInterstitial);
        Advertisement.Load(_adUnitIdInterstitial, this);
        
    }

    public void ShowInterstitialAd() {
        AudioListener.volume = 0;
        Debug.Log("Showing Ad: " + _adUnitIdInterstitial);
        Advertisement.Show(_adUnitIdInterstitial, this);
    }

    public void LoadRewardAd() {
        Debug.Log("Loading Ad: " + _adUnitIdReward);
        Advertisement.Load(_adUnitIdReward, this);
    }

    public void GetSecondLife() {
        adPurpose = secondLife;
        ShowRewardAd();
    }

    public void EarnCash() {
        adPurpose = earnCash;
        ShowRewardAd();
    }

    public void ShowRewardAd() {
        AudioListener.volume = 0;
        Debug.Log("Showing Ad: " + _adUnitIdReward);
        Advertisement.Show(_adUnitIdReward, this);
    }

    public void OnInitializationComplete() {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message) {
        Debug.Log($"Unity Ads Initialization Failed: {error} - {message}");
    }

    
    public void OnUnityAdsAdLoaded(string placementId) {
        Debug.Log("Ad Loaded: " + placementId);
        if (placementId.Equals(_adUnitIdReward)) {
            adsAreReady = true;
            _showAdButton.onClick.AddListener(EarnCash);
            _showAdButton.interactable = true;
            _showSecondLifeButton.onClick.AddListener(GetSecondLife);
            _showSecondLifeButton.interactable = true;
            //_showAdButtonContainer.SetActive(true);
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowStart(string placementId) {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowClick(string placementId) {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) {
        Debug.Log("Finished Ad: " + placementId);
        Debug.Log("Current ID: " + _adUnitIdReward);
        Debug.Log("Completion State: " + showCompletionState);
        Debug.Log("Completion State Enum: " + UnityAdsCompletionState.COMPLETED);
        if (placementId.Equals(_adUnitIdReward) && showCompletionState == UnityAdsShowCompletionState.COMPLETED) {
            Debug.Log("Completed Reward Ad, and entered if statement");
            LoadRewardAd();
            if (adPurpose.Equals(earnCash)) {
                MoneyTracker.AddMoneyToBank(rewardAmount);
            } else if (adPurpose.Equals(secondLife)) {
                FindObjectOfType<GameController>().FinishedSecondLifeAd();
            }
        }
    }

    public bool AdsAreReady() {
        return adsAreReady;
    }

    void OnDestroy() {
        _showAdButton.onClick.RemoveAllListeners();
        _showSecondLifeButton.onClick.RemoveAllListeners();
        //Advertisement.RemoveListener(this);
    }

    public void OnUnityAdsReady(string placementId) {
        Debug.Log("Ad Loaded: " + placementId);
       /* if (placementId.Equals(_adUnitIdReward)) {
            showAdButtonContainer.SetActive(true);
        } */
    }

    public void OnUnityAdsDidError(string message) {
        
    }

    public void OnUnityAdsDidStart(string placementId) {
        
    }

    /*
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult) {
        AudioListener.volume = 1;
        Debug.Log("Finished Ad: " + placementId + " with completion state: " + showResult);
        if (placementId.Equals(_adUnitIdReward)) {
            LoadRewardAd();
            if (showResult == ShowResult.Finished) {
                //showAdButtonContainer.SetActive(false);
                Debug.Log("Unity Ads Rewarded Ad Completed");
                MoneyTracker.AddMoneyToBank(rewardAmount);
            }
        }
    } */

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) {
        //throw new System.NotImplementedException();
    }
}

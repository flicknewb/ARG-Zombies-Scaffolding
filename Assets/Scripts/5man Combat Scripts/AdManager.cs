﻿using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class AdManager : MonoBehaviour {

	public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show("", new ShowOptions(){resultCallback = HandleAdResult});

			Analytics.CustomEvent("Show Ad chosen", new Dictionary<string, object>
				{
					{"userID", GameManager.instance.userId},
					{"time_alive", GameManager.instance.GetCurrentTimeAlive()}
				});
        } 
    }

    private void HandleAdResult(ShowResult result)
    {
        BattleStateMachine BSM = FindObjectOfType<BattleStateMachine>();
        switch (result)
        {

            case ShowResult.Finished:
                //restore survivor
                if (BSM != null)
                {
                    BSM.PlayerChoosePurchaseSurvivorSave();
                }else
                {
                    Debug.Log("unable to find battlestatemachine to reward player for successful ad watch");
                }
                break;

            case ShowResult.Skipped:
                //unbite- but don't restore survivor.
                if (BSM != null)
                {
                    BSM.PlayerPartiallyWatchedAD();
                }else
                {
                    Debug.Log("unable to find battlestatemachine to reward player for partial ad watch");
                }

                break;
            case ShowResult.Failed:
                //kill the survivor horribler...

			Analytics.CustomEvent("AdWatch Failed", new Dictionary<string, object>
				{
					{"userID", GameManager.instance.userId},
					{"time_alive", GameManager.instance.GetCurrentTimeAlive()}
				});
                break;
        }
    }

    public void ShowZombieAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show("", new ShowOptions() { resultCallback = HandleZombieAdResult });
        }
    }

    private void HandleZombieAdResult(ShowResult result)
    {
        ZombieModeManager ZMM = FindObjectOfType<ZombieModeManager>(); //this is the level manager for zombie mode
        switch (result)
        {

            case ShowResult.Finished:
                //notify counter on level manager
                if (ZMM != null)
                {
                    ZMM.ZombieAdFinished();
                }
                else
                {
                    Debug.Log("unable to find battlestatemachine to reward player for successful ad watch");
                }
                break;

            case ShowResult.Skipped:
                //unbite- but don't restore survivor.
                if (ZMM != null)
                {
                    ZMM.AdPartialWatch();
                }
                else
                {
                    Debug.Log("unable to find battlestatemachine to reward player for partial ad watch");
                }

                break;
            case ShowResult.Failed:
                //kill the survivor horribly-er than otherwise...
                Debug.Log("Ad Failed to run");
                break;
        }
    }
}

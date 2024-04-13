using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamManager : MonoBehaviour
{
    public static SteamManager Instance { get; set; }
    public const uint AppId = 480;
    bool steamSucceeded = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        try
        {
            SteamClient.Init(AppId, false);
            CheckIfLaunchedOutsideSteam();
            steamSucceeded = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Steamworks failed to initialize: " + e.Message);
            // Something went wrong! Steam is closed?
        }
    }

    void CheckIfLaunchedOutsideSteam()
    {
        try
        {
            if (SteamClient.RestartAppIfNecessary(AppId))
            {
                Application.Quit();
            }
        }
        catch (Exception)
        {
            Application.Quit();
        }
    }

    private void Start()
    {
        if (steamSucceeded)
        {
            Instance = this;
            Debug.Log($"Player name: {SteamClient.Name}, SteamID: {SteamClient.SteamId}, From: {SteamUtils.IpCountry}");
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        SteamClient.RunCallbacks();
    }

    private void OnApplicationQuit()
    {
        SteamFriends.ClearRichPresence();
        SteamClient.Shutdown();
    }
}

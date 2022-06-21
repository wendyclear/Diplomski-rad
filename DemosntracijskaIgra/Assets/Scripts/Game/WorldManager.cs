using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class WorldManager : MonoBehaviourPunCallbacks
{
    public static WorldManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    //the only 2 methods that need base call in override
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded; //every time a scene is loaded, OnSceneLoaded method is called
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //check if the player is loading into game scene
        if (scene.buildIndex == 1)
        {
            //Instantiate object via string (rather that via prefab - via photon) - photon prefabs ened to be in Resources folder
            PhotonNetwork.Instantiate(Path.Combine("NetworkedPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

}

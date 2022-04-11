using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    private PhotonView _photonView;
    private GameObject _player;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void CreateController()
    {
        //create player controller
        Transform spawnPoint = PlayerSpawnManager.Instance.getSpawnPoint();
        _player = PhotonNetwork.Instantiate(Path.Combine("NetworkedPrefabs", "Player"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { _photonView.ViewID }); //list that contains PM view ID, it will help player find PM
        //_player = PhotonNetwork.Instantiate(Path.Combine("NetworkedPrefabs", "Player"), Vector3.zero, Quaternion.identity, 0, new object[] { _photonView.ViewID }); //list that contains PM view ID, it will help player find PM
    }

    private void Start()
    {
        if (_photonView.IsMine)
        {
            CreateController();
        }
    }

    public void Die()
    {
        PhotonNetwork.Destroy(_player);
        CreateController();
    }
}

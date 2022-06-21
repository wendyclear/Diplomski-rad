using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private List<Material> _playerMaterials;
    private int _currentColor = 0;
    [SerializeField] private GameObject _player;
    private MeshRenderer _meshRenderer;
    [SerializeField] PhotonView _photonView;

    private void Awake()
    {
        _meshRenderer = _player.GetComponent<MeshRenderer>();
    }

    public void ChangeColor()
    {
        if (_playerMaterials.Count > 0)
        {
            _currentColor = (_currentColor + 1) % _playerMaterials.Count;
            _meshRenderer.material = _playerMaterials[_currentColor];
            _photonView.RPC("RPC_ChangeColor", RpcTarget.All, _currentColor);
        }
    }

    [PunRPC]

    void RPC_ChangeColor(int color)
    {
        _meshRenderer.material = _playerMaterials[color];
    }
}

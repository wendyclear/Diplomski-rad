using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _scoreboardCanvas;
    [SerializeField] private GameObject _gameMenuCanvas;
    private PhotonView _playerPV;

    private void Awake()
    {
        CursorLock(true);
        _scoreboardCanvas.alpha = 0;
        _gameMenuCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            if (_scoreboardCanvas.alpha > 0)
            {
                Debug.Log("Closing");
                //close canvas
                _scoreboardCanvas.alpha = 0;
                CursorLock(true);
            }
            else
            {
                Debug.Log("Opening");
                //open canvas
                _scoreboardCanvas.alpha = 1;
                _gameMenuCanvas.SetActive(false);
                CursorLock(false);
            }
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (_gameMenuCanvas.activeInHierarchy)
            {
                //close canvas
                Debug.Log("Closing");
                _gameMenuCanvas.SetActive(false);
                CursorLock(true);
            }
            else
            {
                //open canvas
                Debug.Log("Opening");
                _scoreboardCanvas.GetComponent<CanvasGroup>().alpha = 0;
                _gameMenuCanvas.SetActive(true);
                CursorLock(false);
            }
        }
    }

    private void CursorLock(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void onContinueButtonPressed()
    {
        _gameMenuCanvas.SetActive(false);
        CursorLock(true);
    }

    public void onReturnButtonPressed()
    {
        PhotonView.Find((int)_playerPV.InstantiationData[0]).GetComponent<PlayerManager>().DieAndLeave();
        Destroy(WorldManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }

    public void onExitButtonPressed()
    {
        Application.Quit();
    }

    public void SetPlayerPV(PhotonView pv)
    {
        _playerPV = pv;
    }
}

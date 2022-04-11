using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] private GameObject _cameraHolder;
    private Rigidbody _rigidbody;

    [SerializeField] private float _runSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _smoothTime;
    [SerializeField] private float _rotateSensitivity;

    [SerializeField] private Item[] _items;
    private int _currentlyEquippedItemIndex = -1;
    private int _previousItemIndex = -1;

    private float _verticalRotation;
    private bool _onFloor;
    private Vector3 _smoothMoveVelocity;
    private Vector3 _moveAmount;

    private float _maxRotation;
    private PhotonView _photonView;

    private float _health = 100f;

    private PlayerManager _playerManager;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();
        _playerManager = PhotonView.Find((int)_photonView.InstantiationData[0]).GetComponent<PlayerManager>();
        _maxRotation = 90f;
        _onFloor = true;
    }

    private void Start()
    {
        if (!_photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(_rigidbody);
        }
        else
        {
            EquipItem(0);
        }
    }

    private void Update()
    {
        if (_photonView.IsMine)
        {
            Look();
            Move();
            Jump();
            SwitchWeapon();
            UseItem();
        }
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * _rotateSensitivity);
        _verticalRotation += Input.GetAxisRaw("Mouse Y") * _rotateSensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -_maxRotation, _maxRotation);
        _cameraHolder.transform.localEulerAngles = Vector3.left * _verticalRotation;
    }

    private void Move()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        _moveAmount = Vector3.SmoothDamp(_moveAmount, moveDirection * (Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _walkSpeed), ref _smoothMoveVelocity, _smoothTime);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _onFloor)
        {
            _rigidbody.AddForce(transform.up * _jumpForce);
        }
    }

    public void OnFloor(bool onFloor)
    {
        _onFloor = onFloor;
    }

    private void FixedUpdate()
    {
        //all physics and movement here, so movement isnt impacted by FPS
        if (_photonView.IsMine)
        {
            _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(_moveAmount * Time.deltaTime));
        }
    }

    private void EquipItem(int index)
    {
        if (index == _currentlyEquippedItemIndex) return;
        if (index < 0) _currentlyEquippedItemIndex = _items.Length - 1;
        else if (index > (_items.Length - 1)) _currentlyEquippedItemIndex = 0;
        else _currentlyEquippedItemIndex = index;
        _items[_currentlyEquippedItemIndex]._itemGameObject.SetActive(true);

        if (_previousItemIndex != -1)
        {
            _items[_previousItemIndex]._itemGameObject.SetActive(false);
        }
        _previousItemIndex = _currentlyEquippedItemIndex;

        //syncing - sending info to other players
        if (_photonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", _currentlyEquippedItemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        }
    }

    private void SwitchWeapon()
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }

        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            //scrolling up 
            EquipItem(_currentlyEquippedItemIndex + 1);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            //scrolling up 
            EquipItem(_currentlyEquippedItemIndex + -1);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (!_photonView.IsMine && targetPlayer == _photonView.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _items[_currentlyEquippedItemIndex].Use();
        }
    }

    public void TakeDamage(float damage)
    {
        _photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]

    void RPC_TakeDamage(float damage)
    {
        if (!_photonView.IsMine)
        {
            return;
        }
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _playerManager.Die();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "InstaDeath")
        {
            Die();
        }

    }
}

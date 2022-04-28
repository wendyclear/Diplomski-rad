using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shotgun : Gun
{

    [SerializeField] private Camera _camera;
    private Ray _shootingRay;
    private Vector3 _shootingDir = new Vector3(0.5f, 0.5f);
    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = this.GetComponent<PhotonView>();
    }

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        //shoot towards middle of the screen
        _shootingRay = _camera.ViewportPointToRay(_shootingDir);
        _shootingRay.origin = _camera.transform.position;
        if (Physics.Raycast(_shootingRay, out RaycastHit hit))
        {
            hit.collider.gameObject.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)_itemInfo).damage, _photonView.Owner);
            _photonView.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }

    }

    [PunRPC]
    void RPC_Shoot(Vector3 hit, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hit, 0.2f);
        if (colliders.Length != 0)
        {
            GameObject bulletImp = Instantiate(_bulletHitPrefab, hit + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * _bulletHitPrefab.transform.rotation);
            bulletImp.transform.SetParent(colliders[0].transform);
            //destroy after 10 seconds
            Destroy(bulletImp, 10f);
        }
    }
}

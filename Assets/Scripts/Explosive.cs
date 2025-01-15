using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Explosive : MonoBehaviour
{
    [HideInInspector]
    public bool isLocalExplosive = false;


    [Header("States")]
    public float explosionRadius = 5f;
    public int damage = 30;


    [Header("Fire Settings")]
    public float fireForce;


    [Header("VFX")]
    public GameObject explosionVFX;


    public static bool isExploded;


    private bool alreadyExploded = false;



    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * fireForce);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!isLocalExplosive)
            return;

        PhotonNetwork.Instantiate(explosionVFX.name, transform.position, Quaternion.identity);

        isExploded = true;

        // Explode Function

        Explode();
    }


    private void Explode()
    {
        if (alreadyExploded)
            return;

        alreadyExploded = true;

        foreach (var collider in Physics.OverlapSphere(transform.position, explosionRadius))
        {
            if (collider.transform.gameObject.GetComponent<Health>() && collider.transform.gameObject.GetComponent<Health>().isLocalPlayer == false)
            {
                PhotonNetwork.LocalPlayer.AddScore(damage);

                if (damage >= collider.transform.gameObject.GetComponent<Health>().health)
                {
                    //Kill

                    RoomManager.instance.kills++;
                    RoomManager.instance.SetHashes();

                    PhotonNetwork.LocalPlayer.AddScore(100);
                }


                collider.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }
        }

        PhotonNetwork.Destroy(gameObject);
    }

}

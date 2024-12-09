using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;

public class Weapon : MonoBehaviour
{

    private Animator anim;

    public Camera camera;

    public int damage;

    public float fireRater;


    [Header("VFX")]
    public GameObject hitVFX;

    private float nextFire;


    [Header("Ammo")]
    public int mag = 5;
    public int ammo = 30;
    public int magAmmo = 30;

    [Header("UI")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;


    private enum movementState { idle, run, fullReload, reload }

    private movementState state;

    private bool isReloading, isFull, isRunning;


    private void Start()
    {
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;

        anim = GetComponent<Animator>();
    }


    void Update()
    {
        UpdateAnimationState();

        if (nextFire > 0)
            nextFire -= Time.deltaTime;


        if (Input.GetButton("Fire1") && nextFire <= 0 && ammo > 0 && isReloading == false)
        {
            nextFire = 1 / fireRater;

            ammo--;

            magText.text = mag.ToString();
            ammoText.text = ammo + "/" + magAmmo;

            Fire();
        }

        if (Input.GetKeyDown(KeyCode.R) && ammo != 30)
        {
            Reload();
        }
    }

    private void Reload()
    {
        if (mag > 0)
        {
            if (ammo == 0)
            {
                isFull = true;
            }
            else { isFull = false; }

            StartCoroutine(Reloading());

            mag--;

            ammo = magAmmo;
        }

        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
    }



    private void Fire()
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
        {
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

            if (hit.transform.gameObject.GetComponent<Health>())
            {
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
    }

    
    
    IEnumerator Reloading()
    {
        isReloading = true;

        if (isFull)
        {
            yield return new WaitForSeconds(3.5f);
        }
        else
        {
            yield return new WaitForSeconds(2.8f);
        }

        isReloading = false;
    }



    private void UpdateAnimationState()
    {
        if (isReloading && isFull)
        {
            state = movementState.fullReload;
        }
        else if (isReloading && !isFull)
        {
            state = movementState.reload;
        }
        else
        {
            state = movementState.idle;
        }

        anim.SetInteger("state", (int)state);
    }

}

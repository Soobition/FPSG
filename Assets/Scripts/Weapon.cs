using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

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


    private enum movementState { idle, run, fullReload, reload_1, reload_2, walk }

    private movementState state;

    private bool isReloading, isFull, isOne, isRunning;


    public static bool isWalking, isGrounded;


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
            isRunning = false;

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


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }
    }


    private void RandomNum()
    {
        int randomRange = Random.Range(1, 3);

        if (randomRange == 1)
        {
            isOne = true;
        }
        else { isOne = false; }
    }


    private void Reload()
    {
        if (mag > 0)
        {
            if (ammo == 0)
            {
                isFull = true;
            }
            else
            {
                isFull = false;
                
                RandomNum();
            }

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
            yield return new WaitForSeconds(2.7f);
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
            if (isOne)
            {
                state = movementState.reload_1;
            }
            else { state = movementState.reload_2; }
        }
        else if (isRunning && isGrounded)
        {
            state = movementState.run;
        }
        else if (isWalking && !isRunning)
        {
            state = movementState.walk;
        }
        else
        {
            state = movementState.idle;
        }

        anim.SetInteger("state", (int)state);
    }

}

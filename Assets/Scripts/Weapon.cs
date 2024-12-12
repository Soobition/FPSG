using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class Weapon : MonoBehaviour
{

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

    [Header("Recoil Settings")]
    /*[Range(0, 1)]
    public float recoilPercent = 0.3f;*/

    [Range(0, 2)]
    public float recoverPercent = .7f;

    [Space]
    public float recoilUp = 1f;
    public float recoilBack = 0f;

    [Header("Holster Settings")]
    public float holsterUp = -3f;



    public static bool isWalking, isGrounded, isHolsterUp, isHolsterDown;



    private Animator anim;


    private Vector3 originalPosition;

    private Vector3 recoilVelocity = Vector3.zero;


    private Quaternion originalRotation;


    private float recoilLength;

    private float recoverLength;

    
    private enum movementState { idle, run, fullReload, reload_1, reload_2, walk }

    private movementState state;


    private bool isReloading, isFull, isOne, isRunning, recoiling, recovering;



    private void Start()
    {
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;


        anim = GetComponent<Animator>();


        originalPosition = transform.localPosition;

        originalRotation = transform.localRotation;


        recoilLength = 0;

        recoverLength = 1 / fireRater * recoverPercent;
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


        if (recoiling)
        {
            Recoil();
        }


        if (recovering)
        {
            Recovering();
        }
    }


    private void FixedUpdate()
    {
        if (isHolsterUp)
        {
            HolsterUp();
        }
        else if (isHolsterDown)
        {
            HolsterDown();
        }
    }


    private void HolsterUp()
    {
        Quaternion finalRotation = new Quaternion(originalRotation.x + holsterUp, originalRotation.y, originalRotation.z, originalRotation.w);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, finalRotation, Time.deltaTime * 10f);

        if (transform.localRotation == finalRotation)
        {
            isHolsterUp = false;

            isHolsterDown = true;
        }
    }


    private void HolsterDown()
    {
        Quaternion finalRotation = originalRotation;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, finalRotation, Time.deltaTime * 10f);

        if (transform.localRotation == finalRotation)
        {
            isHolsterUp = false;

            isHolsterDown = false;
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
        recoiling = true;

        recovering = false;


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



    private void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + recoilUp, originalPosition.z - recoilBack);


        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);


        if (transform.localPosition == finalPosition)
        {
            recoiling = false;

            recovering = true;
        }
    }


    private void Recovering()
    {
        Vector3 finalPosition = originalPosition;


        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLength);


        if (transform.localPosition == finalPosition)
        {
            recoiling = false;

            recovering = false;
        }
    }

    
    
    IEnumerator Reloading()
    {
        isReloading = true;

        if (isFull)
        {
            if (gameObject.name == ("FpsAnims"))
            {
                yield return new WaitForSeconds(3.5f);
            }
            else { yield return new WaitForSeconds(2.8f); }
        }
        else
        {
            if (gameObject.name == ("FpsAnims"))
            {
                yield return new WaitForSeconds(2.7f);
            }
            else { yield return new WaitForSeconds(2f); }
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
            if (gameObject.name == ("FpsAnims"))
            {
                if (isOne)
                {
                    state = movementState.reload_1;
                }
                else { state = movementState.reload_2; }
            }
            else { state = movementState.reload_1; }
        }
        else if (isRunning && isGrounded && isWalking)
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

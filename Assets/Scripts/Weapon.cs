using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Photon.Pun.UtilityScripts;

public class Weapon : MonoBehaviour
{

    public Image ammoCircle; 


    public Camera camera;


    public int damage;
    public int pelletsCount = 1;


    public float sprayMultiplayer = 0f;
    public float fireRater;


    [Header("Projectile Weapon Settings")]
    public bool isProjectileWeapon = false;
    public GameObject projectile;
    public Transform projectileExit;


    [Header("VFX")]
    public GameObject hitVFX;

    [Space]
    public GameObject muzzleFlashVFX;
    public Transform muzzleFlashPoint;
    public float muzzleFlashDuration = 0.1f;


    [Header("Ammo")]
    public int mag = 5;
    public int ammo = 30;
    public int magAmmo = 30;


    [Header("SFX")]
    public int shootSFXIndex = 0;
    public int emptyMagSFXIndex = 0;
    public PlayerPhotonSoundManager playerPhotonSoundManager;


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



    public static bool isWalking, isGround, isHolsterUp, isHolsterDown, isRunning, isEmpty;



    private Animator anim;


    private float nextFire;


    private Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;


    private Quaternion originalRotation;


    private float recoilLength;
    private float recoverLength;

    
    private enum movementState { idle, run, fullReload, reload_1, reload_2 }
    private movementState state;


    private bool isReloading, isFull, isOne, recoiling, recovering;



    private void SetAmmo()
    {
        ammoCircle.fillAmount = (float)ammo / magAmmo;
    }



    private void Start()
    {
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
        SetAmmo();


        anim = GetComponent<Animator>();


        originalPosition = transform.localPosition;

        originalRotation = transform.localRotation;


        recoilLength = 0;

        recoverLength = 1 / fireRater * recoverPercent;
    }


    void Update()
    {
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
        SetAmmo();


        if (gameObject.name == "FpsAnims")
        {
            PlayerSetup.isAK = true;
            PlayerSetup.isPistol = false;
            PlayerSetup.isShotgun = false;
            PlayerSetup.isLauncher = false;
        }
        else if (gameObject.name == "FPS_Anims")
        {
            PlayerSetup.isAK = false;
            PlayerSetup.isPistol = true;
            PlayerSetup.isShotgun = false;
            PlayerSetup.isLauncher = false;
        }
        else if (gameObject.name == "shotgunAnimated")
        {
            PlayerSetup.isAK = false;
            PlayerSetup.isPistol = false;
            PlayerSetup.isShotgun = true;
            PlayerSetup.isLauncher = false;
        }
        else
        {
            PlayerSetup.isAK = false;
            PlayerSetup.isPistol = false;
            PlayerSetup.isShotgun = false;
            PlayerSetup.isLauncher = true;
        }



        UpdateAnimationState();


        if (nextFire > 0)
            nextFire -= Time.deltaTime;


        if (Input.GetButton("Fire1") && nextFire <= 0 && ammo > 0 && isReloading == false)
        {
            StartCoroutine(MuzzleFlashVFX());

            isRunning = false;

            nextFire = 1 / fireRater;

            ammo--;

            /*magText.text = mag.ToString();
            ammoText.text = ammo + "/" + magAmmo;
            SetAmmo();*/


            if (isProjectileWeapon)
            {
                ProjectileFire();
            }
            else { Fire(); }
        }
        else if (Input.GetButton("Fire1") && ammo == 0)
        {
            playerPhotonSoundManager.PlayEmptyMagSFX(emptyMagSFXIndex);
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gameObject.name == ("FpsAnims") && ammo != 30)
            {
                Reload();
            }
            else if (gameObject.name == ("FPS_Anims") && ammo != 15)
            {
                Reload();
            }
            else if (gameObject.name == ("shotgunAnimated") && ammo != 1 || gameObject.name == ("Launcher") && ammo != 1)
            {
                Reload();
            }
        }


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!Movement.isbackwards)
            {
                isRunning = true;
            }
            else { isRunning = false; }
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

        if (ammo > 0)
        {
            isEmpty = false;
        }
        else { isEmpty = true; }
    }


    private IEnumerator MuzzleFlashVFX()
    {
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashVFX, muzzleFlashPoint.position, muzzleFlashPoint.rotation);
            muzzleFlash.transform.parent = muzzleFlashPoint; // Parent the flash to the muzzle point for better control

            if (gameObject.name == ("Launcher"))
            {
                yield return new WaitForSeconds(muzzleFlashDuration * 3);
            }
            else
            {
                yield return new WaitForSeconds(muzzleFlashDuration);
            }

            Destroy(muzzleFlash);
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


            if (isReloading)
            {
                isReloading = false;

                StartCoroutine(Reloading());
            }

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



    private void ProjectileFire()
    {
        GameObject myProjectile = PhotonNetwork.Instantiate(projectile.name, projectileExit.position, projectileExit.rotation);
        myProjectile.GetComponent<Explosive>().isLocalExplosive = true;

        playerPhotonSoundManager.PlayShootSFX(shootSFXIndex);
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

        /*magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
        SetAmmo();*/
    }



    private void Fire()
    {
        recoiling = true;

        recovering = false;


        playerPhotonSoundManager.PlayShootSFX(shootSFXIndex);


        for (int i = 0; i < pelletsCount; i++)
        {
            Vector3 sprayOffset = Random.insideUnitCircle * sprayMultiplayer;
            sprayOffset.z = 0;


            Ray ray = new Ray(camera.transform.position, camera.transform.forward + sprayOffset);

            RaycastHit hit;

            PhotonNetwork.LocalPlayer.AddScore(1);


            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
            {
                PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

                if (hit.transform.gameObject.GetComponent<Health>() && hit.transform.gameObject.GetComponent<Health>().isLocalPlayer == false)
                {
                    PhotonNetwork.LocalPlayer.AddScore(damage);

                    if (damage >= hit.transform.gameObject.GetComponent<Health>().health)
                    {
                        //Kill

                        RoomManager.instance.kills++;
                        RoomManager.instance.SetHashes();

                        PhotonNetwork.LocalPlayer.AddScore(100);
                    }


                    hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                }
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
            else if (gameObject.name == ("FPS_Anims"))
            {
                yield return new WaitForSeconds(2.8f);
            }
            else if (gameObject.name == ("shotgunAnimated"))
            {
                yield return new WaitForSeconds(1.8f);
            }
            else { yield return new WaitForSeconds(4.1f); }
        }
        else
        {
            if (gameObject.name == ("FpsAnims"))
            {
                yield return new WaitForSeconds(2.7f);
            }
            else if (gameObject.name == ("FPS_Anims"))
            {
                yield return new WaitForSeconds(2f);
            }
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
        else if (isRunning && isGround && isWalking)
        {
            state = movementState.run;
        }
        else
        {
            state = movementState.idle;
        }

        anim.SetInteger("state", (int)state);
    }

}

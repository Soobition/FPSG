using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.
    //public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    private void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    private void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    private void Update()
    {
        if (Weapon.isRunning && Weapon.isGround && Weapon.isWalking || Input.GetButton("Fire1") && !Weapon.isEmpty)
        {
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalPos + Random.insideUnitSphere * shakeAmount, Time.deltaTime * 3);
        }
        else if (Explosive.isExploded)
        {
            StartCoroutine(StopShaking());

            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalPos + Random.insideUnitSphere * shakeAmount * 2, Time.deltaTime * 3);

           // shakeDuration -= Time.deltaTime * decreaseFactor;

        }
        else
        {
           // shakeDuration = 0f;
            camTransform.localPosition = originalPos;
        }
    }


    private IEnumerator StopShaking()
    {
        yield return new WaitForSeconds(.8f);

        Explosive.isExploded = false;
    }
}
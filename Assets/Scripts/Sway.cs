using UnityEngine;

public class Sway : MonoBehaviour
{
    [Header("Settings")]
    public float swayClamp = .09f;

    [Space]
    public float smoothing = 3f;


    private Vector3 origin;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        origin = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        input.x = Mathf.Clamp(input.x, -swayClamp, swayClamp);
        input.y = Mathf.Clamp(input.y, -swayClamp, swayClamp);

        Vector3 target = new Vector3(-input.x, -input.y, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, target + origin, Time.deltaTime * smoothing);
    }
}

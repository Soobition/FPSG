using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 8f;
    [SerializeField] private float sprintSpeed = 14f;
    [SerializeField] private float maxVelocityChange = 10f;

    [Space]
    [SerializeField] private float airControl = .5f;

    [Space]
    [SerializeField] private float jumpHeight = 5f;


    private Vector2 input;


    private Rigidbody rb;


    private bool sprinting;
    private bool jumping;

    private bool grounded = false;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }



    private void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical") * 3);
        input.Normalize();

        sprinting = Input.GetButton("Fire3");
        jumping = Input.GetButton("Jump");
    }


    private void OnTriggerStay(Collider other)
    {
        grounded = true;
    }



    private void FixedUpdate()
    {
        if (grounded)
        {
            if (jumping)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight, rb.linearVelocity.z);
            }
            else if (input.magnitude > .5f)
            {
                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed : walkSpeed), ForceMode.VelocityChange);
            }
            else
            {
                var velocity1 = rb.linearVelocity;
                velocity1 = new Vector3(velocity1.x * .2f * Time.fixedDeltaTime, velocity1.y, velocity1.z * .2f * Time.fixedDeltaTime);
                rb.linearVelocity = velocity1;
            }
        }
        else
        {
            if (input.magnitude > .5f)
            {
                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed * airControl : walkSpeed * airControl), ForceMode.VelocityChange);
            }
            else
            {
                var velocity1 = rb.linearVelocity;
                velocity1 = new Vector3(velocity1.x * .2f * Time.fixedDeltaTime, velocity1.y, velocity1.z * .2f * Time.fixedDeltaTime);
                rb.linearVelocity = velocity1;
            }
        }

        grounded = false;
    }



    private Vector3 CalculateMovement(float _speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity) * _speed;

        Vector3 velocity = rb.linearVelocity;


        if (input.magnitude > .5f)
        {
            Vector3 velocityChange = targetVelocity - velocity;

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

            velocityChange.y = 0;


            return (velocityChange);
        }
        else
        {
            return Vector3.zero;
        }
    }
}

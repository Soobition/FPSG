using UnityEngine;

public class Movement : MonoBehaviour
{
    public float walkSpeed = 6f;
    public float sprintSpeed = 10f;
    public float maxVelocityChange = 10f;

    [Space]
    public float airControl = .5f;

    [Space]
    public float jumpHeight = 5f;

    [Header("Animation")]
    public Animation handAnimation;
    public AnimationClip handWalkAnimation;
    public AnimationClip handIdleAnimation;


    public static bool isbackwards;



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


        if (grounded)
        {
            Weapon.isGround = true;
        }
        else { Weapon.isGround = false; }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Obstacle"))
        {
            grounded = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Obstacle"))
        {
            grounded = false;
        }
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
                Weapon.isWalking = true;

                handAnimation.clip = handWalkAnimation;
                handAnimation.Play();

                if (Input.GetButton("Fire1") && !Weapon.isEmpty)
                {
                    rb.AddForce(CalculateMovement(sprinting ? sprintSpeed - 3.5f : walkSpeed), ForceMode.VelocityChange);
                }
                else
                { 
                    if (input.y < 0)
                    {
                        if (sprinting || !sprinting)
                        {
                            rb.AddForce(CalculateMovement(walkSpeed), ForceMode.VelocityChange);
                        }

                        isbackwards = true;
                    }
                    else
                    {
                        if (input.y > 0)
                        {
                            isbackwards = false;
                        }

                        rb.AddForce(CalculateMovement(sprinting ? sprintSpeed : walkSpeed), ForceMode.VelocityChange);
                    }
                }
            }
            else
            {
                Weapon.isWalking = false;

                handAnimation.clip = handIdleAnimation;
                handAnimation.Play();

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

        //grounded = false;
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

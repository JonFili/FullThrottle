using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jet : MonoBehaviour
{
    public Animator animator;

    public Canvas canvas;
    public Text thrustText;

    [Header("WheelFriction")]
    public WheelCollider Front;
    public WheelCollider BackLeft;
    public WheelCollider BackRight;

    float thrustForce = 0;

    public Rigidbody rb;
    bool isWheelOpen = true;

    public float drive;
    public float reverse;
    public float turn;

    public float thrustControl;

    public float pitchAmount;
    public float rollAmount;

    float pitch = 0; 
    float roll = 0;

    public float lift;
    public float speedToLift;

    public float driveDrag;
    public float flyDrag;

    public float stabilizerForce;
    Vector3 stabilizer = Vector3.zero;

    public void OpenCloseWheel()
    {
        if (isWheelOpen)
        {
            animator.SetTrigger("Close");

            Front.enabled = false;
            BackRight.enabled = false;
            BackLeft.enabled = false;

            isWheelOpen = false;
        }
        else if (!isWheelOpen)
        {
            animator.SetTrigger("Open");

            Front.enabled = true;
            BackRight.enabled = true;
            BackLeft.enabled = true;

            isWheelOpen = true;
        }
    }

    

    public void Accelerate(float torque)
    {
        BackRight.brakeTorque = 0;
        BackLeft.brakeTorque = 0;

        BackRight.motorTorque = torque;
        BackLeft.motorTorque = torque;
    }

    public void ThrustForce()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            thrustForce += 40 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            thrustForce -= 40 * Time.deltaTime;
        }
        if (thrustForce > 100)
        {
            thrustForce = 100;
        }
        if (thrustForce < 0)
        {
            thrustForce = 0;
        }

        thrustText.text = Mathf.RoundToInt(thrustForce).ToString();
    }

    public void ThrustMove()
    {
        rb.AddForce(transform.forward * thrustForce * thrustControl * Time.deltaTime, ForceMode.Force);
    }
    

    public void Turn(float turnAmount)
    {
        
        Front.steerAngle = turnAmount;
        
    }

    public void Drag()
    {
        if (isWheelOpen)
        {
            rb.drag = driveDrag;
        }
        else
        {
            rb.drag = flyDrag;
        }
    }

    public void Pitch()
    {
        pitch = Mathf.Clamp(rb.velocity.magnitude, 0, speedToLift) / speedToLift * pitchAmount;

        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.localRotation = Quaternion.Euler(pitch * Time.deltaTime + transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z); //find way to fix:  add relative rotation 
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.localRotation = Quaternion.Euler(-1 * pitch * Time.deltaTime + transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z); //here too
        }
    }

    public void Roll()
    {
        roll = Mathf.Clamp(rb.velocity.magnitude, 0, speedToLift) / speedToLift * rollAmount;

        if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeTorque(Vector3.forward * roll * Time.deltaTime * 10000, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddRelativeTorque(Vector3.forward * roll * Time.deltaTime * -10000, ForceMode.Force);
        }
    }

    public void Lift()
    {
        rb.AddForce(Mathf.Clamp(rb.velocity.magnitude, 0, speedToLift) / speedToLift * Vector3.up * 9.81f, ForceMode.Force);
    }

    public void TorqueStabilizer()
    {
        rb.angularVelocity = Vector3.zero;
    }

    public void MyInputs()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OpenCloseWheel();
        }
        if (isWheelOpen)
        {
            if(Input.GetKey(KeyCode.W))
            {
                Accelerate(drive);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                Accelerate(reverse * -1);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                BackLeft.brakeTorque = 1;
                BackRight.brakeTorque = 1;
            }
            else
            {
                Accelerate(0);
            }

            if (Input.GetKey(KeyCode.D))
            {
                Turn(turn);
            }
            else if (Input.GetKey(KeyCode.A)) 
            {
                Turn(turn * -1);
            }
            else
            {
                Turn(0);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        

        animator = animator.GetComponent<Animator>();

        rb = rb.GetComponent<Rigidbody>();

        canvas = canvas.GetComponent<Canvas>();

        thrustText = thrustText.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        MyInputs();

        Drag();

        ThrustForce();
        ThrustMove();

        Pitch();
        Roll();

        Lift();
        Debug.Log(rb.velocity.magnitude);
    }
}

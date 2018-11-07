using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class playerscript : MonoBehaviour
{
    //vars
    public float maxtorque = 10;
    public float maxTurningAngle = 10;
    public Vector3 currentpos;
    public Vector3 gravity;
    public static int score = 0;
    public static int highscore = 0;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelBL;
    public WheelCollider wheelBR;
    public Vector3 centerOfMassAdjustment = new Vector3(0f, -0.9f, 0f);
    private Rigidbody body;
    public GameObject backlights;
    public float spoilerRatio = 0.1f;
    public Transform wheelTransformFL;
    public Transform wheelTransformFR;
    public Transform wheelTransformBL;
    public Transform wheelTransformBR;
    public float decelerationTorque = 30;
    public float topSpeed = 150;
    private float currentSpeed;
    public float acceleration = 0;
    public float maxacceleration = 100;
    public float maxoffroadacceleration = 40;
    public bool isonroad = true;
    public bool ismanual = true;
    public bool isgrounded = true;
    // Use this for initialization
    void Start()
    {
        //lower center of mass for roll-over resistance
        body = GetComponent<Rigidbody>();
        body.centerOfMass += centerOfMassAdjustment;
        gravity = new Vector3(0, -20, 0);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
            currentpos = gameObject.transform.position;
        //steering
        wheelFL.steerAngle = Input.GetAxis("Horizontal") * maxTurningAngle;
        wheelFR.steerAngle = Input.GetAxis("Horizontal") * maxTurningAngle;
        //drive
        wheelBL.motorTorque = Input.GetAxis("Vertical") * maxtorque;
        wheelBR.motorTorque = Input.GetAxis("Vertical") * maxtorque;
       Vector3 localVelocity = transform.InverseTransformDirection(body.velocity);
       body.AddForce(-transform.up * (localVelocity.z * spoilerRatio), ForceMode.Impulse);
        //apply deceleration when pressing the breaks or lightly when not pressing the gas.
        if (Input.GetAxis("Vertical") <= -0.5f && localVelocity.z > 0)
        {
            wheelBL.brakeTorque = decelerationTorque + maxtorque;
            wheelBR.brakeTorque = decelerationTorque + maxtorque;
        }
        else if (Input.GetAxis("Vertical") == 0)
        {
            wheelBL.brakeTorque = decelerationTorque;
            wheelBR.brakeTorque = decelerationTorque;
        }
        else
        {
            wheelBL.brakeTorque = 0;
            wheelBR.brakeTorque = 0;
        }
        //calculate max speed in KM/H (condensed calculation)
        currentSpeed = wheelBL.radius * wheelBL.rpm * Mathf.PI * 0.12f;
        if (currentSpeed < topSpeed)
        {
            //rear wheel drive.
            wheelBL.motorTorque = Input.GetAxis("Vertical") * maxtorque;
            wheelBR.motorTorque = Input.GetAxis("Vertical") * maxtorque;
        }
        else
        {
            //can't go faster, already at top speed that engine produces.
            wheelBL.motorTorque = 0;
            wheelBR.motorTorque = 0;
        }

    }
    void Update()
    {
        //alternate control scheme
        if (ismanual) {
             body.velocity = transform.forward * acceleration;
            if (isgrounded)
          {
                body.velocity = new Vector3(transform.forward.x * acceleration, transform.forward.y * acceleration , transform.forward.z * acceleration);
            } else
           {
               body.velocity = new Vector3(transform.forward.x * acceleration, -98f, transform.forward.z * acceleration);
            }
            //controls
            if (Input.GetKey(KeyCode.W) )
        {
                if (isonroad == true && acceleration <= maxacceleration)
                {
                    acceleration++;
                }
                if (isonroad == false && acceleration <= maxoffroadacceleration)
                {
                    acceleration++;
                }
                backlights.SetActive(false);
            } else if (acceleration > 0)
        {
            acceleration--;
            backlights.SetActive(true);
            }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * maxTurningAngle, Space.World);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * maxTurningAngle, Space.World);
        }
        if (Input.GetKey(KeyCode.S))
        {
                if (isonroad == true && acceleration >= -maxacceleration)
                {
                    acceleration--;
                }
                if (isonroad == false && acceleration >= -maxoffroadacceleration)
                {
                    acceleration--;
                }
            }
            else if (acceleration < 0)
            {
                acceleration++;
            }
        }
        float rotationThisFrame = 360 * Time.deltaTime;
      wheelTransformFL.Rotate(0, -wheelFL.rpm / rotationThisFrame, 0);
      wheelTransformFR.Rotate(0, -wheelFR.rpm / rotationThisFrame, 0);
      wheelTransformBL.Rotate(0, -wheelBL.rpm / rotationThisFrame, 0);
      wheelTransformBR.Rotate(0, -wheelBR.rpm / rotationThisFrame, 0);
        //Adjust the wheels heights based on the suspension.
        UpdateWheelPositions();
    }
    //move wheels based on their suspension.
    void UpdateWheelPositions()
    {
        WheelHit contact = new WheelHit();

        if (wheelFL.GetGroundHit(out contact))
        {
            Vector3 temp = wheelFL.transform.position;
            temp.y = (contact.point + (wheelFL.transform.up * wheelFL.radius)).y;
            wheelTransformFL.position = temp;
        }
        if (wheelFR.GetGroundHit(out contact))
        {
            Vector3 temp = wheelFR.transform.position;
            temp.y = (contact.point + (wheelFR.transform.up * wheelFR.radius)).y;
            wheelTransformFR.position = temp;
        }
        if (wheelBL.GetGroundHit(out contact))
        {
            Vector3 temp = wheelBL.transform.position;
            temp.y = (contact.point + (wheelBL.transform.up * wheelBL.radius)).y;
            wheelTransformBL.position = temp;
        }
        if (wheelBR.GetGroundHit(out contact))
        {
            Vector3 temp = wheelBR.transform.position;
            temp.y = (contact.point + (wheelBR.transform.up * wheelBR.radius)).y;
            wheelTransformBR.position = temp;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //check if vehicle is on the track
         if (other.gameObject.tag == "Road")
        {
            Debug.Log("hit");
            isonroad = true;
        }
        if (other.gameObject.tag == "Terrain")
        {
            isgrounded = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Road")
        {
            Debug.Log("miss");
            isonroad = false;
            if (acceleration > maxoffroadacceleration)
            {
                 acceleration = maxoffroadacceleration;
            }
        }
        if (other.gameObject.tag == "Terrain")
        {
            isgrounded = false;
        }
    }
}
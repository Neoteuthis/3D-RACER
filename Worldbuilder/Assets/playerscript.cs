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
    public static int score = 0;
    public static int highscore = 0;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelBL;
    public WheelCollider wheelBR;
    public Vector3 centerOfMassAdjustment = new Vector3(0f, -0.9f, 0f);
    private Rigidbody body;
    public float spoilerRatio = 0.1f;
    public Transform wheelTransformFL;
    public Transform wheelTransformFR;
    public Transform wheelTransformBL;
    public Transform wheelTransformBR;
    public float decelerationTorque = 30;
    public float topSpeed = 150;
    private float currentSpeed;



    // Use this for initialization
    void Start()
    {
        //lower center of mass for roll-over resistance
        body = GetComponent<Rigidbody>();
        body.centerOfMass += centerOfMassAdjustment;

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
            //Note: trans.up works for my model, you might need trans.right if you rotated a cylinder!
            temp.y = (contact.point + (wheelFL.transform.up * wheelFL.radius)*20).y;
            wheelTransformFL.position = temp;
        }
        if (wheelFR.GetGroundHit(out contact))
        {
            Vector3 temp = wheelFR.transform.position;
            temp.y = (contact.point + (wheelFR.transform.up * wheelFR.radius) * 20).y;
            wheelTransformFR.position = temp;
        }
        if (wheelBL.GetGroundHit(out contact))
        {
            Vector3 temp = wheelBL.transform.position;
            temp.y = (contact.point + (wheelBL.transform.up * wheelBL.radius) * 20).y;
            wheelTransformBL.position = temp;
        }
        if (wheelBR.GetGroundHit(out contact))
        {
            Vector3 temp = wheelBR.transform.position;
            temp.y = (contact.point + (wheelBR.transform.up * wheelBR.radius) * 20).y;
            wheelTransformBR.position = temp;
        }
    }
}
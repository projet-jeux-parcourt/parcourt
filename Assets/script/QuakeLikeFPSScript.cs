using System;
using Unity.VisualScripting;
using UnityEngine;

public class QuakeLikeFPSScript : MonoBehaviour
{
    public Transform bodyTransform;
    public Transform headTransform;
    public Rigidbody playerRigidBody;
    public Variables GeneralVar; // 0 = mid-air, 1 = walk, 2 = wall-running
    public float speed;
    public float yawRotationSpeed;
    public float pitchRotationSpeed;

    private Vector3 directionIntent;
    private bool wantToJump;
    private int state = 0;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private int GetState()
    {
        return state;
    }
    // Update is called once per frame
    private void Update()
    {

        if (Input.GetKey(KeyCode.W))
        {
            directionIntent += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            directionIntent += Vector3.back;
        }

        if (Input.GetKey(KeyCode.A))
        {
            directionIntent += Vector3.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            directionIntent += Vector3.right;
        }

        var mouseXDelta = Input.GetAxis("Mouse X");

        bodyTransform.Rotate(Vector3.up, Time.deltaTime * yawRotationSpeed * mouseXDelta);

        var mouseYDelta = Input.GetAxis("Mouse Y");

        var rotation = headTransform.localRotation;

        var rotationX = rotation.eulerAngles.x;

        rotationX += -Time.deltaTime * pitchRotationSpeed * mouseYDelta;
        

        var unClampedRotationX = rotationX;

        if (unClampedRotationX >= 180)
        {
            unClampedRotationX -= 360;
        }

        var clampedRotationX = Mathf.Clamp(unClampedRotationX, -80, 80);

        headTransform.localRotation =
            Quaternion.Euler(new Vector3(
                clampedRotationX,
                rotation.eulerAngles.y,
                rotation.eulerAngles.z
            ));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            wantToJump = true;
        }
    }

    private void FixedUpdate()
    {
        var n = Physics.SphereCast(bodyTransform.position + Vector3.up * (0.1f + 0.45f), 0.45f, Vector3.down,
            out var hitInfo,
            0.11f);
        state = ((n) ? 1 : 0);
        GeneralVar.declarations.Set("PlayerState", state);
        if (n)
        {
            var normalizedDirection = directionIntent.normalized;
            if (playerRigidBody.velocity.magnitude < speed)
            {
                playerRigidBody.velocity = 0.9f * playerRigidBody.velocity ;
                playerRigidBody.AddForce(bodyTransform.rotation * normalizedDirection * (speed*7), ForceMode.Acceleration);
            }
            directionIntent = Vector3.zero;

            if (wantToJump)
            {
                playerRigidBody.AddForce(
                    Vector3.up * 8f, ForceMode.VelocityChange
                );
            }
        }
        wantToJump = false;
    }
}
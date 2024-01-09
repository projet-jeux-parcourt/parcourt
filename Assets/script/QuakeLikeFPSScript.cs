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
    public float wallRunForce;

    private Vector3 directionIntent;
    private bool wantToJump;
    private int state = 0;
    private Vector2 rotationWanted = Vector2.zero;
    private bool isWallRunning;
    private float wallRunStartHeight;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private int GetState()
    {
        return state;
    }

    private void Update()
    {
        directionIntent = Vector3.zero;

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
        rotationWanted.x += mouseXDelta;

        var mouseYDelta = Input.GetAxis("Mouse Y");
        rotationWanted.y += mouseYDelta;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            wantToJump = true;
        }

        CheckForWallRun();
    }

    private void CheckForWallRun()
    {
        RaycastHit hit;
        bool isWallRight = Physics.Raycast(transform.position, transform.right, out hit, 1f);
        bool isWallLeft = Physics.Raycast(transform.position, -transform.right, out hit, 1f);

        // Modification pour vérifier si le mur touché est nommé "cube"
        if ((isWallRight || isWallLeft) && hit.collider.gameObject.name == "cube")
        {
            if (Input.GetKey(KeyCode.W) && state == 0) // Vérifie si le joueur est en l'air
            {
                StartWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        wallRunStartHeight = playerRigidBody.position.y;
        state = 2; // État de wall-running
    }

    private void FixedUpdate()
    {
        bodyTransform.Rotate(Vector3.up, Time.deltaTime * yawRotationSpeed * rotationWanted.x);
        var rotation = headTransform.localRotation;

        var rotationX = rotation.eulerAngles.x - Time.deltaTime * pitchRotationSpeed * rotationWanted.y;

        var unClampedRotationX = rotationX >= 180 ? rotationX - 360 : rotationX;
        var clampedRotationX = Mathf.Clamp(unClampedRotationX, -80, 80);

        headTransform.localRotation = Quaternion.Euler(new Vector3(clampedRotationX, rotation.eulerAngles.y, rotation.eulerAngles.z));

        rotationWanted = Vector2.zero;

        var isGrounded = Physics.SphereCast(bodyTransform.position + Vector3.up * (0.1f + 0.45f), 0.45f, Vector3.down, out var hitInfo, 0.11f);
        state = isGrounded ? 1 : 0;
        GeneralVar.declarations.Set("PlayerState", state);

        if (isGrounded || isWallRunning)
        {
            var normalizedDirection = directionIntent.normalized;
            if (playerRigidBody.velocity.magnitude < speed)
            {
                playerRigidBody.velocity = 0.9f * playerRigidBody.velocity;
                playerRigidBody.AddForce(bodyTransform.rotation * normalizedDirection * (speed * 7), ForceMode.Acceleration);
            }

            if (wantToJump)
            {
                playerRigidBody.AddForce(Vector3.up * 8f, ForceMode.VelocityChange);
                isWallRunning = false; // Arrête le wall-run si le joueur saute
            }
        }

        if (isWallRunning)
        {
            WallRunning();
        }

        directionIntent = Vector3.zero;
        wantToJump = false;
    }

    private void WallRunning()
    {
        // Maintenez la hauteur Y constante pendant le wall-run
        playerRigidBody.position = new Vector3(playerRigidBody.position.x, wallRunStartHeight, playerRigidBody.position.z);

        // Appliquez une force horizontale pour maintenir le joueur contre le mur
        Vector3 wallRunDirection = Vector3.ProjectOnPlane(bodyTransform.forward, Vector3.up).normalized;
        playerRigidBody.AddForce(wallRunDirection * wallRunForce, ForceMode.Acceleration);

        // Pour sortir du wall-run (par exemple, en sautant)
        if (wantToJump || !IsWallRunningConditionMet())
        {
            isWallRunning = false;
        }
    }

    private bool IsWallRunningConditionMet()
    {
        // Implémentez ici la logique pour déterminer si les conditions pour continuer le wall-run sont toujours remplies
        // Par exemple, vérifier si le joueur est toujours à côté d'un mur "wallToRun"
        return Physics.Raycast(transform.position, transform.right, 1f) || Physics.Raycast(transform.position, -transform.right, 1f);
    }

    private bool IsGrounded()
    {
        return Physics.SphereCast(bodyTransform.position + Vector3.up * 0.5f, 0.45f, Vector3.down, out var hitInfo, 1f);
    }
}

using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class QuakeLikeFPSScript : MonoBehaviour
{
    public Transform headTransform;
    public Transform playerTransform;
    public Rigidbody playerRigidBody;
    public LayerMask wallRunnable;
    public Variables GeneralVar; // 0 = mid-air, 1 = walk, 2 = wall-running / playerFlying:bool
    public float speed;
    public float yawRotationSpeed;
    public float pitchRotationSpeed;
    public float wallRunForce;

    private Vector3 directionIntent;
    private bool wantToJump, wantToFly;
    private int state = 0;
    private Vector2 rotationWanted = Vector2.zero;
    private bool isWallRunning;
    private float wallRunStartHeight;
    private Transform wallWhereYouRun;

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

        if (Input.GetKey(KeyCode.Space))
        {
            wantToFly = true;
        }
    }

    private void CheckForWallRun()
    {
        if (state == 0)
        {
            var hitInfo = Physics.OverlapSphere(playerTransform.position + Vector3.up * (0.1f + 0.8f), 0.8f, wallRunnable);
            
            if (hitInfo.Length>0)
            {
                StartWallRun(hitInfo[0].transform);
            }
        }
    }

    private void StartWallRun(Transform wallWhereYouRunArg)
    {
        isWallRunning = true;
        wallRunStartHeight = playerRigidBody.position.y;
        wallWhereYouRun = wallWhereYouRunArg;
        state = 2; // �tat de wall-running
    }

    private void FixedUpdate()
    {
        CheckForWallRun();
        playerTransform.Rotate(Vector3.up, Time.deltaTime * yawRotationSpeed * rotationWanted.x);
        var rotation = headTransform.localRotation;

        var rotationX = rotation.eulerAngles.x - Time.deltaTime * pitchRotationSpeed * rotationWanted.y;

        var unClampedRotationX = rotationX >= 180 ? rotationX - 360 : rotationX;
        var clampedRotationX = Mathf.Clamp(unClampedRotationX, -80, 80);

        headTransform.localRotation = Quaternion.Euler(new Vector3(clampedRotationX, rotation.eulerAngles.y, rotation.eulerAngles.z));

        rotationWanted = Vector2.zero;

        var isGrounded = Physics.SphereCast(playerTransform.position + Vector3.up * (0.1f + 0.45f), 0.45f, Vector3.down, out var hitInfo, 0.11f);
        if ((state == 2)  &  IsWallRunningConditionMet())
        {
            state = 2;
        }
        else
        {
            state = isGrounded ? 1 : 0;
            isWallRunning = false;

        }
        var genVar_declaration = GeneralVar.declarations;
        genVar_declaration.Set("PlayerState", state);

        if (isGrounded)
        {
            playerRigidBody.velocity = 0.9f * playerRigidBody.velocity;
            if (playerRigidBody.velocity.magnitude < speed)
            {
                var normalizedDirection = directionIntent.normalized;
                playerRigidBody.AddForce(playerTransform.rotation * normalizedDirection * (speed * 7), ForceMode.Acceleration);
            }

            if (wantToJump)
            {
                playerRigidBody.AddForce(Vector3.up * 6f, ForceMode.VelocityChange);
            }
        } 
        else if (isWallRunning)
        {
            WallRunning();
        }
        if (state==0)
        {
            AirControl(genVar_declaration);
        }

        directionIntent = Vector3.zero;
        wantToJump = false;
        wantToFly = false;
        genVar_declaration.Set("playerFlying", false);
    }

    private void AirControl(VariableDeclarations genVarDeclaration)
    {
        playerRigidBody.velocity = 0.995f * playerRigidBody.velocity;
        if (directionIntent.magnitude != 0f){
            var normalizedDirection = directionIntent.normalized;
            var reflux = -Vector3.Dot(normalizedDirection, playerRigidBody.velocity);
            reflux = (reflux < 1f) ? reflux : 1f;
            playerRigidBody.AddForce(playerTransform.rotation * (normalizedDirection * speed*1.5f) + (playerRigidBody.velocity.normalized * reflux), ForceMode.Acceleration);
        }
        if (wantToFly || genVarDeclaration.Get("playerFlying").Equals(true))
        {
            playerRigidBody.AddForce(Vector3.up * 5f, ForceMode.Acceleration);
        }
    }

    private bool isforward(float a, float b)
    {
        var diff = norme(a-b)%360;
        return !(90<diff & diff<270);
    }

    private float norme(float value)
    {
        return value < 0 ? -value : value;
    }

    private bool isOnRightSide()
    {
        return Vector3.Dot(playerRigidBody.position-wallWhereYouRun.position, wallWhereYouRun.rotation * Vector3.right) > 0;
    }

    private bool hasOnRightSide()
    {
        return Vector3.Dot(playerRigidBody.position-wallWhereYouRun.position, playerTransform.rotation * Vector3.right) > 0;
    }

    private void WallRunning()
    {
        // Maintenez la hauteur Y constante pendant le wall-run
        if (wallRunStartHeight > playerRigidBody.position.y)
        {
            playerRigidBody.position = new Vector3(playerRigidBody.position.x, wallRunStartHeight, playerRigidBody.position.z);
        }
        else
        {
            wallRunStartHeight = playerRigidBody.position.y;
        }

        // Appliquez une force horizontale pour maintenir le joueur contre le mur
        var isOnRight = isOnRightSide();
        var isForward = isforward(playerTransform.rotation.eulerAngles.y, wallWhereYouRun.rotation.eulerAngles.y);
        var wallDir = wallWhereYouRun.rotation;
        if (isOnRight)
        {
            playerRigidBody.AddForce(wallDir * Vector3.left * (wallRunForce), 
                ForceMode.Acceleration
                );
        }
        else
        {
            playerRigidBody.AddForce(wallWhereYouRun.rotation * Vector3.right * (wallRunForce), 
                ForceMode.Acceleration
                );
        }

        //playerTransform.rotation = Quaternion.Euler(0, playerTransform.eulerAngles.y, 0);
        //playerTransform.Rotate(wallWhereYouRun.rotation  * Vector3.forward,(isForward!=isOnRight)?20:-20);
        // deplacements
        playerRigidBody.velocity = 0.95f * playerRigidBody.velocity;
        if (playerRigidBody.velocity.magnitude <(speed*1.5))
        {
            int n = isforward(playerTransform.rotation.eulerAngles.y, wallWhereYouRun.rotation.eulerAngles.y)?1:-1;
            var directionInt = directionIntent.normalized.z * n;
            playerRigidBody.AddForce(wallWhereYouRun.rotation * Vector3.forward * (directionInt * speed * 7), ForceMode.Acceleration);
        }
        
        // Pour sortir du wall-run (par exemple, en sautant)
        if (wantToJump)
        {
            var direction = wallWhereYouRun.rotation * (isOnRight ? Vector3.right : Vector3.left);
            playerRigidBody.AddForce((Vector3.up * 6f) + (direction *5f), ForceMode.VelocityChange);
            isWallRunning = false;
        }
    }

    private bool IsWallRunningConditionMet()
    {
        // Impl�mentez ici la logique pour d�terminer si les conditions pour continuer le wall-run sont toujours remplies
        // Par exemple, v�rifier si le joueur est toujours � c�t� d'un mur "wallToRun"
        var contact = Physics.OverlapSphere(playerTransform.position + Vector3.up * (0.1f + 0.8f), 0.8f, wallRunnable);
        var met = false;
        for (var i = 0; i < contact.Length; ++i)
        {
            if (contact[i].transform == wallWhereYouRun)
            {
                met = true;
            }
        }
        if (!met)
        {
            playerTransform.rotation = Quaternion.Euler(0, playerTransform.eulerAngles.y, 0);
        }
        return met;
    }
}

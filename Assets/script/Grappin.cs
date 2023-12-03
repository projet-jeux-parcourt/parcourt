using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Grappin : MonoBehaviour
{
    public MeshRenderer Grappin_s_Visual;
    public Transform Grappin_s;
    public Transform MainCamera;
    public Transform playerTransform;
    public Rigidbody PlayerRigidbody;
    public Transform playerNeck;
    public float grappin_sStreng;
    public float grappin_sBake;
        
    private Vector3 catchPoint;
    private bool actif = false;
    private bool first = true;

    // Start is called before the first frame update
    void Start()
    {
        Grappin_s_Visual.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (first)
            {
                first = false;
            }
            else
            {
                if (actif)
                {
                    Grappin_s_Visual.enabled = false;
                    actif = false;
                }
                else
                {
                    var direction = MainCamera.rotation * Vector3.forward;
                    if (Physics.Raycast(MainCamera.position, direction,
                            out var hitInfo, 30f))
                    {
                        actif = true;
                        catchPoint = hitInfo.point;
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (actif)
        {
            if (!Grappin_s_Visual.enabled)
            {
                Grappin_s_Visual.enabled = true;
            }

            var pointA = playerTransform.position + (Vector3.up * 0.8f);
            var direction_vetor = catchPoint - pointA;
            var dist = direction_vetor.magnitude;
            var direction_normalized = direction_vetor.normalized;
            pointA = pointA + (direction_normalized * 0.5f);
            Grappin_s.position = pointA;
            float dir_X = Vector3.Angle(direction_normalized, Vector3.up);
            Vector2 n = Vector2.zero;
            n.x = direction_normalized.x;
            n.y = direction_normalized.z;
            float dir_Y = Vector2.SignedAngle(n, Vector2.up);
            Grappin_s.rotation.Set(90, 45, 0,0);
        }
    }
}

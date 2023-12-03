using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
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

            var pointA = playerTransform.position;
            var direction_vetor = pointA - catchPoint;
            pointA = pointA + direction_vetor.normalized;
            Grappin_s.position = pointA;
            //Grappin_s.rotation = direction_vetor.; WIP
        }
    }
}

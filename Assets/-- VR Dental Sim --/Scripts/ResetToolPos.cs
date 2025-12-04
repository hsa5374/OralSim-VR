using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
public class ResetToolPos : MonoBehaviour
{

    private Vector3 startPosition;
    private Quaternion startRotation;
    bool resetPos;
    public float delay;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    private void Update()
    {
        if (!GetComponent<Grabbable>().isGrabbed)
        {
            if (delay > 0)
            {
                delay -= Time.deltaTime;
                if (delay <= 0)
                {
                    if (!resetPos)
                    {
                        resetPos = true;
                        transform.position = startPosition;
                        transform.rotation = startRotation;
                       // GetComponent<Rigidbody>().useGravity = true;
                        GetComponent<Rigidbody>().isKinematic=true;

                    }
                    
                }
            }
        }
        else
        {
            delay = 1f;
            resetPos = false;
        }
    }

    //public void ResetToStart()
    //{
    //    if (!GetComponent<Grabbable>().isGrabbed)
    //    {
    //        if (delay <= 0)
    //        {
    //            transform.position = startPosition;
    //            transform.rotation = startRotation;
    //        }
    //    }
    //}
}

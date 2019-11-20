using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraFollow : MonoBehaviour
{

    private Func<Vector3> getCameraFollowPositionFunc;
    
    public void Setup(Func<Vector3> getCameraFollowPositionFunc)
    {
        this.getCameraFollowPositionFunc = getCameraFollowPositionFunc;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 cameraFollowPosition = getCameraFollowPositionFunc();
        cameraFollowPosition.z = transform.position.z;
        Vector3 cameraMoveDir = (cameraFollowPosition - transform.position).normalized;
        float distance = Vector3.Distance(cameraFollowPosition, transform.position);
        //change this to modify how quickly the camera follows the player.
        float cameraMoveSpeed = 5f;   
        transform.position = transform.position+ cameraMoveDir*distance*cameraMoveSpeed*Time.deltaTime;
    }
}

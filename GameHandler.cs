using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public CameraFollow cameraFollow;

    public Transform playerTransform;
    // Start is called before the first frame update
    
    //attatch this to the games main camera.
    void Start()
    {
        cameraFollow.Setup(()=>playerTransform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

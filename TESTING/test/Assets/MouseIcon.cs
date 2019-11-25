using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MouseIcon : MonoBehaviour
{
    public Camera MainCamera;
    // Start is called before the first frame update
    void Start()
    {
        MainCamera = FindObjectOfType<Camera>();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;
    }
}
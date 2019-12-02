using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShell : MonoBehaviour
{
    public Transform Direction;
    public Transform OriginPlayer;
    public GameObject ShellPrefab;    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShellEject()
    {
        GameObject Shell = Instantiate(ShellPrefab, OriginPlayer.transform.position, Quaternion.identity);
        Shell.GetComponent<Rigidbody2D>().AddForce(Direction.transform.position-OriginPlayer.transform.position, ForceMode2D.Force);
        
    }
}

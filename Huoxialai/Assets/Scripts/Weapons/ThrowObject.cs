using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{

    public GameObject projectile;
    private float throwSpeed=2000f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            GameObject throwThis = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
            throwThis.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, throwSpeed));
        }
        
    }
}

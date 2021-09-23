using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{

    private GameObject role;
    private GameObject roleShield;// Weapon that the player's holding
    private GameObject roleHead;
    private Animator roleAnimator;

    private bool isHold = false;
    private GameObject owner = null;
    private GameObject currentWeapon = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHold) return;
        //float rx = role.transform.rotation.y;

        //float ry = 0;
        //float rz = -86.69801f;
        //float rw = 0;// transform.rotation.w;
        //transform.rotation = new Quaternion(rx, ry, rz, rw);

        //if (owner == null) Destroy(transform.parent);// If the owner has been killed
        //transform.parent.position = owner.transform.position;

        //float px = transform.parent.position.x;// + 0.00176f;// roleSpine.transform.position.x;// -0.00094f;//-0.0006f;
        //float py = transform.parent.position.y;// roleSpine.transform.position.y;//0.00049f;//-0.0065736f;
        //float pz = transform.parent.position.z - 0.3f;// + 0.00273f;// roleSpine.transform.position.z;//0.00043f;
        //transform.position = new Vector3(px, py, pz);

        //float px = role.transform.position.x;// + 0.00176f;// roleSpine.transform.position.x;// -0.00094f;//-0.0006f;
        //float py = role.transform.position.y;// roleSpine.transform.position.y;//0.00049f;//-0.0065736f;
        //float pz = role.transform.position.z;// - 0.3f;// roleSpine.transform.position.z;//0.00043f;
        //transform.position = new Vector3(px, py, pz);

        //transform.parent.rotation = new Quaternion(90f, 0, 0, 0);

        //float rx = role.transform.rotation.x;// roleSpine.transform.rotation.x+ 4.327f;// + 84.596f;
        //float ry = role.transform.rotation.y;// -89.21101f;
        //float rz = role.transform.rotation.z;// + 86.69801f;// roleSpine.transform.rotation.z+ 90.316f;// + -1.86f;
        //float rw = role.transform.rotation.w;// roleSpine.transform.rotation.w;

        transform.parent.Rotate(0, 0, 03f);
        //transform.parent.rotation = new Quaternion(rx+90f, ry, rz, rw);
    }


    private void OnCollisionEnter(Collision other)
    {
        if (!other.transform.tag.Equals("Player")) return;


        if (!isHold)// Is on the ground
        {
            this.GetComponent<Rigidbody>().useGravity = false;
            Debug.Log(other.transform.name + " picked up a " + transform.name);

            role = other.gameObject;
            role.SendMessage("increaseWeaponNum");
            roleHead = other.transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head").gameObject;
            //roleShield = roleHead.transform.Find("Shield").gameObject;
            roleAnimator = other.collider.GetComponent<Animator>();

            //transform.rotation = roleSpine.transform.rotation;

            transform.name = "Shield";


            ////float px = role.transform.position.x;// + 0.00176f;// roleSpine.transform.position.x;// -0.00094f;//-0.0006f;
            ////float py = role.transform.position.y;// roleSpine.transform.position.y;//0.00049f;//-0.0065736f;
            ////float pz = role.transform.position.z - 0.3f;// + 0.00273f;// roleSpine.transform.position.z;//0.00043f;
            ////transform.position = new Vector3(px, py, pz);

            //transform.parent.position = other.transform.position;

            //transform.parent.rotation = other.transform.rotation;

            //float px = transform.parent.position.x;// + 0.00176f;// roleSpine.transform.position.x;// -0.00094f;//-0.0006f;
            //float py = transform.parent.position.y+1f;// roleSpine.transform.position.y;//0.00049f;//-0.0065736f;
            //float pz = transform.parent.position.z - 0.3f;// + 0.00273f;// roleSpine.transform.position.z;//0.00043f;
            //transform.position = new Vector3(px, py, pz);

            ////float rx = 90f;// roleSpine.transform.rotation.x+ 4.327f;// + 84.596f;
            ////float ry = role.transform.rotation.y;// -89.21101f;
            ////float rz = role.transform.rotation.z;// + 86.69801f;// roleSpine.transform.rotation.z+ 90.316f;// + -1.86f;
            ////float rw = role.transform.rotation.w;// roleSpine.transform.rotation.w;
            //////////transform.rotation = roleShield.transform.rotation;
            ////transform.parent.rotation = new Quaternion(rx, ry, rz, rw);
            ////transform.parent.rotation=new Quaternion(90f, 0, 0,0);
            //transform.parent.Rotate(90f, 0, 0);

            //transform.localScale = new Vector3(1f, 1f, 1f);


            //transform.parent = roleHead.transform;

            // Destory the weapon's rigidbody property
            Destroy(transform.GetComponent<Rigidbody>());

            isHold = true;
            owner = other.gameObject;


            if (owner.name.Contains("NPC"))
            {
                Destroy(owner.GetComponent<NPCMovement2>().currentShield);
                owner.GetComponent<NPCMovement2>().currentShield = transform.parent.gameObject;
            }
            if (owner.name.Equals("Player"))
            {
                Destroy(owner.GetComponent<PlayerMovement>().currentShield);
                owner.GetComponent<PlayerMovement>().currentShield = transform.parent.gameObject;
            }
            //Destroy(transform.GetComponent<Halo>());

        }
        if(isHold) // if is hold
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), owner.GetComponent<Collider>());// let the shield not collide with owner itself

            
            if (owner.name.Contains("NPC")) currentWeapon = owner.GetComponent<NPCMovement2>().currentWeapon;
            if (owner.name.Equals("Player")) currentWeapon = owner.GetComponent<PlayerMovement>().currentWeapon;
            Physics.IgnoreCollision(GetComponent<Collider>(), currentWeapon.GetComponent<Collider>());// let the shield not collide with owner's weapon
        }
    }
}

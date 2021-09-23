using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    public float X_bias;
    public float Y_bias;

    private GameObject role;
    private GameObject roleWeapon;// Weapon that the player's holding
    private GameObject roleRightHand;
    private Animator roleAnimator;

    private bool isHold = false;
    private bool attacking = false;
    private bool idling = false;
    private string owner;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {

        if (!role) return;
        if (role.name.Contains("Player"))
        {

            if (Input.GetKeyDown(KeyCode.Return) == true)
            {

                StartCoroutine("PlayAnimation");
            }
        }
    }

 


    private void OnTriggerEnter(Collider other)
    {
        if (isHold)
        {
            Debug.Log(owner + " attack " + other.transform.name);
            if (owner.Contains("Player") && other.transform.name.Contains("NPC")) Debug.Log("yyyyyyyyy");
            //if (other.gameObject.name.Contains("NPC") && roleAnimator.GetBool("wrench"))
            if (other.gameObject.name.Contains("NPC"))
            {
                Debug.Log(owner + "killed!");
                Destroy(other.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {

        //if (other.transform.name != "Player" || !other.transform.name.Contains("NPC"))
        //    return;
        if (!other.transform.tag.Equals("Player")) return;


        this.GetComponent<Rigidbody>().useGravity = false;



        if (isHold == false)// Is on the ground
        {
            Debug.Log(other.transform.name + " picked up a " + transform.name);

            role = other.gameObject;

            role.SendMessage("increaseWeaponNum");
            
            roleRightHand = other.transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand").gameObject; //roleRightHand.transform.GetChild(5).gameObject;
            roleWeapon = roleRightHand.transform.Find("Weapon").gameObject;
            roleAnimator = other.collider.GetComponent<Animator>();

            // Put the weapon on the player's hand
            //transform.localScale /= 5.0f;
            transform.rotation = roleRightHand.transform.rotation;
            transform.parent = roleRightHand.transform;
            transform.name = "Weapon";


            float x = roleRightHand.transform.position.x + -0.00094f;//-0.0006f;
            float y = roleRightHand.transform.position.y + 0.00049f;//-0.0065736f;
            float z = roleRightHand.transform.position.z;//0.00043f;

            transform.position = new Vector3(x, y, z);

            // Destroy the weapon highlight on the ground
            //Destroy(transform.GetChild(0).gameObject);

            // Destroy the original weapon in hand
            Destroy(roleWeapon);

            // Destory the weapon's rigidbody property
            Destroy(transform.GetComponent<Rigidbody>());

            isHold = true;

            owner = other.transform.name;


        }
        else // Is holding by the role
        {
            Debug.Log(owner + " attack " + other.transform.name);
            if (owner.Contains("Player") && other.transform.name.Contains("NPC")) Debug.Log("yyyyyyyyy");

            if (other.gameObject.name.Contains("NPC"))
            {
                Debug.Log(owner + "killed!");
                Destroy(other.gameObject);
            }
        }
    }

    private IEnumerator PlayAnimation()
    {
        roleAnimator.SetBool("wrench", true);
        Debug.Log("attacking!");
        yield return new WaitForSeconds(Time.deltaTime * 10);
        roleAnimator.SetBool("wrench", false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrench : MonoBehaviour
{

    //throw star
    //-0.00077
    //0.00049
    //-0.00076
    //90 0 0
    //0.03*3

    public float X_rot;
    public float Y_rot;
    public float Z_rot;
    public GameObject rangeSpellPrefab;
    public GameObject selectUnit;
    public GameObject rangeAttackPrefab;

    public float X_bias;
    public float Y_bias;
    //public float rotate_bias;
    public float size = 1;
    public float ATK = 0.5f;
    public string weaponName;
    public string weaponAnimation;
    public string weaponSound;

    //public Transform rotat;

    private GameObject role;
    private GameObject roleWeapon;// Weapon that the player's holding
    private GameObject roleRightHand;
    private Animator roleAnimator;
    private GameObject rot;

    private bool isThrown;
    private bool isCastMagic;
    private Vector3 g = new Vector3(0, -5f,0);
    private Vector3 v;


    private bool isHold = false;
    private bool attacking = false;
    private GameObject owner = null;

    private GameObject player;
    private Vector3 playerPos;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.Find("Player");
        isThrown = false;
        ATK = 0.5f;
}

    public bool GetAttackMode()
    {
        return attacking;
    }



    // Update is called once per frame
    void Update()
    {
        if (isThrown)
        {

            transform.position += Time.fixedDeltaTime * v;
            v = v + Time.fixedDeltaTime * g;
            if (transform.position.x < 0.0f)
                Destroy(gameObject);
            transform.Rotate(new Vector3(0, 0, 30.0f));
            return;
        }
        playerPos = player.transform.position;
        //Debug.Log(transform.parent.name);
        if (!role) return;
        if (role.name.Contains("Player"))
        {
            if (Input.GetKeyDown(KeyCode.Return) == true)
            {
                StartCoroutine("PlayAnimation");
            }
        }

        if(attacking == false && owner != null && owner.transform.name.Contains("NPC")) {
            if((Vector3.Distance(transform.position, playerPos) <= 3&&weaponName.Equals("wrench"))||(Vector3.Distance(transform.position, playerPos)>3&&Vector3.Distance(transform.position, playerPos) <= 5 && weaponName.Equals("proj")) ||(Vector3.Distance(transform.position, playerPos)>5&&Vector3.Distance(transform.position, playerPos) <= 10 && weaponName.Equals("wand"))) {
                Debug.Log("NPC wanted to attack");
                StartCoroutine("PlayAnimation");
            }

        }
        
    }



    private void OnTriggerEnter(Collider other)
    {
        if (owner == null) return;

        if (owner.transform.name.Equals("Player") && attacking)
        {
            if (other.transform.name.Contains("NPC"))
            {
                //Debug.Log(owner.transform.name + " attack " + other.transform.name);
                SoundManager.instance.playSound("npcHurt");
                other.SendMessage("getHurt", ATK);
                GameObject.Find("GameManager").GetComponent<GameDisplay>().showPrompt("Got hurt!");
            }
        }

        if(owner.transform.name.Contains("NPC") && attacking) {
            if(other.transform.name.Equals("Player")) {
                Debug.Log(owner.transform.name + " attack " + other.transform.name);
                SoundManager.instance.playSound("playerHurt");
                other.SendMessage("getHurt", ATK);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //When throwing, the collision is detected here, don't know why, just a bug
        if (isThrown)
        {
            Debug.Log("撞到" + other.transform.name + "啦！");
            if (owner.transform.name.Equals("Player") && attacking)
            {
                if (other.transform.name.Contains("NPC"))
                {
                    Debug.Log(owner.transform.name + " attack " + other.transform.name);
                    other.transform.SendMessage("getHurt", ATK);
                    SoundManager.instance.playSound("npcHurt");
                    //other.transform.GetComponent<NPCMovement>().health -= ATK;
                }
            }

            if (owner.transform.name.Contains("NPC") && attacking)
            {
                if (other.transform.name.Equals("Player"))
                {
                    Debug.Log(owner.transform.name + " attack " + other.transform.name);
                    //other.transform.GetComponent<PlayerMovement>().health -= ATK;
                    SoundManager.instance.playSound("playerHurt");
                    other.transform.SendMessage("getHurt", ATK);
                }
            }
        }

        //if (other.transform.name != "Player" || !other.transform.name.Contains("NPC"))
        //    return;
        if (!other.transform.tag.Equals("Player")) return;
        if (isThrown) return;

        GetComponent<Rigidbody>().useGravity = false;

        

        if (isHold == false||other.gameObject.tag=="player")// Is on the ground
        {
            isHold = true;
            Debug.Log(other.transform.name+" picked up a "+transform.name);

            role = other.gameObject;
            
            
            role.SendMessage("increaseWeaponNum");

            roleRightHand = other.transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand").gameObject; //roleRightHand.transform.GetChild(5).gameObject;
            rot = other.transform.Find("Throwing hand").gameObject; //roleRightHand.transform.GetChild(5).gameObject;

            Transform tmp = roleRightHand.transform.Find("Weapon");
            

            roleAnimator = role.GetComponent<Animator>();
            if (roleAnimator == null)
            { Debug.Log("见鬼了:"+role.name); }

            // Put the weapon on the player's hand
            
            transform.rotation = roleRightHand.transform.rotation;
            transform.Rotate(new Vector3(X_rot, Y_rot, Z_rot));
            //transform.rotation += new Vector3(X_rot, Y_rot, Z_rot);
            transform.parent = roleRightHand.transform;
            

            //Vector3 scale = transform.localScale;

            //scale.Set(2f,2f,2f);//(scale*size).x, (scale * size).y, (scale * size).z

            transform.localScale *= size;

            float x = roleRightHand.transform.position.x + X_bias;
            float y = roleRightHand.transform.position.y + Y_bias;
            float z = roleRightHand.transform.position.z;
            //transform.rotate;
            transform.position = new Vector3(x, y, z);


            // Destroy the original weapon in hand
            if(tmp!=null)
            {
                roleWeapon = tmp.gameObject;
                Destroy(roleWeapon);
            }
            

            // Destory the weapon's rigidbody property
            Destroy(transform.GetComponent<Rigidbody>());

            // If the weapon is hold, then it starts to function
            

            owner = other.gameObject;

            if (owner.name.Contains("NPC")) owner.GetComponent<NPCMovement2>().currentWeapon = gameObject;
            if (owner.name.Equals("Player"))
            {
                owner.GetComponent<PlayerMovement>().currentWeapon = gameObject;
                GameObject.Find("GameManager").GetComponent<GameDisplay>().showPrompt("Pick up a " + transform.name.Substring(0, transform.name.Length -7));
            }
            transform.name = "Weapon";
            //GameObject range = Instantiate(rangeAttackPrefab, role.transform.position, Quaternion.identity);

        }
        else // Is holding by the role
        {
        }
    }

    public IEnumerator PlayAnimation()
    {
        
        roleAnimator.SetBool(weaponAnimation,true);
        yield return new WaitForSeconds(Time.deltaTime * 10);

        if (owner.transform.name.Equals("Player")) SoundManager.instance.playSound(weaponSound);// Play efx sound 
        attacking = true;
        roleAnimator.SetBool(weaponAnimation, false);
        float waitNextHit = 40f;
        if (weaponName.Equals("wand"))
            waitNextHit = 80f;
        yield return new WaitForSeconds(Time.deltaTime * waitNextHit);
        attacking = false;
        if (gameObject != null && weaponName == "proj")
        {
            role.SendMessage("decreaseWeaponNum");
            Destroy(gameObject);
        }
    }

    public void RangeAttack()
    {
        selectTarget();
        if (selectUnit != null)
        {
            transform.GetChild(0).GetComponent<BoxCollider>().isTrigger = true;
            RFX4_EffectEvent ev = transform.GetChild(0).GetComponent<RFX4_EffectEvent>();
            ev.OverrideAttachPointToTarget = selectUnit.transform.GetChild(1);
            ev.ActivateEffect();
            //Vector3 SpawnSpellLoc = transform.position;
            //GameObject clone = Instantiate(rangeSpellPrefab, SpawnSpellLoc, Quaternion.identity);
            //clone.transform.GetComponent<RangeSpell>().Target = selectUnit;
        }
    }

    public void selectTarget()
    {
        selectUnit = null;
        if (owner.name.Contains("NPC"))
        {
            selectUnit = player;
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(role.transform.position, 20f);

        //Collider nearestCollider = null;
        float minSqrDistance = Mathf.Infinity;
        Debug.Log("=====================================");
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.name != "NPC(Clone)") continue;
            Vector3 dir =  colliders[i].transform.position- role.transform.position;
            float degree = Mathf.Acos(Vector3.Dot(role.transform.forward.normalized, dir.normalized));
            
            if ( degree> 2*Mathf.PI/3) continue;
            Debug.Log("结果：" + degree);
            Debug.Log("现在碰到的是：" + colliders[i].gameObject.name );
            float sqrDistanceToCenter = dir.sqrMagnitude;

            if (sqrDistanceToCenter < minSqrDistance)
            {
                minSqrDistance = sqrDistanceToCenter;
                selectUnit = colliders[i].gameObject;
            }
        }
        

    }


    public void ThrowMe()
    {

        Debug.Log("Throwlalallala!");
        isThrown = true;
        gameObject.transform.SetParent(null);
        gameObject.transform.rotation = rot.transform.rotation;
        v = g+transform.forward*30.0f/1.0f;
    }
}

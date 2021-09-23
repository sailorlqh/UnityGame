using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NPCMovement2 : MonoBehaviour
{
    public float walkingSpeed = 0.1f;
    public GameObject currentWeapon = null;
    public GameObject currentShield = null;
    public GameObject healthBar;
    public float health = 1f;
    public float maxHealth = 1f;

    private GameObject player;
    private Vector3 playerPos;
    private Vector3 NPCPos;
    private bool playerIsNear;
    private bool attacking;
    private Animator m_animator;
    private Rigidbody m_rb;
    private float gravity = 9.8f;
    private int numWeaponNPCHave;
    private int maxWeaponNum;
    private int nearByWeaponIndex;
    private int countFrame;
    private int changeDirectionDurtion;
    private GameObject thisNPC;
    private float fixRotationY = 0;
    private Camera cam;
    private Vector3 constVec = new Vector3(-1, -1, -1);
    private List<Vector3> npcPosQueue;
    private List<float> playerPosXList;
    private List<float> playerPosZList;
    private int playerPosPredicted;
    private Vector3 predictedPlayerPos;

    public static class queueManageVaribeles
    {
        public const int X = 0;
        public const int Z = 1;
        public const int EMPTYQUEUE = 0;
        public const int APPENDQUEUE = 1;
    }

    //private int currentState = 1;


    // Start is called before the first frame update
    public void increaseWeaponNum()
    {
        if (numWeaponNPCHave < maxWeaponNum)
        {
            numWeaponNPCHave += 1;
        }
        // Debug.Log("Now this NPC has one more weapon");
    }

    public void decreaseWeaponNum()
    {
        if (numWeaponNPCHave >= 1)
        {
            numWeaponNPCHave -= 1;
        }
        Debug.Log("Now this NPC has used one weapon");
    }

    private int findNearByWeaponIndex()
    {
        int i = 0;
        int res = -1;
        for (i = 0; i < Environment.weaponPositionList.Count; i++)
        {
            if (Environment.weaponPositionList[i] != null && Vector3.Distance(NPCPos, Environment.weaponPositionList[i].transform.position) <= 15)
            {
                if (Environment.weaponPositionList[i].transform.position.y >= 1.5)
                    return i;
            }
        }
        return res;
    }

    private Vector3 getNearByWeaponByIndex(int index)
    {
        GameObject nearByWeapon = Environment.weaponPositionList[index];
        return nearByWeapon.transform.position;
    }


    void Start()
    {
        m_animator = gameObject.GetComponent<Animator>();
        m_animator.SetFloat("transit", 0.3f);

        StartCoroutine("RandomDirectionChanged");
        player = GameObject.Find("Player");
        numWeaponNPCHave = 0;
        maxWeaponNum = 1;
        nearByWeaponIndex = -1;
        countFrame = 0;
        changeDirectionDurtion = 60;
        health = 1f;
        thisNPC = gameObject;
        cam = GameObject.Find("Player/MainCamera").GetComponent<Camera>();
        healthBar = Instantiate(healthBar, GameObject.Find("Canvas").transform);
        npcPosQueue = new List<Vector3>();
        playerPosXList = new List<float>();
        playerPosZList = new List<float>();
        playerPosPredicted = 0;
        StartCoroutine("CheckInSafeZone");
        m_rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(Environment.weaponPositionList[0].printPos());
        countFrame += 1;
        transform.Translate(Vector3.forward * walkingSpeed);
        attacking = false;

        // Configurate the health bar
        if (Vector3.Distance(player.transform.position, transform.position) < 15)
        {
            healthBar.SetActive(true);
            updateHealthBar();
        }
        else { healthBar.SetActive(false); }

        // Check death
        if (health <= 0)
        {
            DeathEffect();
        }


        playerPos = player.transform.position;
        NPCPos = transform.position;
        float distanceToPlayer = Vector3.Distance(playerPos, NPCPos);
        Vector3 weaponPos;
        Transform t;
        t = transform;
        Vector3 safeZonePos = getSafeZoneCoord();
        if (!safeZonePos.Equals(constVec))
        {
            NPCMovementWhenOutsideOfSafeZone(safeZonePos);
        }
        else
        {
            if (numWeaponNPCHave == 0)
            {
                NPCMoveMentNoWeapen(distanceToPlayer);
            }
            else if(currentWeapon.GetComponent<Wrench>().GetAttackMode() == false)
            {
                NPCMoveMentWithWeapon(distanceToPlayer);
            }
        }
        NPCPos = transform.position;
        ManageNPCPosQueue(NPCPos);
        if (playerPosPredicted > 0) playerPosPredicted -= 1;
    }

    // private void NPCAttackPlayer() {
    //     if(thisNPC.GetComponent<Weapon>() == null) {
    //         Debug.Log("1");
    //     } else {
    //         Debug.Log("don't fuck 1");
    //     }
    //     // if(thisNPC.Find)
    //     // if(thisNPC.Find("Player/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Weapon") != null) {
    //     //     Debug.Log("hahahahah");
    //     // }
    //     if(thisNPC.GetComponent<Wrench>() == null) {
    //         Debug.Log("fuck 2");
    //     } else {
    //         Debug.Log("don't fuck 2");
    //     }
    //     // thisNPC.GetComponent<Wrench>().PlayAnimation();
    // }

    public void Throw()
    {
        if (currentWeapon != null)
            currentWeapon.SendMessage("ThrowMe");
    }

    public void Cast()
    {
        currentWeapon.SendMessage("RangeAttack");
    }

    public void getHurt(float ATK)
    {
        health -= ATK;
        StartCoroutine("hurt");
    }

    public IEnumerator hurt()
    {
        m_animator.SetBool("Hurt", true);
        yield return new WaitForSeconds(Time.deltaTime * 10);
        m_animator.SetBool("Hurt", false);

    }

    private void NPCMovementWhenOutsideOfSafeZone(Vector3 safeZonePos)
    {
        Vector3 dx = (safeZonePos - NPCPos) / Vector3.Distance(safeZonePos, NPCPos);
        dx.y = 0;
        Vector3 lookAtPos = safeZonePos;
        lookAtPos.y = NPCPos.y;
        transform.position += dx * 0.05f * Time.deltaTime;
        transform.LookAt(lookAtPos);
    }

    private void NPCMoveMentNoWeapen(float distanceToPlayer)
    {
        Vector3 lookAtPos;
        nearByWeaponIndex = findNearByWeaponIndex();
        // }
        Vector3 weaponPos;
        if (distanceToPlayer <= 15)
        {
            if (playerPosPredicted > 0)
            {
                ManagePlayerPosQueue(queueManageVaribeles.X, queueManageVaribeles.EMPTYQUEUE);
                ManagePlayerPosQueue(queueManageVaribeles.Z, queueManageVaribeles.EMPTYQUEUE);
                playerPosPredicted = 0;
            }
            if (countFrame % (changeDirectionDurtion / changeDirectionDurtion) == 0)
            {
                Debug.Log("Add Player Position to List");
                ManagePlayerPosQueue(queueManageVaribeles.X, queueManageVaribeles.APPENDQUEUE);
                ManagePlayerPosQueue(queueManageVaribeles.Z, queueManageVaribeles.APPENDQUEUE);
            }
        }
        if (nearByWeaponIndex != -1)
        {
            weaponPos = getNearByWeaponByIndex(nearByWeaponIndex);
            Vector3 dx = (weaponPos - NPCPos) / Vector3.Distance(weaponPos, NPCPos);
            dx.y = 0;
            transform.position += dx * 0.05f * Time.deltaTime;
            lookAtPos = weaponPos;
            lookAtPos.y = NPCPos.y;
            transform.LookAt(lookAtPos);
        }
        else
        {
            if (distanceToPlayer <= 15)
            {
                playerIsNear = true;
                float randomX = Random.Range(-1.0f, 1.0f);
                float randomZ = Random.Range(-1.0f, 1.0f);
                Vector3 dx = (playerPos - new Vector3(randomX, 0.0f, randomZ) - NPCPos) / Vector3.Distance(playerPos, NPCPos);
                dx.y = 0;
                transform.position += dx * 0.05f * Time.deltaTime;
                lookAtPos = player.transform.position;
                lookAtPos.y = NPCPos.y;
                transform.LookAt(lookAtPos);
            }
            else
            {
                //Debug.Log(playerPosXList.Count);
                if (playerPosPredicted > 0)
                {
                    playerPosPredicted -= 1;
                    // predictedPlayerPos.y = NPCPos.y;
                    predictedPlayerPos = new Vector3(predict(playerPosXList, 50 - playerPosPredicted), NPCPos.y, predict(playerPosZList, 50 - playerPosPredicted));
                    //Debug.Log("NPC Move to predicted position");
                    //Debug.Log(playerPosPredicted);
                    //Debug.Log(predictedPlayerPos);
                    Vector3 dx = (predictedPlayerPos - NPCPos) / Vector3.Distance(predictedPlayerPos, NPCPos);
                    dx.y = 0;
                    transform.position += dx * 0.05f * Time.deltaTime;
                    transform.LookAt(predictedPlayerPos);
                    if (playerPosPredicted == 0)
                    {
                        ManagePlayerPosQueue(queueManageVaribeles.X, queueManageVaribeles.APPENDQUEUE);
                        ManagePlayerPosQueue(queueManageVaribeles.Z, queueManageVaribeles.APPENDQUEUE);
                    }
                }
                else
                {

                    if (playerPosXList.Count >= 100)
                    {
                        predictedPlayerPos = new Vector3(predict(playerPosXList, 0), NPCPos.y, predict(playerPosZList, 0));
                        Debug.Log(predictedPlayerPos);
                        playerPosPredicted = 50;
                        Vector3 dx = (predictedPlayerPos - NPCPos) / Vector3.Distance(predictedPlayerPos, NPCPos);
                        dx.y = 0;
                        transform.position += dx * 0.05f * Time.deltaTime;
                        transform.LookAt(predictedPlayerPos);
                    }
                    else
                    {
                        if (countFrame % changeDirectionDurtion == 0)
                        {
                            float y = Random.Range(0, 360);
                            transform.Rotate(0, y, 0);
                        }
                        transform.position += Vector3.forward * Time.deltaTime;
                    }
                }
            }
        }
    }

    private void NPCMoveMentWithWeapon(float distanceToPlayer)
    {
        Vector3 lookAtPos;
        // Debug.Log("NPC already has weapon");
        if (distanceToPlayer <= 15)
        {
            if (playerPosPredicted > 0)
            {
                ManagePlayerPosQueue(queueManageVaribeles.X, queueManageVaribeles.EMPTYQUEUE);
                ManagePlayerPosQueue(queueManageVaribeles.Z, queueManageVaribeles.EMPTYQUEUE);
                playerPosPredicted = 0;
            }
            if (countFrame % (changeDirectionDurtion / changeDirectionDurtion) == 0)
            {
                Debug.Log("Add Player Position to List");
                ManagePlayerPosQueue(queueManageVaribeles.X, queueManageVaribeles.APPENDQUEUE);
                ManagePlayerPosQueue(queueManageVaribeles.Z, queueManageVaribeles.APPENDQUEUE);
            }

            //if (distanceToPlayer <= 3)
            //{
            //    attacking = true;
            //    // StartCoroutine("NPCAttackPlayer");
            //}

            //else
            //{
                if (currentWeapon.GetComponent<Wrench>().GetAttackMode() == false)
                {
                    playerIsNear = true;
                    float randomX = Random.Range(-1.0f, 1.0f);
                    float randomZ = Random.Range(-1.0f, 1.0f);
                    Vector3 dx = (playerPos - new Vector3(randomX, 0.0f, randomZ) - NPCPos) / Vector3.Distance(playerPos, NPCPos);
                    // Vector3 dx = (playerPos - NPCPos) / Vector3.Distance(playerPos, NPCPos);
                    dx.y = 0;
                    transform.position += dx * 0.05f * Time.deltaTime;
                    lookAtPos = player.transform.position;
                    lookAtPos.y = NPCPos.y;
                    transform.LookAt(lookAtPos);
                    attacking = false;
                }
            //}
        }
        else
        {
            Debug.Log(playerPosXList.Count);
            if (playerPosPredicted > 0)
            {
                playerPosPredicted -= 1;
                predictedPlayerPos = new Vector3(predict(playerPosXList, 50 - playerPosPredicted), NPCPos.y, predict(playerPosZList, 50 - playerPosPredicted));
                // predictedPlayerPos.y = NPCPos.y;
                //Debug.Log("NPC Move to predicted position");
                //Debug.Log(playerPosPredicted);
                //Debug.Log(predictedPlayerPos);
                Vector3 dx = (predictedPlayerPos - NPCPos) / Vector3.Distance(predictedPlayerPos, NPCPos);
                dx.y = 0;
                transform.position += dx * 0.05f * Time.deltaTime;
                lookAtPos = predictedPlayerPos;
                lookAtPos.y = NPCPos.y;
                transform.LookAt(lookAtPos);
                if (playerPosPredicted == 0)
                {
                    ManagePlayerPosQueue(queueManageVaribeles.X, queueManageVaribeles.APPENDQUEUE);
                    ManagePlayerPosQueue(queueManageVaribeles.Z, queueManageVaribeles.APPENDQUEUE);
                }
                return;
            }
            else
            {
                if (playerPosXList.Count >= 100)
                {
                    predictedPlayerPos = new Vector3(predict(playerPosXList, 0), NPCPos.y, predict(playerPosZList, 0));
                    Debug.Log(predictedPlayerPos);
                    playerPosPredicted = 50;
                    Vector3 dx = (predictedPlayerPos - NPCPos) / Vector3.Distance(predictedPlayerPos, NPCPos);
                    dx.y = 0;
                    transform.position += dx * 0.05f * Time.deltaTime;
                    transform.LookAt(predictedPlayerPos);
                }
                else
                {
                    if (countFrame % changeDirectionDurtion == 0)
                    {
                        float y = Random.Range(0, 360);
                        transform.Rotate(0, y, 0);
                    }
                    transform.position += Vector3.forward * Time.deltaTime;
                    playerIsNear = false;
                    attacking = false;
                }
            }
        }
    }


    private IEnumerator RandomDirectionChanged()
    {
        if (!playerIsNear)
        {
            yield return new WaitForSecondsRealtime(1f);

            float y = Random.Range(0, 360);// Randomly pick a rotation direction

            transform.Rotate(0, y, 0);
        }
    }

    public void DeathEffect()
    {

        GameObject bone = GameObject.Find("EnvironmentManager").GetComponent<Environment>().bone;
        GameObject deathBone = Instantiate(bone, transform.position, Quaternion.identity);
        deathBone.AddComponent<Rigidbody>().useGravity = true;
        deathBone.AddComponent<BoxCollider>();
        deathBone.transform.localScale *= 5;

        Destroy(gameObject);
        Destroy(healthBar);
    }


    private void updateHealthBar()
    {
        Vector3 barWorldPosition = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head").transform.position;
        //Vector3 barWorldPosition = transform.position;
        Vector3 screenPos = cam.WorldToScreenPoint(barWorldPosition);
        screenPos.y += 20;
        healthBar.transform.position = screenPos;

        //healthBar.SetActive(true);

        healthBar.GetComponent<Scrollbar>().size = health / maxHealth;
        if (health < maxHealth / 2)
        {
            ColorBlock cb = healthBar.GetComponent<Scrollbar>().colors;
            cb.disabledColor = new Color(1.0f, 0.0f, 0.0f);
            healthBar.GetComponent<Scrollbar>().colors = cb;
        }
        else
        {
            ColorBlock cb = healthBar.GetComponent<Scrollbar>().colors;
            cb.disabledColor = new Color(0.0f, 1.0f, 0.25f);
            healthBar.GetComponent<Scrollbar>().colors = cb;
        }
        //healthBar.SetActive(false);
    }

    private IEnumerator CheckInSafeZone()
    {
        while (true)
        {
            GameObject sphere = GameObject.Find("Sphere");
            Vector3 SafeZoneCenter = sphere.transform.position;
            float SafeZoneRadius = sphere.transform.localScale.x * sphere.GetComponent<SphereCollider>().radius;
            if (Vector3.Distance(transform.position, SafeZoneCenter) > SafeZoneRadius)
            {
                health -= 0.1f;
                Debug.Log(transform.name + " is outside the safe zone!");
            }
            yield return new WaitForSecondsRealtime(1f);
        }

    }

    private Vector3 getSafeZoneCoord()
    {
        GameObject sphere = GameObject.Find("Sphere");
        Vector3 SafeZoneCenter = sphere.transform.position;
        float SafeZoneRadius = sphere.transform.localScale.x * sphere.GetComponent<SphereCollider>().radius;
        if (Vector3.Distance(transform.position, SafeZoneCenter) > SafeZoneRadius)
        {
            health -= 0.1f;
            // Debug.Log(transform.name + " is outside the safe zone!");
            return sphere.transform.position;
        }
        else
        {
            return new Vector3(-1, -1, -1);
            //If the returned position is (-1, -1, -1), the NPC is inside the safe zone
        }
    }
    private void ManageNPCPosQueue(Vector3 NPCPos)
    {
        if (npcPosQueue.Count < 5)
            return;
        npcPosQueue.RemoveAt(0);
        npcPosQueue.Add(NPCPos);
        if (NPCIsStuck())
        {
            float y = Random.Range(0, 360);
            transform.Rotate(0, y, 0);
            StartCoroutine("Jump");
        }
    }

    //index = 0
    //index = 1
    //method = 0 
    //method = 1
    private void ManagePlayerPosQueue(int index, int method)
    {
        if (index == queueManageVaribeles.X)
        {
            switch (method)
            {
                case queueManageVaribeles.APPENDQUEUE:
                    playerPosXList.Add(playerPos.x);
                    break;
                case queueManageVaribeles.EMPTYQUEUE:
                    playerPosXList.Clear();
                    break;
            }
        }
        if (index == queueManageVaribeles.Z)
        {
            switch (method)
            {
                case queueManageVaribeles.APPENDQUEUE:
                    playerPosZList.Add(playerPos.z);
                    break;
                case queueManageVaribeles.EMPTYQUEUE:
                    playerPosZList.Clear();
                    break;
            }
        }

    }

    private bool NPCIsStuck()
    {
        float npcMovementPositionDiff = 0;
        for (int i = 1; i < npcPosQueue.Count; i++)
        {
            // npcMovementPositionDiff += Mathf.Abs()
            npcMovementPositionDiff += Mathf.Abs(Vector3.Distance(npcPosQueue[i], npcPosQueue[i - 1]));
        }
        if (npcMovementPositionDiff <= 4)
        {
            return true;
        }
        else
            return false;
    }


    private IEnumerator Jump()
    {
        m_animator.SetBool("Jump", true);
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(Time.deltaTime / 2);

            float new_x = transform.position.x;
            float new_y = transform.position.y;
            float new_z = transform.position.z;

            float v = 9.9f;
            float m = m_rb.mass;
            float G = -m * gravity;

            float dv = G / m;
            float dy = v + dv;

            transform.position = new Vector3(new_x, new_y + dy, new_z);
            m_animator.SetBool("Jump", false);
        }
    }


    private float predict(List<float> playerCoordnate, int num)
    {
        Debug.Log("NPC Predicted Player Position");
        return predictPlayerMovement.predictMovement(playerCoordnate, playerCoordnate.Count, 2, num);
    }

}

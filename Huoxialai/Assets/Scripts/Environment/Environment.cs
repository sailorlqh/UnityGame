using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    // Abstract objects
    public GameObject[] weaponEntries = new GameObject[] { };
    public GameObject bone;
    public float circleShrinkSpeed = 0.01f;
    public GameObject poisonCircle;

    private List<GameObject> weaponsOnGround;
    private Bounds bounds; // Game field bounds
    private List<GameObject> NPCList;

    // Concrete objects
    public GameObject NPC;
    public GameObject[] groundType = new GameObject[] {};
    public GameObject[] naturalObjectType = new GameObject[] {};

    private GameObject Grounds;
    private GameObject Weapons;
    private GameObject Environments;
    private GameObject NPCs;

    public static List<GameObject> weaponPositionList;

    // public class weaponPos{
    //     float x;
    //     float y;
    //     float z;
    //     public weaponPos(float a, float b, float c){
    //         this.x = a;
    //         this.y = b;
    //         this.z = c;
    //     }
    //     public string printPos() {
    //         return "X:" + x.ToString() + " Z: " + z.ToString();
    //     }
    // }

    // Start is called before the first frame update
    void Awake()
    {
        
        weaponPositionList = new List<GameObject>();

        Grounds = new GameObject("Grounds");
        Weapons = new GameObject("Weapons");
        Environments = new GameObject("Environments");
        NPCs = new GameObject("NPCs");

        bounds = GameObject.Find("Terrain").GetComponent<Collider>().bounds;

        // Play background music
        SoundManager.instance.playSound("gameBGM");

        GenerateGround();
        GenerateNatureObjects(500);
        GenerateWeaponsOnGround(2000);
        GenerateNPCsOnGround(10);//10

        StartCoroutine("PoisonCircle");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateGround() {
        for(int i = (int) bounds.min[0]; i < bounds.max[0]; ) {
            for(int j = (int) bounds.min[2]; j < bounds.max[2];) {
        // for(int i = 0; i < 10; i++) {
            // for(int j = 0; j < 10; j++) {
                int type = Random.Range(0,2);
                GameObject newGround = Instantiate(groundType[type], new Vector3(i, 0, j), Quaternion.identity, Grounds.transform);
                // i += Random.Range(10,25);
                j += Random.Range(10,25);
            }
            GameObject lastGround = Instantiate(groundType[Random.Range(0, 2)], new Vector3(i, 0, bounds.max[2]+10), Quaternion.identity, Grounds.transform);
            i += Random.Range(10,25);
        }
    }

    private void GenerateNatureObjects(int amount) {
        for (int i = 0; i < amount; i++) {
            int type = Random.Range(0, naturalObjectType.Length);
            float x = Random.Range(bounds.min[0], bounds.max[0]);
            float y = 7.0f;
            if (type == 12 || type == 13)
            {
                y = 0.5f;
            }
                
            float z = Random.Range(bounds.min[2], bounds.max[2]);
            GameObject newNaturalObject = Instantiate(naturalObjectType[type], new Vector3(x, y, z), Quaternion.identity, Environments.transform);
        }
    }


    private void GenerateWeaponsOnGround(int amount)// Generate the weapons on the ground for players to pick up
    {
        for(int i=0; i< amount; i++)
        {
            int k = Random.Range(0, weaponEntries.Length);
            float x = Random.Range(bounds.min[0], bounds.max[0]);
            float y = 10.0f;
            float z = Random.Range(bounds.min[2], bounds.max[2]);
            // if(k < 4) k = 5;
            // else k = 7;
            GameObject weapon = Instantiate(weaponEntries[k], new Vector3(x, y, z), Quaternion.identity, Weapons.transform);
            weapon.transform.localScale *= 5;
           if(k == 0 || k == 1 || k == 3 )
            weaponPositionList.Add(weapon);
            //weaponsOnGround.Add(weapon);
            //LightenWeapon(weapon);
        }
    }

    private void GenerateNPCsOnGround(int amount)// Generate NPCs on the ground
    {
        for (int i = 0; i < amount; i++)
        {
            float x = Random.Range(bounds.min[0], bounds.max[0]);
            float y = 7.0f;
            float z = Random.Range(bounds.min[2], bounds.max[2]);
            GameObject npc = Instantiate(NPC, new Vector3(x, y, z), Quaternion.identity, NPCs.transform);
            //NPCList.Add(npc);
        }
    }

    private IEnumerator PoisonCircle()// Shrink the circle randomly
    {
        Vector3 center;
        center = new Vector3(Random.Range(bounds.min[0]+50, bounds.max[0]-50), 0, Random.Range(bounds.min[2]+50, bounds.max[2]-50));
        while (true)
        {
            poisonCircle.transform.position = center;

            float sx = poisonCircle.transform.localScale.x - circleShrinkSpeed;
            float sy = poisonCircle.transform.localScale.y - circleShrinkSpeed;
            float sz = poisonCircle.transform.localScale.z - circleShrinkSpeed;
            poisonCircle.transform.localScale = new Vector3(sx, sy, sz);
            yield return new WaitForSecondsRealtime(1f);
        }

    }

    //private void LightenWeapon(GameObject weapon)
    //{
    //    float x = weapon.transform.position.x;
    //    float y = weapon.transform.position.y;
    //    float z = weapon.transform.position.z;
    //    Light light = Instantiate(weaponHighlight, new Vector3(x, 14.0f, z), Quaternion.identity);
    //    light.GetComponent<Light>().range = 5;
    //    light.GetComponent<Transform>().rotation = new Quaternion(0, 0, 0, 0);
    //    light.transform.parent = weapon.transform;
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{

    public GameObject currentWeapon = null;
    public GameObject currentShield = null;
    public float health = 5f;
    public GameObject healthBar;
    public float maxHealth = 5f;

    public float rotateSpeed = 3.0F;
    public float runningSpeed = 0.3f;//0.1f
    public float walkingSpeed = 0.05f;
    private Transform tran;
    private Animator playerAnimator;
    private Rigidbody m_rb;
    CharacterController controller;
    float gravity = 9.8f;
    private GameObject playerWeapon;
    int currentState;
    private int numWeaponNPCHave;
    private int maxWeaponNum;
    private Camera cam;
    private GameDisplay gameDisplayClass;
    //recognize voice
    //private KeywordRecognizer keywordRecognizer;
    // Start is called before the first frame update
    //private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    //private int act;

    
    public void increaseWeaponNum() {
        if(numWeaponNPCHave < maxWeaponNum) {
            numWeaponNPCHave += 1;
        }
        Debug.Log("Now this Player has one more weapon");
    }

    public void decreaseWeaponNum()
    {
        if (numWeaponNPCHave > 0)
        {
            numWeaponNPCHave --;
        }
        Debug.Log("Weapon used");
    }

    public void getHurt(float ATK)
    {
        health -= ATK;
        StartCoroutine("hurt");
    }

    public IEnumerator hurt()
    {
        playerAnimator.SetBool("Hurt", true);
        yield return new WaitForSeconds(Time.deltaTime * 10);
        playerAnimator.SetBool("Hurt", false);

    }

    //private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    //{
    //    Debug.Log(speech.text);
    //    actions[speech.text].Invoke();
    //}

    //private void Forward()
    //{
    //    //transform.Translate(1, 0, 0);
    //    act = 1;
    //}

    //private void Left()
    //{
    //    //transform.Translate(1, 0, 0);
    //    act = 2;
    //}

    //private void Right()
    //{
    //    act = 3;
    //    //transform.Translate(1, 0, 0);
    //}

    //private void Back()
    //{
    //    act = 4;
    //    //transform.Translate(1, 0, 0);
    //}

    //private void JumpAct()
    //{
    //    act = 5;
    //    //transform.Translate(1, 0, 0);
    //}


    void Start()
    {
        //act = 0;
        //actions.Add("forward", Forward);
        //actions.Add("left", Left);
        //actions.Add("right", Right);
        //actions.Add("back", Back);
        //actions.Add("jump", JumpAct);
        //keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        //keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;

        gameDisplayClass = GameObject.Find("GameManager").GetComponent<GameDisplay>();

        tran = gameObject.GetComponent<Transform>();
        playerAnimator = gameObject.GetComponent<Animator>();
        m_rb = gameObject.GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        currentState = 0;
        playerWeapon = GameObject.Find("Player/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Weapon");
        playerWeapon.SetActive(false);
        numWeaponNPCHave = 0;
        maxWeaponNum = 1;
        cam = GameObject.Find("Player/MainCamera").GetComponent<Camera>();
        healthBar = Instantiate(healthBar, GameObject.Find("Canvas").transform);
        healthBar.transform.localScale *= 2;

        StartCoroutine("CheckInSafeZone");
    }

    // Update is called once per frame
    void Update()
    {
        currentState = 0;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        updateHealthBar();

        // Check death
        if (health <= 0)
        {
            DeathEffect();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //act = 0;
            StartCoroutine("Jump");
        }
        else
        {
            if (Mathf.Abs(v) > 0.1)//Check whether pressed WS
            {
                if (Input.GetKey(KeyCode.W))
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        playerAnimator.SetFloat("transit", 1.0f);
                        tran.Translate(Vector3.forward * runningSpeed);
                    }
                    else
                    {
                        playerAnimator.SetFloat("transit", 0.3f);
                        tran.Translate(Vector3.forward * walkingSpeed);
                    }
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        playerAnimator.SetFloat("transit", 1f);
                        tran.Translate(- Vector3.forward * runningSpeed);
                    }
                    else
                    {
                        playerAnimator.SetFloat("transit", 0.3f);
                        tran.Translate(- Vector3.forward * walkingSpeed);
                    }
                }
                else
                {
                    playerAnimator.SetFloat("transit", 0.0f);
                }
            }
            if (Mathf.Abs(h) > 0.1)
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    playerAnimator.SetFloat("transit", 1.0f);
                    transform.Rotate(0, h * rotateSpeed, 0);
                }
                else
                {
                    playerAnimator.SetFloat("transit", 0.3f);
                    transform.Rotate(0, h * rotateSpeed, 0);//Rotate in horizontal when pressing AD
                }
                
            }
            if (Mathf.Abs(h) <= 0.1 && Mathf.Abs(v) <= 0.1)
            {
                playerAnimator.SetFloat("transit", 0.0f);
            }
        }
        // tran.Rotate(0f,0f,0f);
    }

    public void Throw()
    {
        currentWeapon.SendMessage("ThrowMe");
    }

    public void Cast()
    {
        currentWeapon.SendMessage("RangeAttack");
    }




    private IEnumerator Jump()
    {
        playerAnimator.SetBool("Jump", true);
        for (int i=0; i<20; i++)
        {
            yield return new WaitForSeconds(Time.deltaTime/2);

            float new_x = transform.position.x;
            float new_y = transform.position.y;
            float new_z = transform.position.z;

            float v = 9.9f;
            float m = m_rb.mass;
            float G = - m * gravity;

            float dv = G/m;
            float dy = v + dv;

            transform.position = new Vector3(new_x, new_y + dy, new_z);
            playerAnimator.SetBool("Jump",false);
        }
    }

    public void DeathEffect() {

        GameObject bone = GameObject.Find("EnvironmentManager").GetComponent<Environment>().bone;
        GameObject deathBone = Instantiate(bone, transform.position, Quaternion.identity);
        deathBone.AddComponent<Rigidbody>().useGravity = true;
        deathBone.AddComponent<BoxCollider>();
        deathBone.transform.localScale *= 5;

        Destroy(healthBar);
        // Destroy(gameObject);
        Debug.Log("Player Dead");
        //SceneManager.LoadScene(0);
    }

    private void updateHealthBar()
    {
        Vector3 barWorldPosition = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head").transform.position;
        //Vector3 barWorldPosition = transform.position;
        Vector3 screenPos = cam.WorldToScreenPoint(barWorldPosition);
        screenPos.y += 40;
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
                gameDisplayClass.safeZoneAlert = true;
                gameDisplayClass.defaultPrompt = "Fall back to safe zone!!";
                gameDisplayClass.refresh = true;

                SoundManager.instance.playSound("heartbeat");
                //gameDisplayClass.promptTextObject.text = "Fall back to safe zone!!";
                //GameObject.Find("Canvas/Warning").SetActive(true);// Show bloody warning
            }
            else
            {
                gameDisplayClass.safeZoneAlert = false;
                //GameObject.Find("GameManager").GetComponent<GameDisplay>().defaultPrompt = "";// No prompt for backing safe zone
                //GameObject.Find("Canvas/Warning").SetActive(false);// No bloody warning
            }
            yield return new WaitForSecondsRealtime(1f);
        }

    }
}

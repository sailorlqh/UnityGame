using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameObject player;
    GameObject NPCs;
    public GameObject helpPanel;
    public static int rank;
    // Start is called before the first frame update
    void Awake() {
        player = GameObject.Find("Player");
        NPCs = GameObject.Find("NPCs");
    }
    void Start()
    {
        
        player = GameObject.Find("Player");
        NPCs = GameObject.Find("NPCs");
    }

    // Update is called once per frame
    void Update()
    {
        
        NPCs = GameObject.Find("NPCs");
        rank = NPCs.transform.childCount + 1;
        if (player.GetComponent<PlayerMovement>().health > 0 && NPCs.transform.childCount == 0)
        {
            SceneManager.LoadScene("WinScene");
        }
        if (player.GetComponent<PlayerMovement>().health <= 0)
        {
            SceneManager.LoadScene("LoseScene");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("hhhhh:" + helpPanel.activeSelf);
            helpPanel.SetActive(!helpPanel.activeSelf);
        }
    }
}

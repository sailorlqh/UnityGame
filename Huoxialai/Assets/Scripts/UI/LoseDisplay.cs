using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoseDisplay : MonoBehaviour
{
    GameObject rankText;
    public AudioClip BGM;
    //GameObject timeText;
    // Start is called before the first frame update
    void Start()
    {
        rankText = GameObject.Find("Canvas/Panel/Rank");
        rankText.GetComponent<Text>().text = "Your rank: " + GameManager.rank.ToString();
        //timeText = GameObject.Find("Canvas/Panel/Time");
        //timeText.GetComponent<Text>().text = "Time spent: " + GameManager.time.ToString();

        //SoundManager.instance.playSound("loseBGM");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RestartButtonOnClick()
    {
        SceneManager.LoadScene("GameScene");
    }
}

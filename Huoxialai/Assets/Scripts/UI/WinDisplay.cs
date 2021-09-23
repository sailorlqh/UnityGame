using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinDisplay : MonoBehaviour
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

        //SoundManager.instance.playSound("winBGM");
    }


    public void RestartButtonOnClick()
    {
        SceneManager.LoadScene("GameScene");
    }
}
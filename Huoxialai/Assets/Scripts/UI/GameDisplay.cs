using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Control the UI of the gaming scene
public class GameDisplay : MonoBehaviour
{
    public GameObject leftText;
    public GameObject promptText;
    public bool safeZoneAlert = false;
    public bool refresh = false;


    public string defaultPrompt = "";
    string promptContext;
    Text promptTextObject;

    public GameObject bloodyEffect;


    // Start is called before the first frame update
    void Start()
    {
        promptTextObject = promptText.GetComponent<Text>();
        promptTextObject.text = defaultPrompt;
    }

    // Update is called once per frame
    void Update()
    {
        leftText.GetComponent<Text>().text = "Enemies left: " + (GameManager.rank - 1).ToString();
        promptTextObject.text = promptContext;
        //promptContext = defaultPrompt;

        if (refresh)
        {
            promptTextObject.text = defaultPrompt;
            refresh = false;
            GameObject.Find("GameManager").GetComponent<GameDisplay>().showPrompt("Fall back to safe zone!!");
        }

        if (safeZoneAlert)
        {
            defaultPrompt = "Fall back to safe zone!!";
            bloodyEffect.SetActive(true);
        }
        else
        {
            defaultPrompt = "";
            bloodyEffect.SetActive(false);
        }

    }

    public void showPrompt(string context)
    {
        promptContext = context;
        StartCoroutine("showPromptCoroutine");
    }

    IEnumerator showPromptCoroutine()
    {
        yield return new WaitForSecondsRealtime(1f);
        promptContext = defaultPrompt;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartButtonOnClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void HelpButtonOnClick()
    {
        SceneManager.LoadScene("HelpScene");
    }
}

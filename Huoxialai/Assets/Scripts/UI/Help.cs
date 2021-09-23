using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Help : MonoBehaviour
{
    public GameObject helpPanel;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            gameObject.SetActive(!helpPanel.activeInHierarchy);
    }

    public void invokeHelp()
    {
        helpPanel.SetActive(!helpPanel.activeInHierarchy);
    }

    public void ret2Home()
    {
        SceneManager.LoadScene("StartScene");
    }
}

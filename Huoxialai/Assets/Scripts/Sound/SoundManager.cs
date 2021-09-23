using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioSource efxSource;
    public static SoundManager instance = null;        //Allows other scripts to call functions from SoundManager.

    public AudioClip gameBGM;
    public AudioClip winBGM;
    public AudioClip loseBGM;
    public AudioClip npcHurt;
    public AudioClip playerHurt;
    public AudioClip drug;
    public AudioClip wrench;
    public AudioClip heartbeat;
    public AudioClip throwingstar;


    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        // DontDestroyOnLoad(gameObject);
    }


    //Used to play Efx sound clips.
    public void PlayEfx(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        efxSource.clip = clip;

        //Play the clip.
        efxSource.Play();
    }

    //Used to play BGM sound clips.
    public void PlayBGM(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        bgmSource.clip = clip;

        //Play the clip.
        bgmSource.Play();
    }

    public void playSound(string name)
    {
        switch (name)
        {
            case "gameBGM":
                PlayBGM(gameBGM); break;
            case "loseBGM":
                PlayBGM(loseBGM); break;
            case "winBGM":
                PlayBGM(winBGM); break;
            case "npcHurt":
                PlayEfx(npcHurt); break;
            case "playerHurt":
                PlayEfx(playerHurt); break;
            case "wrench":
                PlayEfx(wrench); break;
            case "drug":
                PlayEfx(drug); break;
            case "heartbeat":
                PlayEfx(heartbeat); break;
            case "throwingstar":
                PlayEfx(throwingstar); break;
            case "":
                break;

        }
    }
}
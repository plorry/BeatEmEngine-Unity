using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Vars

    public Dictionary<string, AudioClip> audioLibrary;
    private static AudioManager audioManager;
    public Camera listenerObject;
    public AudioSource musicPlayer;
    public AudioSource sfxPlayer;

    // Clips

    public AudioClip playerJump;
    public AudioClip playerAtk;
    public AudioClip playerHit;

    public AudioClip enemyJump;
    public AudioClip enemyAtk;
    public AudioClip enemyHit;

    public AudioClip goodStuff;
    public AudioClip badStuff;

    public AudioClip powerUp;
    public AudioClip powerDown;

    // Songs

    public AudioClip song1;
    public AudioClip song2;
    public AudioClip song3;

    // singleton pattern
    public static AudioManager instance
    {
        get
        {
            audioManager = FindObjectOfType(typeof(AudioManager)) as AudioManager;
            audioManager.Init();
            return audioManager;
        }
    }

    void Init()
    {
        if (audioLibrary == null) audioLibrary = new Dictionary<string, AudioClip>();
    }

    public static void PlayClip(string clipName)
    {
        AudioClip thisClip = null;
        if(instance.audioLibrary.TryGetValue(clipName, out thisClip))
        {
            Debug.Log(clipName);
            instance.sfxPlayer.PlayOneShot(thisClip);
        }
    }

    void Start()
    {
        instance.listenerObject = Camera.main;

        instance.musicPlayer = listenerObject.GetComponents<AudioSource>()[0];
        instance.sfxPlayer = listenerObject.GetComponents<AudioSource>()[1];

        instance.audioLibrary.Add("playerJump", playerJump);
        instance.audioLibrary.Add("playerAtk", playerAtk);
        instance.audioLibrary.Add("playerHit", playerHit);

        instance.audioLibrary.Add("enemyJump", enemyJump);
        instance.audioLibrary.Add("enemyAtk", enemyAtk);
        instance.audioLibrary.Add("enemyHit", enemyHit);

        instance.audioLibrary.Add("goodStuff", goodStuff);
        instance.audioLibrary.Add("badStuff", badStuff);

        instance.audioLibrary.Add("powerUp", powerUp);
        instance.audioLibrary.Add("powerDown", powerDown);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

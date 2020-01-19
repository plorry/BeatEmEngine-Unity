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

    public float fadeDuration = 1.0f;
    public float targetVolume = 1.0f;
    public string songToPlay;
    public bool changingSong = false;
    public bool isPlaying = false;
    public bool isStopping = false;

    // Clips

    public AudioClip playerJump;
    public AudioClip playerLand;
    public AudioClip playerAtk;
    public AudioClip playerHit;

    public AudioClip enemyJump;
    public AudioClip enemyLand;
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
            instance.sfxPlayer.PlayOneShot(thisClip);
        }
    }

    void Awake()
    {
        FindCamera();
        VerifyAudioSources();
        AssignSources();
        instance.audioLibrary.Add("playerJump", playerJump);
        instance.audioLibrary.Add("playerLand", playerLand);
        instance.audioLibrary.Add("playerAtk", playerAtk);
        instance.audioLibrary.Add("playerHit", playerHit);

        instance.audioLibrary.Add("enemyJump", enemyJump);
        instance.audioLibrary.Add("enemyLand", enemyLand);
        instance.audioLibrary.Add("enemyAtk", enemyAtk);
        instance.audioLibrary.Add("enemyHit", enemyHit);

        instance.audioLibrary.Add("goodStuff", goodStuff);
        instance.audioLibrary.Add("badStuff", badStuff);

        instance.audioLibrary.Add("powerUp", powerUp);
        instance.audioLibrary.Add("powerDown", powerDown);

        instance.audioLibrary.Add("song1", song1);
        instance.audioLibrary.Add("song2", song2);
        instance.audioLibrary.Add("song3", song3);
    }

    void FindCamera()
    {
        instance.listenerObject = Camera.main;
    }

    void VerifyAudioSources()
    {
        AudioSource[] sources = instance.listenerObject.GetComponents<AudioSource>();
        if(sources.Length < 2)
        {
            AudioSource newSource = listenerObject.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            VerifyAudioSources();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(instance.listenerObject == null)
        {
            FindCamera();
        }
        VerifyAudioSources();
        AssignSources();
        if(instance.musicPlayer != null)
        {
            HandleMusicPlayer();
        }
    }

    void HandleMusicPlayer()
    {
        if(instance.songToPlay != null && instance.musicPlayer.clip == null)
        {
            PlayMusic(instance.songToPlay);
        }
        if (instance.musicPlayer.volume > instance.targetVolume)
        {
            instance.musicPlayer.volume -= Time.deltaTime * 0.5f;
        }
        if (instance.musicPlayer.volume < instance.targetVolume)
        {
            instance.musicPlayer.volume += Time.deltaTime * 0.5f;
        }
        if (instance.changingSong && instance.musicPlayer.volume == 0f)
        {
            instance.musicPlayer.Stop();
            instance.musicPlayer.Play();
            instance.targetVolume = 1.0f;
        }
        if(instance.musicPlayer.volume == 0f)
        {
            instance.isPlaying = false;
            if (instance.isStopping)
            {
                instance.musicPlayer.Stop();
            }
        }
    }

    void AssignSources()
    {
        if (instance.musicPlayer == null && instance.listenerObject != null)
        {
            instance.musicPlayer = listenerObject.GetComponents<AudioSource>()[0];
            instance.musicPlayer.loop = true;
            instance.sfxPlayer = listenerObject.GetComponents<AudioSource>()[1];
        }
    }

    public static void FadeOutMusic()
    {
        instance.targetVolume = 0f;
    }

    public static void FadeInMusic()
    {
        instance.targetVolume = 1.0f;
    }

    public static void PlayMusic(string songName)
    {
        instance.songToPlay = songName;
        AudioClip newSong = null;
        if (instance.audioLibrary.TryGetValue(songName, out newSong))
        {
            instance.musicPlayer.clip = newSong;
        }
        instance.musicPlayer.Play();
        instance.targetVolume = 1.0f;
        instance.isStopping = false;
        instance.isPlaying = true;
    }

    public static void StopMusic()
    {
        instance.isStopping = true;
        FadeOutMusic();
    }

    public static void ChangeSong(string songName)
    {
        AudioClip newSong = null;
        if (instance.audioLibrary.TryGetValue(songName, out newSong))
        {
            instance.musicPlayer.clip = newSong;
        }
        instance.changingSong = true;
        FadeOutMusic();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Curtain Values
    private bool isFadingOut = false;
    private bool isFadingIn = false;
    public GameObject curtain;
    private bool curtainUp = true;
    private float targetAlpha = 0.0f;

    // Scene MGMT Values
    private string targetScene;
    private bool isChangingScenes = false;
    private bool sceneLoaded = true;

    // Game State Values
    private bool gameStarted = false;
    private int players;
    public GameObject player1;
    public GameObject player2;
    private bool allPlayersDead;
    public GameObject PlayerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        InitializeMenuScreen();
        FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        HandleCurtain();
        if (isChangingScenes)
        {
            HandleSceneLoad();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartNewOnePlayer()
    {
        ChangeToScene("onePlayer");
    }

    public void StartNewTwoPlayer()
    {
        ChangeToScene("twoPlayer");
    }

    private void FadeOut()
    {
        targetAlpha = 1.0f;
        AudioManager.FadeOutMusic();
    }

    private void FadeIn()
    {
        targetAlpha = 0.0f;
    }

    private void HandleCurtain()
    {
        var curtainAlpha = curtain.GetComponent<Image>().color.a;
        if(targetAlpha > curtainAlpha)
        {
            curtainAlpha += Time.deltaTime * 1f;
            if (Mathf.Abs(targetAlpha - curtainAlpha) < 0.1f)
            {
                curtainAlpha = targetAlpha;
            }
            curtain.GetComponent<Image>().color = new Color(0, 0, 0, curtainAlpha);
        }
        if (targetAlpha < curtainAlpha)
        {
            curtainAlpha -= Time.deltaTime * 1f;
            if (Mathf.Abs(targetAlpha - curtainAlpha) < 0.1f)
            {
                curtainAlpha = targetAlpha;
            }
            curtain.GetComponent<Image>().color = new Color(0, 0, 0, curtainAlpha);
        }
        if(curtainAlpha == 1.0f)
        {
            curtainUp = true;
        } else
        {
            curtainUp = false;
        }
    }

    private void ChangeToScene(string sceneName)
    {
        targetScene = sceneName;
        isChangingScenes = true;
        sceneLoaded = false;
        FadeOut();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneLoaded = true;
    }

    private void HandleSceneLoad()
    {
        if (curtainUp && !AudioManager.instance.isPlaying)
        {
            SceneManager.LoadScene(targetScene);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        if (sceneLoaded)
        {
            isChangingScenes = false;
            Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene.name;
            switch (sceneName)
            {
                case "onePlayer":
                    InitializeOnePlayerGame();
                    break;
                case "twoPlayer":
                    InitializeTwoPlayerGame();
                    break;
                case "mainMenu":
                    InitializeMenuScreen();
                    break;
                case "winScreen":
                    InitializeWinScreen();
                    break;
                case "loseScreen":
                    InitializeLoseScreen();
                    break;
            }
            FadeIn();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void InitializeOnePlayerGame()
    {
        AudioManager.PlayMusic("song2");
        SpawnPlayer(1);
        InputHandler.instance.player1 = player1.GetComponent<BattlerBehaviour>();
        InputHandler.instance.InitializeSinglePlayerControls();
    }

    private void InitializeTwoPlayerGame()
    {
        AudioManager.PlayMusic("song2");
        SpawnPlayer(1);
        InputHandler.instance.player1 = player1.GetComponent<BattlerBehaviour>();
        SpawnPlayer(2);
        InputHandler.instance.player2 = player2.GetComponent<BattlerBehaviour>();
        InputHandler.instance.InitializeTwoPlayerControls();
    }

    private void InitializeMenuScreen()
    {
        AudioManager.PlayMusic("song1");
    }

    private void InitializeWinScreen()
    {

    }

    private void InitializeLoseScreen()
    {

    }

    private void SpawnPlayer(int identity)
    {
        switch (identity)
        {
            case 1:
                player1 = Instantiate(PlayerPrefab, new Vector3(-0.75f, -0.45f, 0.40f), Quaternion.identity);
                break;
            case 2:
                player2 = Instantiate(PlayerPrefab, new Vector3(-0.75f, -0.45f, 0.40f), Quaternion.identity);
                break;
        }        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class GameManager : MonoBehaviour
{
    public enum GameState { title, play, pause, win, death };
    public GameState currentState = GameState.play;
    public GameObject playUI;
    public GameObject pausedUI;
    public GameObject winUI;
    public GameObject titleUI;
    public GameObject deathUI;
    public GameObject instructions;


    public GameObject player;

    public bool gameWin;
    public bool gameOver;

    AudioSource src;

    private static GameManager Instance;

    public static GameManager GetInstance()
    {
        return Instance;
    }

    void Start()
    {
        src = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");


        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.title:
                Title();
                break;
            case GameState.play:
                Play();
                break;
            case GameState.pause:
                Pause();
                break;
            case GameState.win:
                Win();
                break;
            case GameState.death:
                Death();
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.play)
            {
                currentState = GameState.pause;
            }
            else if (currentState == GameState.pause)
            {
                currentState = GameState.play;
            }
        }

        if (gameWin)
        {
            currentState = GameState.win;
        }

        if (gameOver)
        {
            currentState = GameState.death;
        }
    }

    void Title()
    {
        titleUI.SetActive(true);
        playUI.SetActive(false);
        Time.timeScale = 0;
    }

    void Play()
    {
        StartCoroutine("Instructions");
        titleUI.SetActive(false);
        playUI.SetActive(true);
        pausedUI.SetActive(false);
        Time.timeScale = 1;
    }

    void Pause()
    {
        pausedUI.SetActive(true);
        Time.timeScale = 0;
    }

    void Win()
    {
        winUI.SetActive(true);
        playUI.SetActive(false);
        Time.timeScale = 0;
    }

    void Death()
    {
        deathUI.SetActive(true);
        playUI.SetActive(false);
        Time.timeScale = 0;
    }

    IEnumerator Instructions()
    {
        yield return new WaitForSeconds(1.5f);
        instructions.SetActive(false);
    }
}

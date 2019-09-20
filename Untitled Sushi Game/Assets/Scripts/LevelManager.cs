using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Text t_Score;
    public Text t_Text;
    public Text h_Score;
    public GameObject t_Size;
    public GameObject gameMan;
    Vector3 start;
    public float highScore;
    public float l_Time;
    public float l_MTime;
    public Text r_Counter;
    public float r_Max;
    public float r_Current;

    string sceneName;
    Scene currentScene;
    public enum Levels { menu, one, two, three, four, five };
    public Levels currentLevel;


    public Text lvlOne_hScore;
    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        if (sceneName == "MainMenu")
        {
            currentLevel = Levels.menu;
        }
        if (sceneName == "Level1Prototype")
        {
            currentLevel = Levels.one;
            start = t_Size.transform.position;
            gameMan = GameObject.FindGameObjectWithTag("GameMan");
        }

        switch (currentLevel)
        {
            case Levels.menu:
                Nullifier();
                lvlOne_hScore.text = "¥" + $"{ PlayerPrefs.GetFloat("LVL1HSCORE", highScore)}";
                break;
            case Levels.one:
                l_MTime = 120;
                l_Time = l_MTime;
                r_Max = 20;
                break;
            case Levels.two:
                l_Time = 120;
                break;
            case Levels.three:
                l_Time = 120;
                break;
            case Levels.four:
                l_Time = 120;
                break;
            case Levels.five:
                l_Time = 120;
                break;
        }
        r_Current = r_Max;
    }

    // Update is called once per frame
    void Update()
    {
        if (r_Counter != null)
        {
            r_Counter.text = Mathf.Floor(((r_Current / r_Max) * 100)) + "%";
            StartCoroutine(WaitForStart());
        }
       
        //this all needs to be built up for multiple levels but serves the prototype well
        if (gameMan != null)
        {
            if (gameMan.GetComponent<GameManager>().gameWin)
            {
                if (currentLevel == Levels.one)
                {
                    t_Score.text = "Score: " + $"{Score(l_Time, l_MTime, r_Current, r_Max)}";
                    if (Score(l_Time, l_MTime, r_Current, r_Max) > PlayerPrefs.GetFloat("LVL1HSCORE", highScore))
                    {
                        highScore = Mathf.Round(Score(l_Time, l_MTime, r_Current, r_Max));
                        PlayerPrefs.SetFloat("LVL1HSCORE", highScore);
                        PlayerPrefs.Save();
                    }
                    h_Score.text = "HighScore: " + $"{PlayerPrefs.GetFloat("LVL1HSCORE", highScore)}";
                }
            }
        }
    }

    public void ToSceneOne()
    {
        SceneManager.LoadSceneAsync(1);
        SceneManager.UnloadSceneAsync(sceneName);
    }
  
    void TimerTextFormat()
    {    
            l_Time -= Time.deltaTime;
            if (Mathf.Round(l_Time % 60) >= 10)
            {
                t_Text.text = $"{Mathf.Floor((l_Time / 60))}:{Mathf.Floor(l_Time % 60)}";
            }
            if (Mathf.Floor(l_Time % 60) < 10)
            {
                t_Text.text = $"{Mathf.Round((l_Time / 60))}:0{Mathf.Floor(l_Time % 60)}";
            }
            if (l_Time <= 0)
            {
                t_Text.text = "Times Up!";
                StartCoroutine(GameOver());
            }
            if (l_Time <= 30)
            {
                Hurry();
            }
    }

    void Hurry()
    {
        t_Size.transform.position = Vector3.Lerp(start, new Vector3(t_Size.transform.position.x, t_Size.transform.position.y + 5, t_Size.transform.position.z), Mathf.PingPong(Time.time,.5f));
        t_Text.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1f));
    }

    IEnumerator GameOver()
    {
        gameMan.GetComponent<GameManager>().gameOver = true;
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(0);
        SceneManager.UnloadSceneAsync(sceneName);
    }
    
    IEnumerator WaitForStart()
    {
        yield return new WaitForSeconds(1.5f);
        if (t_Text != null)
        {
            TimerTextFormat();
        }
    }

    public void EraseData()
    {
        PlayerPrefs.DeleteAll();
    }

    void Nullifier()
    {
        t_Text = null;
        t_Size = null;
        t_Score = null;
        r_Counter = null;
        gameMan = null;
        h_Score = null;
    }

  
    public float Score(float x, float y, float r, float rm)
    {
        return Mathf.Round((((x % 60) / (y / 60)) * (r / rm)) * 10);
    }
}

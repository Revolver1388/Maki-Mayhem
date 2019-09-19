using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Text t_Text;
    public GameObject t_Size;
    Vector3 start;
    public float l_Time;
    public GameObject gameMan;
    string sceneName;
    Scene currentScene;

    public Text r_Counter;
    public float r_Max;
    public float r_Current;
    public enum Levels { menu, one, two, three, four, five };
    public Levels currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        gameMan = GameObject.FindGameObjectWithTag("GameMan");
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        start = t_Size.transform.position;
        if (sceneName == "MainMenu")
        {
            currentLevel = Levels.menu;
        }
        if (sceneName == "Level1Prototype")
        {
            currentLevel = Levels.one;
        }

        switch (currentLevel)
        {
            case Levels.menu:
                t_Text = null;
                break;
            case Levels.one:
                l_Time = 120;
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
        r_Counter.text = Mathf.Floor(((r_Current/r_Max) * 100)) + "%";
        StartCoroutine(WaitForStart());
    }

    void Hurry()
    {
        t_Size.transform.position = Vector3.Lerp(start, new Vector3(t_Size.transform.position.x, t_Size.transform.position.y + 5, t_Size.transform.position.z), Mathf.PingPong(Time.time,.5f));
        t_Text.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1f));
    }
    public void ToScene()
    {
        SceneManager.LoadSceneAsync(1);
        SceneManager.UnloadSceneAsync(sceneName);
    }

    IEnumerator WaitForStart()
    {
        yield return new WaitForSeconds(1.5f);
        if (t_Text != null)
        {
            TextFormat();
        }
    }
    void TextFormat()
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

    IEnumerator GameOver()
    {
        gameMan.GetComponent<GameManager>().gameOver = true;
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(0);
        SceneManager.UnloadSceneAsync(sceneName);
    }
}

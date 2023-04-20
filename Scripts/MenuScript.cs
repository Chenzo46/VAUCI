using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text finalScore, HighScore;

    [SerializeField] private GameObject gameGUI, endGUI;

    private float score;

    [SerializeField] private bool isMain = false;

    public void changeScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void quitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (!isMain)
        {
            if (Time.timeScale == 0)
            {
                finalScore.text = "Final Score: " + ((int)score).ToString();

                if (PlayerPrefs.GetInt("High Score") < score)
                {
                    PlayerPrefs.SetInt("High Score", (int)score);
                    HighScore.text = "High Score: " + PlayerPrefs.GetInt("High Score").ToString() + " *NEW*";
                }
                else
                {
                    HighScore.text = "High Score: " + PlayerPrefs.GetInt("High Score").ToString();
                }



                gameGUI.SetActive(false);
                endGUI.SetActive(true);
            }
            else
            {
                score += Time.deltaTime * 5;
                scoreText.text = ((int)score).ToString();
                gameGUI.SetActive(true);
            }
        }
        
    }


}

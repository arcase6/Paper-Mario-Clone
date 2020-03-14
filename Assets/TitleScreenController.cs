using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    public RectTransform MainPanel;
    public RectTransform AboutPanel;

    public UnitData MarioStats;
    public UnitData GoombellaStats;
    public PlayerSharedData SharedData;

    public int GoombellaStartingHP = 8;
    public int MarioStartingHP = 12;

    public int StartingFP = 5;

    public void ShowAboutScreen()
    {
        MainPanel.gameObject.SetActive(false);
        AboutPanel.gameObject.SetActive(true);
    }

    public void HideAboutScreen()
    {
        MainPanel.gameObject.SetActive(true);
        AboutPanel.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        Debug.Log("Quit Application Received");
    #else
        Application.Quit();
    #endif
    }

    public void OpenGameScene()
    {
        //SceneManager.LoadSceneAsync(SceneManager.scene)
        MarioStats.HP = Mathf.Min(MarioStartingHP,MarioStats.MaxHP);
        GoombellaStats.HP = Mathf.Min(GoombellaStartingHP,GoombellaStats.MaxHP);
        SharedData.FP = Mathf.Min(StartingFP,SharedData.MaxFP);
        SceneManager.LoadScene(1,LoadSceneMode.Single);
    }

}

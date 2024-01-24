using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }
    public void PauseGame()
    {
        GameManager.Instance.PauseGame();
    }
    public void BackMainMenu()
    {
        GameManager.Instance.BackMainMenu();
    }
    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
    }
    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }
}

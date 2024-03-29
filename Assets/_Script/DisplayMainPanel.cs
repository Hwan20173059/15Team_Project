using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class DisplayMainPanel : MonoBehaviour
{
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text timerText;
    //[SerializeField] private Text countTanghuluText;

    private float timer;

    void Update()
    {
        if(GameManager.Instance.currentState == GameState.CountDown)
        {
            GameManager.Instance.isStarted = false;
            timer = GameManager.Instance.limitTime;
            GameManager.Instance.SetDifficulty(1);
            UpdateMainSceneMenuDisplay();
        }
        else if (GameManager.Instance.currentState == GameState.Playing)
        {
            GameManager.Instance.isStarted = true;
            timer -= Time.deltaTime;
            timer = Math.Max(timer, 0f);

            // 페이즈 전환 로직
            float runningTime = GameManager.Instance.limitTime - timer;
            if (runningTime < GameManager.Instance.PhaseTwo)
                GameManager.Instance.SetDifficulty(1);
            if (runningTime > GameManager.Instance.PhaseTwo && runningTime < GameManager.Instance.PhaseThree)
                GameManager.Instance.SetDifficulty(2);
            else if (runningTime > GameManager.Instance.PhaseThree)
                GameManager.Instance.SetDifficulty(3);

            UpdateMainSceneMenuDisplay();
        }

        if (timer <= 0f && GameManager.Instance.currentState == GameState.Playing)
        {
            GameManager.Instance.GameOver();
        }
    }
    public void UpdateMainSceneMenuDisplay()
    {
        highScoreText.text = $"최고점수 {GameManager.Instance.highScore}";
        currentScoreText.text = $"현재점수 {GameManager.Instance.score}";
        timerText.text = $"남은시간\n{Math.Max(0f, (int)timer)}";
        //countTanghulu.text = $"탕후루\n{GameManager.Instance.tanghuluMade} / 10";
    }
}

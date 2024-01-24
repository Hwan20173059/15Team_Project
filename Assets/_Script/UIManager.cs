using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // 패널 참조를 위한 변수
    private GameObject settingsPanel;
    private GameObject pausedPanel;
    private GameObject resultPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAndAssignUIElements(scene);
    }

    private void FindAndAssignUIElements(Scene scene)
    {
        // 로드되는 씬 이름에 따라 UI를 로드
        if (scene.name == "TitleScene")
        {
            settingsPanel = GameObject.Find("SettingUI");
        }
        else if (scene.name == "MainScene")
        {
            pausedPanel = GameObject.Find("PauseUI");
            resultPanel = GameObject.Find("ScoreUI");
        }

        // 씬 로드 이벤트 발생할 때마다 초기 상태를 비활성화로 설정
        SetPanelActive(settingsPanel, false);
        SetPanelActive(pausedPanel, false);
        SetPanelActive(resultPanel, false);
    }

    // 패널 활성화/비활성화를 위한 범용 메서드
    public void SetPanelActive(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
    public void SetPanelActive(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
    }

    // 각 패널을 위한 활성화/비활성화 메서드
    public void ToggleSettingsPanel()
    {
        SetPanelActive(settingsPanel, !settingsPanel.activeSelf);
    }
    public void ToggleSettingsPanel(bool isActive)
    {
        SetPanelActive(settingsPanel, isActive);
    }

    public void TogglePausedPanel()
    {
        SetPanelActive(pausedPanel, !pausedPanel.activeSelf);
    }
    public void TogglePausedPanel(bool isActive)
    {
        SetPanelActive(pausedPanel, isActive);
    }

    public void ToggleResultPanel()
    {
        SetPanelActive(resultPanel, !resultPanel.activeSelf);
    }
    public void ToggleResultPanel(bool isActive)
    {
        SetPanelActive(resultPanel, isActive);
    }
}
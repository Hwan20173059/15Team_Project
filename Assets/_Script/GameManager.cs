using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public enum GameState
{
    MainMenu,
    CountDown,
    Playing,
    Paused,
    GameOver,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlaySoundManager soundManager;

    [HideInInspector] public GameState currentState;

    public int currentPhase = 1;
    [HideInInspector] public int tanghuluMade = 0;
    [HideInInspector] public int highScore = 0; // PlayerPrefs로 초기화
    [HideInInspector] public int score = 0;

    private int minFruitType = 0;
    private int maxFruitType = 2;
    int[] targetTanghulu = new int[3];

    FruitsType[] fruitTypes;
    [HideInInspector] public Sprite[] fruitSprites;
    private Sprite stickSprites;
    private Vector3[] positions; // 프리팹이 생성될 위치

    // 데이터나 기타 변수 작성
    public float BGMSound = 0.1f; // 게임 배경음
    public float PlaySound = 0.1f; // 게임 효과음

    public float limitTime = 60f;   // 게임 제한시간
    public int PhaseTwo = 9;
    public int PhaseThree = 19;
    [HideInInspector] public bool isStarted; // 카운트 도중 일시정지-이어하기 시 시간 흐르는 버그 해결 용도

    private void Awake()
    {
        Application.targetFrameRate = 60;
        // 싱글톤 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGameState();

            // PlayerPrefs를 사용하는 경우
            highScore = PlayerPrefs.GetInt("HighScore", 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGameState()
    {
        // 게임 상태를 MainMenu로 초기화
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        // 상태 변경에 따른 추가 로직
        switch (currentState)
        {
            case GameState.MainMenu: // 메인 메뉴 로직
                Time.timeScale = 1;
                Invoke("displayHighscore", 0.2f);
                break;

            case GameState.CountDown:
                Time.timeScale = 1;
                break;

            case GameState.Playing: // 게임 시작 로직
                Time.timeScale = 1;
                break;

            case GameState.Paused: // 일시 정지 관련 로직
                Time.timeScale = 0;
                break;

            case GameState.GameOver: // 게임 종료 관련 로직
                Time.timeScale = 0; // (필요 시 조율) 타임스케일 일시정지
                break;
        }
    }

    // 게임시작 버튼이 눌렸을 때, ButtonManager에서 호출
    public void StartGame()
    {
        ResetData();
        StartCoroutine(LoadMainSceneAndInitialize());
    }

    // <<<--- 여기부터 씬 로딩 후에 목표 탕후루 생성을 위한 작업
    IEnumerator LoadMainSceneAndInitialize()
    {
        // 비동기 씬 로딩 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("_MainScene");

        // 로딩 완료까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 씬 로딩 완료 후 초기화 작업 수행
        ChangeState(GameState.CountDown);
        soundManager = GameObject.Find("Manager").transform.Find("PlaySoundManager").gameObject.GetComponent<PlaySoundManager>();
        GenerateTargetTanghulu();
    }

    // MainScene 비동기 로드
    public void LoadMainSceneAsync()
    {
        StartCoroutine(LoadMainSceneAndInitialize());
    }
    // --->>> 여기까지 StartGame() 수행

    public void displayHighscore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        Text highscore = GameObject.Find("HighScore").GetComponent<Text>();
        highscore.text = $"최고점수 : {highScore}";
    }

    // 세팅 버튼이 눌렸을 때, ButtonManager에서 호출(까진 안해도 되지만 일단 마련)
    public void ToggleSettings()
    {
        UIManager.Instance.ToggleSettingsPanel();
    }
    // 일시정지 버튼이 눌렸을 때, ButtonManager에서 호출
    public void PauseGame()
    {
        UIManager.Instance.TogglePausedPanel(); // 일시 정지 메뉴 토글
        ChangeState(GameState.Paused);
    }
    // 이어하기 버튼이 눌렸을 때, ButtonManager에서 호출
    public void ResumeGame()
    {
        UIManager.Instance.TogglePausedPanel(); // 일시 정지 메뉴 토글
        if (!isStarted) ChangeState(GameState.CountDown);
        else ChangeState(GameState.Playing);
    }
    // 다시하기 버튼이 눌렸을 때, ButtonManager에서 호출
    public void RestartGame()
    {
        ResetData();
        LoadMainScene();
        ChangeState(GameState.CountDown);
        Invoke("GenerateTargetTanghulu", 0.2f);
    }
    // 메인메뉴로 버튼이 눌렸을 때, ButtonManager에서 호출
    public void BackMainMenu()
    {
        LoadTitleScene();
        ChangeState(GameState.MainMenu);
    }
    // 게임 종료 조건 달성 시, GameManager에서 호출
    public void GameOver()
    {
        if (highScore < score) // 결과 오브젝트(일단은 텍스트) ON
        {
            soundManager.highClearSoundPlay();
            UIManager.Instance.SetPanelActive(UIManager.Instance.highScoreTextObject, true);
            UIManager.Instance.SetPanelActive(UIManager.Instance.resultTextObject, false);
        }
        else
        {
            soundManager.clearSoundPlay();
            UIManager.Instance.SetPanelActive(UIManager.Instance.highScoreTextObject, false);
            UIManager.Instance.SetPanelActive(UIManager.Instance.resultTextObject, true);
        }
        UpdateHighScore(); // 최고 점수 업데이트 및 저장
        UIManager.Instance.ToggleResultPanel(); // 게임 오버 UI 활성화
        ChangeState(GameState.GameOver);
    }
    // 게임 Quit 메서드. 사용 안해도 되지만 일단 마련
    public void QuitGame()
    {
        Application.Quit();

        // 유니티 에디터에서 실행 중인 경우 에디터 플레이 모드 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // MainScene 로드
    public void LoadMainScene()
    {
        SceneManager.LoadScene("_MainScene");
    }
    // TitleScene 로드
    public void LoadTitleScene()
    {
        SceneManager.LoadScene("_TitleScene");
    }

    // 게임 재시작을 위한 게임 데이터 초기화 메서드
    public void ResetData()
    {
        currentPhase = 1;
        tanghuluMade = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        score = 0;
    }

    // 목표 탕후루 생성 및 표시
    public void GenerateTargetTanghulu()
    {
        // 스프라이트 가져오기
        if (fruitSprites.Length == 0)
        {
            // 스틱 스프라이트
            stickSprites = Resources.Load<Sprite>($"Stick/Stick1");

            // FruitsType 열거형의 모든 값 가져오기
            fruitTypes = (FruitsType[])System.Enum.GetValues(typeof(FruitsType));

            // fruitSprites 배열을 FruitsType의 길이만큼 초기화
            fruitSprites = new Sprite[fruitTypes.Length];

            // 각 과일 타입에 해당하는 스프라이트를 Resources 폴더에서 로드
            for (int i = 0; i < fruitTypes.Length; i++)
            {
                string fruitName = fruitTypes[i].ToString();
                fruitSprites[i] = Resources.Load<Sprite>($"Fruits/{fruitName}");
                if (fruitSprites[i] == null)
                {
                    Debug.LogError($"Resources/Fruits 에서 {fruitName} 파일을 찾을 수 없음");
                }
            }
        }

        // 'TargetTanghulu' GameObject를 찾아서 있으면 삭제
        GameObject existingTarget = GameObject.Find("TargetTanghulu");
        if (existingTarget != null)
        {
            Destroy(existingTarget);
        }

        // 현재 난이도(Phase)에 따라 사용할 수 있는 과일의 최대 인덱스 설정
        int maxFruitTypeIndex = Mathf.Min(fruitSprites.Length - 1, currentPhase + 3);

        for (int i = 0; i < targetTanghulu.Length;)
        {
            // 현재 난이도에 따라 랜덤하게 과일 인덱스를 선택
            int fruitIndex = Random.Range(0, maxFruitTypeIndex + 1);

            // 선택된 과일이 'Bomb'인 경우 다시 랜덤하게 선택
            if (FruitsType.Bomb.ToString() == fruitTypes[fruitIndex].ToString() || FruitsType.GoldenApple.ToString() == fruitTypes[fruitIndex].ToString())
            {
                continue; // 다음 반복으로 넘어가서 다른 과일 선택
            }

            targetTanghulu[i] = fruitIndex;
            i++; // Bomb가 아닌 과일을 배열에 추가하고 인덱스 증가
        }
        // 캔버스
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        // 'TargetTanghulu' GameObject 생성 및 캔버스의 자식으로 설정
        GameObject targetTanghuluObject = new GameObject("TargetTanghulu");
        //targetTanghuluObject.transform.SetParent(canvas.transform, false);
        Transform imageTransform = GameObject.Find("TanghuluUI").transform.Find("Image");
        targetTanghuluObject.transform.SetParent(imageTransform, false);

        // "TanghuluUI" 기점으로 목표 탕후루를 표시할 위치를 지정
        //RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        //float posX = canvasRectTransform.sizeDelta.x / 2;
        //float posY = canvasRectTransform.sizeDelta.y / 2;
        if (positions == null || positions.Length == 0)
        {
            //Transform tanghuluUIPosition = GameObject.Find("TanghuluUI").transform.Find("Image");
            positions = new Vector3[]
            //{
            //    new Vector3(tanghuluUIPosition.position.x - posX, tanghuluUIPosition.position.y - posY - 50, tanghuluUIPosition.position.z),
            //    new Vector3(tanghuluUIPosition.position.x - posX, tanghuluUIPosition.position.y - posY + 10, tanghuluUIPosition.position.z),
            //    new Vector3(tanghuluUIPosition.position.x - posX, tanghuluUIPosition.position.y - posY + 70, tanghuluUIPosition.position.z)
            //};
            {
                new Vector3(0, -50, 0),
                new Vector3(0, 10, 0),
                new Vector3(0, 70, 0)
            };
        }

        // 스틱 스프라이트 생성
        GameObject stickObject = new GameObject($"Stick");
        Image stickImage = stickObject.AddComponent<Image>();
        stickImage.sprite = stickSprites;
        stickObject.transform.SetParent(targetTanghuluObject.transform, false);
        RectTransform stickRectTransform = stickObject.GetComponent<RectTransform>();
        stickRectTransform.anchoredPosition = positions[0];
        // 스틱 이미지의 너비와 높이 비율 계산하여 사이즈에 반영
        float originalWidth = stickImage.sprite.rect.width;
        float originalHeight = stickImage.sprite.rect.height;
        float desiredHeight = 140f;
        float scale = desiredHeight / originalHeight;
        float desiredWidth = originalWidth * scale;
        stickRectTransform.sizeDelta = new Vector2(desiredWidth, desiredHeight);

        // 선택된 과일 스프라이트를 지정된 위치에 생성
        for (int i = 0; i < targetTanghulu.Length; i++)
        {
            int fruitIndex = targetTanghulu[i];
            Sprite fruitSprite = fruitSprites[fruitIndex];

            if (fruitSprite != null)
            {
                GameObject fruitObject = new GameObject($"Fruit_{fruitIndex}");
                Image fruitImage = fruitObject.AddComponent<Image>();

                // Image 컴포넌트에 과일 스프라이트를 할당
                fruitImage.sprite = fruitSprite;

                // GameObject를 'TargetTanghulu'의 자식으로 설정
                fruitObject.transform.SetParent(targetTanghuluObject.transform, false);

                // RectTransform을 사용하여 GameObject의 위치와 크기를 지정
                RectTransform rectTransform = fruitObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = positions[i];
                rectTransform.sizeDelta = new Vector2(100, 100);
            }
            else
            {
                Debug.LogError($"{fruitIndex}번 과일 못찾음.");
            }
        }

    }

    // Player에서 과일 세개 쌓이면 매개변수로 넣어 호출. 점수반영, 난이도 관리, 종료 조건 검사 수행
    public void UpdateTanghuluProgress(int[] playerTanghulu)
    {
        tanghuluMade++;
        CalculateAndUpdateScore(playerTanghulu); // 플레이어 탕후루와 목표 탕후루를 비교하고 점수를 반영
        GenerateTargetTanghulu();
        Debug.Log("현재 점수: " + score + " / 현재 난이도: " + currentPhase);
    }

    // 난이도 조정
    public void SetDifficulty(int num)
    {
        currentPhase = num;
    }

    // 점수 계산 및 업데이트
    private void CalculateAndUpdateScore(int[] playerTanghulu)
    {
        // 점수 계산 및 업데이트 로직
        int matchCount = 0; // 일치하는 인덱스의 개수를 저장할 변수

        // playerTanghulu와 targetTanghulu 배열 비교하여 일치하는 인덱스의 갯수 확인
        for (int i = 0; i < 3; i++)
        {
            if (playerTanghulu[i] == targetTanghulu[i] || playerTanghulu[i] == 6)
            {
                matchCount++;
            }
        }
        // 일치하는 개수에 따른 점수 산정
        switch (matchCount)
        {
            case 1:
                score += 100; // 1개 일치 시 100점
                break;
            case 2:
                score += 300; // 2개 일치 시 300점
                break;
            case 3:
                score += 600; // 3개 일치 시 600점
                break;
                // 일치하는 개수가 0개인 경우 점수 증가 없음
        }
    }

    // 최고 점수 업데이트 및 저장
    private void UpdateHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            // 게임 종료시에도 로컬에 저장되도록 하는 메서드
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        UIManager.Instance.resultScore.text = GameManager.Instance.score.ToString();
        UIManager.Instance.resultHighScore.text = GameManager.Instance.highScore.ToString();
    }
    public void ResetRecord()
    {
        PlayerPrefs.DeleteKey("HighScore");
        highScore = 0;
        score = 0;
        Invoke("displayHighscore", 0.2f);
    }

    private void TimePause()
    {
        Time.timeScale = 0;
    }

    public void AddScore(int amount)
    {
        score += amount;
        score = Mathf.Max(score, 0);
    }
}
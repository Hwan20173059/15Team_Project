using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera _camera;
    public PlaySoundManager soundManager;

    [SerializeField] private GameObject[] fruitsOnStick;
    [SerializeField] private float moveSpeed = 10f;
    private Rigidbody2D _rigidbody;

    private Vector2 lastMousePosition; // 마우스 마지막 위치
    private const float fixedYPosition = -4f;
    private float arrivalThreshold = 0.1f; // 이동정지를 위한 마우스 최소거리

    Stack<Fruit> fruitStack = new Stack<Fruit>(); // 꽂힌 과일. 폭탄맞으면 후열부터 삭제
    private const int maxFruitCount = 3;
    private const int zero = 0;

    private const int penaltyScore = -100;

    [SerializeField] private GameObject fruitParticle;
    [SerializeField] private GameObject bombParticle;

    private void Awake()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody2D>();
        soundManager = GameObject.Find("Manager").transform.Find("PlaySoundManager").gameObject.GetComponent<PlaySoundManager>();
    }

    private void FixedUpdate()
    {
        MoveStick(lastMousePosition);
    }

    //public void OnTouchPosition(InputValue value)
    //{
    //    Vector2 targetPos = _camera.ScreenToWorldPoint(value.Get<Vector2>());
    //    targetPos.x = Mathf.Clamp(targetPos.x, -2.6f, 2.6f);
    //    targetPos.y = fixedYPosition; // y축 고정
    //    if (targetPos != lastMousePosition)
    //    {
    //        lastMousePosition = targetPos;
    //    }
    //}

    public void OnAim(InputValue value)
    {
        Vector2 targetPos = _camera.ScreenToWorldPoint(value.Get<Vector2>());
        targetPos.x = Mathf.Clamp(targetPos.x, -2.6f, 2.6f);
        targetPos.y = fixedYPosition; // y축 고정
        if (targetPos != lastMousePosition)
        {
            lastMousePosition = targetPos;
        }
    }

    private void MoveStick(Vector2 targetPos)
    {
        // 현재 위치에서 목표 위치까지 부드럽게 이동
        Vector2 newPosition = Vector2.Lerp(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(newPosition);

        // 도착 여부 확인 및 움직임 멈춤
        if (Vector2.Distance(transform.position, targetPos) < arrivalThreshold)
        {
            _rigidbody.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject fruitObj = collision.gameObject;
        if (fruitObj == null)
        {
            Debug.LogWarning("OnTriggerEnter fruit null");
            return;
        }

        Fruit fruit = fruitObj.GetComponent<Fruit>();
        if (fruit.type == FruitsType.Bomb)
        {
            soundManager.bombSoundPlay();
            fruit.gameObject.SetActive(false); // 폭탄은 무조건 숨기기
            bombParticle.SetActive(true);
            PopFruit();
            Invoke("BombParticleActive", .3f);
        }
        else
        {
            soundManager.fruitSoundPlay();
            fruitParticle.SetActive(true);
            PushFruit(fruit);
            Invoke("FruitParticleActive", .3f);
        }

        // 스택이 풀로 차면 완료됐다고 알리고 막대초기화
        if (fruitStack.Count == maxFruitCount)
        {
            soundManager.makeSoundPlay();
            int[] playerTanghulu = StackToArray(fruitStack);

            if (GameManager.Instance != null) { GameManager.Instance.UpdateTanghuluProgress(playerTanghulu); }
            else { Debug.LogWarning("GameManager is Null"); }

            Debug.Log($"완료 {playerTanghulu.Length}/{maxFruitCount}");

            // TODO : 3개 되자마자 빠른속도로 사라진다. 뭔가 만들었다고 이펙트나 딜레이같은것 넣어야할듯
            // 초기화
            InitFruit();
        }
    }

    private void InitFruit()
    {
        foreach (GameObject fruit in fruitsOnStick)
        {
            fruit.SetActive(false);
        }
        fruitStack.Clear();
    }

    private void PopFruit()
    {
        if (fruitStack.Count > zero)
        {
            // 스틱에서 마지막꺼 비활성화
            fruitsOnStick[fruitStack.Count - 1].SetActive(false);
            fruitStack.Pop();
        }
        else
        {
            // 빈 막대기에 폭탄이 들어오는 경우 점수 하락
            GameManager.Instance.AddScore(penaltyScore);
        }
    }

    private void PushFruit(Fruit fruit)
    {
        if (fruitStack.Count < maxFruitCount)
        {
            fruit.gameObject.SetActive(false); // 떨어지던것 숨기고
            fruitStack.Push(fruit);
            // 그려주기
            SpriteRenderer fruitSpriteRenderer = fruitsOnStick[fruitStack.Count - 1].GetComponent<SpriteRenderer>();
            fruitSpriteRenderer.sprite = Resources.Load<Sprite>($"Fruits/{fruit.type}");
            fruitsOnStick[fruitStack.Count - 1].SetActive(true);
        }
    }

    private int[] StackToArray(Stack<Fruit> stack)
    {
        // 스택의 크기만큼 배열 생성
        int[] array = new int[maxFruitCount];
        for (int i = 2; i >= zero; i--)
        {
            array[i] = (int)stack.Pop().type;
        }
        return array;
    }

    private void FruitParticleActive()
    {
        fruitParticle.SetActive(false);
    }
    private void BombParticleActive()
    {
        bombParticle.SetActive(false);
    }
}
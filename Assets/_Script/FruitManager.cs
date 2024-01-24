using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FruitsType
{
    Apple,
    Blueberry,
    Orange,
    Strawberry,
    Banana,
    Bomb,
    GoldenApple,

    //grape,
    //pear,
}

public class FruitManager : MonoBehaviour
{
    [SerializeField] private GameObject stick;
    //[SerializeField] 

    private void Awake()
    {
        // 객체 초기화 및 생성
        if (stick != null && !GameObject.FindGameObjectWithTag(stick.tag))
        {
            Instantiate(stick, new Vector2(0, -4), Quaternion.identity);
        }
    }


    void Start()
    {

    }

    void Update()
    {

    }
}

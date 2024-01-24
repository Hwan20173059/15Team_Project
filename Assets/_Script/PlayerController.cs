using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }


    public void OnAim(InputValue value)
    {
        Vector2 newAim = value.Get<Vector2>();
        Vector2 worldPos = _camera.ScreenToWorldPoint(newAim);
        MoveStick(new Vector2(worldPos.x, -4));
    }

    private void MoveStick(Vector2 pos)
    {
        // 위치 이동. 일단은 순간이동
        gameObject.transform.position = pos;
    }
}
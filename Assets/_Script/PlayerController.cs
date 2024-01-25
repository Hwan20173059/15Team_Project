using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    private Camera _camera;

    [SerializeField] private float moveSpeed = 10f;
    private float fixedYPosition = -4f;
    private Rigidbody2D _rigidbody;

    private Vector2 lastMousePosition; // 마우스 마지막 위치
    private float arrivalThreshold = 0.1f; // 이동정지를 위한 마우스 최소거리

    private void Awake()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnAim(InputValue value)
    {
        Vector2 targetPos = _camera.ScreenToWorldPoint(value.Get<Vector2>());
        targetPos.y = fixedYPosition; // y축 고정
        if (targetPos != lastMousePosition)
        {
            lastMousePosition = targetPos;
        }
        MoveStick(targetPos);
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

    private void FixedUpdate()
    {
        MoveStick(lastMousePosition);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollowMousePosition : MonoBehaviour
{
    private Camera mainCamer;

    private void Awake()
    {
        mainCamer = Camera.main;
    }

    private void Update()
    {
        // 화면의 마우스 좌표를 기준으로 게임 월드 상의 좌표를 구한다.
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        transform.position = mainCamer.ScreenToWorldPoint(position);
        // z 위치를 0으로 설정
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}

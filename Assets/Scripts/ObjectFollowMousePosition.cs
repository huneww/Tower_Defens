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
        // ȭ���� ���콺 ��ǥ�� �������� ���� ���� ���� ��ǥ�� ���Ѵ�.
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        transform.position = mainCamer.ScreenToWorldPoint(position);
        // z ��ġ�� 0���� ����
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}

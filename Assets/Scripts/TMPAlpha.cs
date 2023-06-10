using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPAlpha : MonoBehaviour
{
    [SerializeField]
    private float lerpTime = 0.5f;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void FadeOut()
    {
        StartCoroutine(AlphaLerp(1, 0));
    }

    private IEnumerator AlphaLerp(float start, float end)
    {
        float currentTime = 0;
        float percent = 0;

        while (percent < 1)
        {
            // lerpTime 시간동안 while문 반복
            currentTime += Time.deltaTime;
            percent = currentTime / lerpTime;

            // text의 폰트의 투명도를 start에서 end로 변경
            Color color = text.color;
            color.a = Mathf.Lerp(start, end, percent);
            text.color = color;

            yield return null;
        }
    }
}

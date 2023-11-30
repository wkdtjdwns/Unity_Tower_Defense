using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TmpAlpha : MonoBehaviour
{
    [SerializeField]
    private float lerp_time = 0.5f;
    private TextMeshProUGUI text;

    private void Awake() { text = GetComponent<TextMeshProUGUI>(); }

    public void FadeOut() { StartCoroutine(AlphaLerp(1, 0)); }

    private IEnumerator AlphaLerp(float start, float end)
    {
        float current_time = 0.0f;
        float percent = 0.0f;

        while (percent < 1)
        {
            // lerp_time 값 동안 반복문 실행
            current_time += Time.deltaTime;
            percent = current_time / lerp_time;

            // Text - TextMeshPro의 폰트 투명도를 start에서 end로 변경함
            Color color = text.color;
            color.a = Mathf.Lerp(start, end, percent);
            text.color = color;

            yield return null;
        }
    }
}

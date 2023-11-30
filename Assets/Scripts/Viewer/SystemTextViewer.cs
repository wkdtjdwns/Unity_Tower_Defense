using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum SystemType { Money = 0, Build }

public class SystemTextViewer : MonoBehaviour
{
    private TextMeshProUGUI text_system;
    private TmpAlpha tmp_alpha;

    private void Awake()
    {
        text_system = GetComponent<TextMeshProUGUI>();
        tmp_alpha = GetComponent<TmpAlpha>();
    }

    public void PrintText(SystemType type)
    {
        switch (type)
        {
            case SystemType.Money:
                text_system.text = "System : Not enough money...";
                break;

            case SystemType.Build:
                text_system.text = "System : Invalid build tower...";
                break;
        }

        tmp_alpha.FadeOut();
    }
}

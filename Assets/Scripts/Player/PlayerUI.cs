using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI prompText;

    public void UpdateText(string promptMessage)
    {
        prompText.text = promptMessage;
    }
}

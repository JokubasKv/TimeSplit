using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI prompText;

    private void Start()
    {
        if (prompText == null)
        {
            prompText = UIManager.instance.promptText;
        }
    }

    public void UpdateText(string promptMessage)
    {
        prompText.text = promptMessage;
    }
}

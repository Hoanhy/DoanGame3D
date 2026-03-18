using System.Collections;
using UnityEngine;
using TMPro;

public class WaveMessageUI : MonoBehaviour
{
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    public IEnumerator ShowMessage(string msg, float time = 2f)
    {
        messagePanel.SetActive(true);
        messageText.text = msg;

        yield return new WaitForSeconds(time);

        messagePanel.SetActive(false);
    }
}
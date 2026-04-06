using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityTutorial.PlayerController;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 1f;

    void Start()
    {
        // Khi mới vào game, đảm bảo màn hình sáng lên
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
    }

    public IEnumerator FadeOutAndTeleport(System.Action teleportAction)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true; // Chặn click chuột phá game lúc màn hình đen

        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        teleportAction.Invoke();
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FadeIn());

        // --- BƯỚC GIẢI CỨU PLAYER QUAN TRỌNG NHẤT ---
        fadeImage.raycastTarget = false; // Nhả chuột ra

        // Tìm Player và bật lại script di chuyển
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerController pc = playerObj.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = true; // Bật công tắc cho chạy lại!

            // Giấu chuột đi để xoay góc nhìn
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
using UnityEngine;

public class MusicChanger : MonoBehaviour
{
    public AudioSource bgmSource; // Nơi nhét cái Loa vào
    public AudioClip newMusic;    // Nơi nhét đĩa nhạc vào

    // Gọi hàm này để đổi nhạc
    public void PlayMusic()
    {
        Debug.Log("Đã bấm E và gọi hàm bật nhạc!");
        if (bgmSource != null && newMusic != null)
        {
            bgmSource.clip = newMusic;
            bgmSource.Play();
        }
    }
}
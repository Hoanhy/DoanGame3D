using UnityEngine;

public class MusicChanger : MonoBehaviour
{
    public AudioSource bgmSource; 
    public AudioClip newMusic;    
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
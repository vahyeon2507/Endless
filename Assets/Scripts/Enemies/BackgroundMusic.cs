// Assets/Scripts/Audio/BackgroundMusic.cs
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance { get; private set; }

    [Header("Optional: Assign an AudioClip here")]
    public AudioClip musicClip;

    [Header("Volume (0 – 1)")]
    [Range(0f, 1f)]
    public float volume = 1f;

    void Awake()
    {
        // 싱글톤 설정: 중복 재생 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // AudioSource 세팅
        var src = GetComponent<AudioSource>();
        src.clip = musicClip != null ? musicClip : src.clip;
        src.loop = true;
        src.playOnAwake = false;
        src.volume = volume;

        // 재생
        src.Play();
    }
}

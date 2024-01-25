using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip bgmusic;
    public Slider soundSlider;
    // Start is called before the first frame update
    void Start()
    {
        soundSlider.value = audioSource.volume;
        audioSource.clip = bgmusic;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = soundSlider.value;
    }
}

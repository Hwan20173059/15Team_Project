using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySoundManager : MonoBehaviour
{
    public AudioClip eatClip;
    public AudioSource eatAudioSource;
    public AudioClip boomClip;
    public AudioSource boomAudioSource;
    public Slider soundSlider;
    // Start is called before the first frame update
    void Start()
    {
        soundSlider.value = eatAudioSource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        eatAudioSource.volume = soundSlider.value;
        boomAudioSource.volume = soundSlider.value;
    }

    public void EatSoundPlay()
    {
        eatAudioSource.PlayOneShot(eatClip);
    }

    public void BoomSoundPlay()
    {
        boomAudioSource.PlayOneShot(boomClip);
    }
}

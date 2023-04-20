using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource ad;

    public static AudioManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void playSound(AudioClip clip)
    {
        ad.PlayOneShot(clip);
    }
}

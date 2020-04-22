using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private AudioSource source;
    private float nextAnnouncement = 0F;

    public bool hacked;
    public float announcemntAfter = 0F;
    public List<AudioClip> audioClips;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayElevatorVocals()
    {
        nextAnnouncement = Time.time + announcemntAfter;
        int index = (int)Random.Range(0, audioClips.Count);
        source.PlayOneShot(audioClips[index]);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;
public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;
    public AudioSource audioSourceForBGM;
    public AudioSource audioSourceForSFX;
    private string currentBGM = "";
    private Sequence oldSequence;
    private void Awake() {
        Instance = this;
    }

    public void PlaySFX(string fileName) {
        string path = Path.Join("Audio", "SFXs", fileName);
        var audioClip = Resources.Load<AudioClip>(path);
        audioSourceForSFX.PlayOneShot(audioClip);
    }
    

    public async void PlayBGM(string fileName, bool playOneShot = false) {
        if (playOneShot) {
            string path = Path.Join("Audio", "BGMs", fileName);
            var audioClip = Resources.Load<AudioClip>(path);
            audioSourceForBGM.loop = false;
            audioSourceForBGM.clip = audioClip;
            audioSourceForBGM.Play();
            return;
        }

        if (currentBGM != "" && oldSequence.active) {
            await oldSequence.AsyncWaitForCompletion();
        }
        float originalVolume = audioSourceForBGM.volume;
        Sequence sequence = DOTween.Sequence();
        if (fileName == "") {
            sequence.Insert(0,audioSourceForBGM.DOFade(0, 0.5f));
            sequence.InsertCallback(2f, () => {
                audioSourceForBGM.clip = null;
            });
            sequence.Insert(2f,audioSourceForBGM.DOFade(0.8f, 0));
        } else {
            if (currentBGM == fileName) {
                return;
            }
            string path = Path.Join("Audio", "BGMs", fileName);
            var audioClip = Resources.Load<AudioClip>(path);
            audioSourceForBGM.loop = true;
            sequence.Insert(0,audioSourceForBGM.DOFade(0, 0.5f));
            sequence.InsertCallback(0.5f, () => { 
                audioSourceForBGM.clip = audioClip;
                audioSourceForBGM.Play(); 
            });
            sequence.Insert(0.5f,audioSourceForBGM.DOFade(originalVolume, 0.5f));
        }
        currentBGM = fileName;
        sequence.Play();
        oldSequence = sequence;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

// http://www.voicerss.org/

public class VoiceRSS : MonoBehaviour
{
    public AudioSource audioSource;


    [TextArea(10, 8)]
    public string textContent;

    [Range(-10, 10)]
    [Tooltip("The speech rate (speed). Allows values: from -10 (slowest speed) up to 10 (fastest speed). Default value: 0 (normal speed)")]
    public float voiceSpeed = -1;


    public GameObject loadingUI;
    public GameObject playButton;
    public GameObject pauseButton;

    private AudioClip _audioClip;

    public AudioType audioType;

    private bool _hasRun = false;
    private readonly string _apiKey = "521685a4592548228f2d9cf2a0f54ded";
    private string _url;

    public void OnPlay()
    {
        if (_hasRun == false)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("Error. Check internet connection!");
                return;
            }

            _hasRun = true;
            StartCoroutine(RequestTextToSpeech());
        }
        else
        {
            audioSource.Play();
            pauseButton.SetActive(true);
            playButton.SetActive(false);
        }
    }

    public void OnPause()
    {
        audioSource.Pause();

        pauseButton.SetActive(false);
        playButton.SetActive(true);
    }


    private IEnumerator RequestTextToSpeech()
    {
        playButton.SetActive(false);
        loadingUI.SetActive(true);

        _url = "https://voicerss-text-to-speech.p.rapidapi.com/?r=" + voiceSpeed + "&c=" + audioType + "&f=8khz_8bit_mono&src=" + textContent + "&hl=en-us&key=" + _apiKey;

        UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(_url, audioType);

        webRequest.SetRequestHeader("x-rapidapi-host", "voicerss-text-to-speech.p.rapidapi.com");
        webRequest.SetRequestHeader("x-rapidapi-key", "69c64357e6mshfafd205e447e98bp1b0617jsn84b39a79606d");

        var progress = webRequest.SendWebRequest();

        if (!webRequest.isNetworkError && !webRequest.isHttpError)
        {
            while (!progress.isDone)
            {
                Debug.Log(progress.progress);
                yield return null;
            }

            DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip)webRequest.downloadHandler;

            if (dlHandler.isDone)
            {
                // the file is ready

                Debug.Log(webRequest.downloadProgress);

                _audioClip = dlHandler.audioClip;
                audioSource.clip = _audioClip;
                audioSource.Stop();
                audioSource.Play();

                loadingUI.SetActive(false);
                pauseButton.SetActive(true);
            }
            else
            {
                Debug.Log("Set up file");
            }
        }
        else
        {
            playButton.SetActive(true);
            pauseButton.SetActive(false);
            loadingUI.SetActive(true);

            Debug.LogError("An error has occur: " + webRequest.error);
            _hasRun = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {
    [SerializeField] List<AudioClip> stageMusic;

    //Cached References
    AudioSource music;

    private void Awake() {
        SetUpSingleton();
    }

    // Use this for initialization
    void Start() {
        music = GetComponent<AudioSource>();
    }

    private void SetUpSingleton() {
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ChangeMusic(int musicIndex) {
        StartCoroutine(FadeOutMusic(musicIndex));
    }

    public void InstantChangeMusic(int musicIndex) {
        music.clip = stageMusic[musicIndex];
    }

    IEnumerator FadeOutMusic(int musicIndex) {
        while (music.volume > 0.0f) {
            music.volume -= 0.2f * Time.deltaTime;
            yield return null;
        }
        music.Stop();
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeInMusic(musicIndex));
    }

    IEnumerator FadeInMusic(int musicIndex) {
        music.clip = stageMusic[musicIndex];
        music.Play();
        while (music.volume < 0.50f) {
            music.volume += 0.2f * Time.deltaTime;
            yield return null;
        }
        music.volume = .5f;
        yield return null;
    }

    public void ResetMusic() {
        Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

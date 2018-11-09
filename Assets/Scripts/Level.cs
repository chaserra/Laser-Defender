using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour {
    //Config Parameters
    [SerializeField] float delayInSeconds = 3f;

    public void LoadStartMenu() {
        SceneManager.LoadScene(0);
        FindObjectOfType<MusicPlayer>().ResetMusic();
        FindObjectOfType<GameSession>().ResetGame();
    }

    public void LoadGameScene() {
        SceneManager.LoadScene(1);
    }

    public void PlayAgain() {
        SceneManager.LoadScene(1);
        FindObjectOfType<GameSession>().ResetGame();
    }

    public void LoadGameOver() {
        StartCoroutine(WaitAndLoad());
    }

    IEnumerator WaitAndLoad() {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene("Game Over");
    }

    public void QuitGame() {
        Application.Quit();
    }
}

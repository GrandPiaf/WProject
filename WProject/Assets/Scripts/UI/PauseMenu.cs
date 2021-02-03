using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static AudioManager;

public class PauseMenu : MonoBehaviour
{

    public static bool IsGamePaused = false;
    public static bool IsGameWin = false;

    //public GameObject winMenuUI;
    public GameObject pauseMenuUI;
    public GameObject optionMenuUI;
    public GameObject endGameMenuUI;
    public TextMeshProUGUI endGameMenuUIScoreText;

    public Slider volumeSlider;
    public Slider volumeMusicSlider;
    public Slider volumeSoundSlider;

    public void Start()
    {
        //Adds a listener to the main slider and invokes a method when the value changes.
        volumeSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(volumeType.master); });
        volumeMusicSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(volumeType.music); });
        volumeSoundSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(volumeType.sound); });

        volumeSlider.value = PlayerPrefs.GetFloat("MasterVol", 0.75f);
        volumeMusicSlider.value = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        volumeSoundSlider.value = PlayerPrefs.GetFloat("SoundVol", 0.75f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        optionMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;

    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        optionMenuUI.SetActive(false);
        Time.timeScale = 0f;
        IsGamePaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Reset()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Invoked when the value of the slider changes.
    public void ValueChangeCheck(volumeType type)
    {
        switch (type)
        {
            case volumeType.master:
                AudioManager.instance.mainMixer.audioMixer.SetFloat("MasterVol", Mathf.Log10(volumeSlider.value) * 20);
                PlayerPrefs.SetFloat("MasterVol", volumeSlider.value);
                break;
            case volumeType.music:
                AudioManager.instance.mainMixer.audioMixer.SetFloat("MusicVol", Mathf.Log10(volumeMusicSlider.value) * 20);
                PlayerPrefs.SetFloat("MusicVol", volumeMusicSlider.value);
                break;
            case volumeType.sound:
                AudioManager.instance.mainMixer.audioMixer.SetFloat("SoundVol", Mathf.Log10(volumeSoundSlider.value) * 20);
                PlayerPrefs.SetFloat("SoundVol", volumeSoundSlider.value);
                break;
        }
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ShowEndMenu() {

        Time.timeScale = 0f;
        endGameMenuUI.SetActive(true);
        endGameMenuUIScoreText.text = "Score : " + GameManager.Instance.score;

    }

    public void HideEndMenu() {

        Time.timeScale = 1f;
        endGameMenuUI.SetActive(false);
        GameManager.Instance.ResetGame();
    }
}

using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePlayPanel;
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private GameObject gamePausePanel;
    [SerializeField] private GameObject gameOverPanel;

    void Awake()
    {
        mainMenuPanel.SetActive(true);
        gamePlayPanel.SetActive(false);
        instructionPanel.SetActive(false);
        gamePausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    // When i press the start button , then the mainMenuPanel got disabled, and instruction panel got enabled, 
    // When i tap on the instruction panel, then start game will be called , and one more thing , 
    // when i press the play button , then gameplay panel also got enabled , but the score and pause is still not visible
    void OnEnable()
    {
        GameEvents.OnPlay += EnableInstruction;
        GameEvents.OnGameStarted += StartGame;
        GameEvents.OnPause += PauseGame;
        GameEvents.OnResume += ResumeGame;
        GameEvents.OnMiss += GameOver;
        GameEvents.OnGameRestart += GameRestart;
    }
    void OnDisable()
    {
        GameEvents.OnPlay -= EnableInstruction;
        GameEvents.OnGameStarted -= StartGame;
        GameEvents.OnPause -= PauseGame;
        GameEvents.OnResume -= ResumeGame;
        GameEvents.OnMiss -= GameOver;
        GameEvents.OnGameRestart -= GameRestart;
    }

    void EnableInstruction()
    {
        mainMenuPanel.SetActive(false);
        instructionPanel.SetActive(true);
    }
    void StartGame()
    {
        instructionPanel.SetActive(false);
        gamePlayPanel.SetActive(true);
    }
    void PauseGame()
    {
        gamePausePanel.SetActive(true);
    }
    void ResumeGame()
    {
        gamePausePanel.SetActive(false);
    }
    void GameOver()
    {
        gameOverPanel.SetActive(true);
    }
    void GameRestart()
    {
        gameOverPanel.SetActive(false);
        gamePausePanel.SetActive(false);
    }
}

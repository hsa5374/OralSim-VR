using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using Autohand;

public class GameManager : MonoBehaviour
{

    public NPCManager npcManager = null;
    public GameObject ray = null;
    [Space]
    public int npcInstIndex = 1;
    [Space]
    public int totalTasks = 0;
    public int tasksCompleted = 0;
    public bool isAssessment = false;
    public int assessmentTime = 0;
    public int correctScore = 0;
    public int wrongToolScore = 0;
    public float hoverPercentage;
    [Space]

    public InputActionReference menuBtn;
    public LocomotionSystem locomotionSystem = null;
    public DynamicMoveProvider dynamicMoveProvider = null;
    public AutoHandPlayer autoHandPlayer = null;
    //public SnapTurnProviderBase snapTurnProvider = null;
    //public ContinuousTurnProviderBase continuousTurnProvider = null;

    public bool isPaused = false;

    [Space]
    [Header("UI")]
    public GameObject pausePanel = null;
    public GameObject levelCompletePanel = null;
    public GameObject toolDescCanvas = null;
    public GameObject assessmentPassPanel = null;
    public GameObject assessmentFailPanel = null;
    public GameObject firstQuestionPanel = null;
    [Space]
    public GameObject progressPanel = null;
    public Slider progressSlider;
    public TextMeshProUGUI progressTxt;
    public GameObject[] progressImage = null;
    public AudioSource successAS = null;

    public TextMeshProUGUI timerTextPass = null;
    public TextMeshProUGUI timePassedText = null;
    public TextMeshProUGUI timerTextFail = null;
    public TextMeshProUGUI stepsTextPass = null;
    public TextMeshProUGUI stepsTextFail = null;
    public TextMeshProUGUI totalStepsTextPass = null;
    public TextMeshProUGUI totalStepsTextFail = null;


    public bool isCounting = false;
    [Space]

    public AudioSource audioSource = null; 
    public AudioClip passClip = null;
    public AudioClip failClip = null;

    public float elapsedTime = 0f;



    public InputActionReference menuButtonAction;


    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    private void OnEnable()
    {
        // Enable the input action and subscribe to the performed event
        menuButtonAction.action.Enable();
        menuButtonAction.action.performed += OnMenuButtonPressed;
    }

    private void OnDisable()
    {
        // Disable the input action and unsubscribe from the performed event
        menuButtonAction.action.Disable();
        menuButtonAction.action.performed -= OnMenuButtonPressed;
    }

    private void OnMenuButtonPressed(InputAction.CallbackContext context)
    {
        isPaused = true;
        pausePanel.SetActive(true);
        autoHandPlayer.useMovement = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        //int currentValue = PlayerPrefs.GetInt("IsSnap", 1);
        //if (currentValue == 1)
        //{
        //    continuousTurnProvider.enabled = false;
        //}
        //else
        //{
        //    snapTurnProvider.enabled = false;
        //}

        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
        AudioListener.volume = savedVolume;
        //Debug.LogError(" Volume : " + AudioListener.volume);

        if (PlayerPrefs.GetInt("IsAssessment", 0) == 1)
        {
            isAssessment = true;
            elapsedTime = 0;
            isCounting = true;
            progressPanel.SetActive(true);
            npcManager.npcCharacter.SetActive(false);
            toolDescCanvas.gameObject.SetActive(false);
            npcManager.npcCanvas.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        float pause = menuBtn.action.ReadValue<float>();

        if (pause > 0.5f)
        {
            //isPaused = true;
            ////ray.SetActive(true);
            ////pausePanel.SetActive(true);
            //////locomotionSystem.enabled = false;
            ////dynamicMoveProvider.enabled = false;
            //autoHandPlayer.useMovement = false;
        }
        //if (Input.GetKeyUp(KeyCode.T))
        //{
        //    TaskCompleted();
        //}
        if (isAssessment && !isPaused)
        {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= assessmentTime)
                {
                isPaused = true;
                    AssessmentFail();
                }

                DisplayTime();
            
        }
    }
    void DisplayTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerTextPass.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerTextFail.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timePassedText.text ="Time: "+ string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void TaskCompleted()
    {
        tasksCompleted++;
        if(successAS)
        {
            successAS.Play();
        }
        if(isAssessment)
        {
            float progress = (float)tasksCompleted / totalTasks;
            progressSlider.value = progress;
            progressTxt.text = Mathf.RoundToInt(progress * 100f) + "%";


            //for (int i = 0; i < tasksCompleted; i++)
            //{
            //    progressImage[i].SetActive(true);
            //}


        }
        if (tasksCompleted >= totalTasks)
        {
            if (isAssessment)
            {
                //AssessmentComplete();
                EnableRay();
                firstQuestionPanel.SetActive(true);
                isPaused = true;
            }
            else
            {
                LevelComplete();
            }
        }
        else
        {
            if (!isAssessment)
            {
                NPCNextInstruction();
            }
        }
    }
    public void EnableRay()
    {
        ray.SetActive(true);
    }

    public void AssessmentFail()
    {
        isCounting = false;
        stepsTextFail.text = tasksCompleted.ToString();
        totalStepsTextFail.text = totalTasks.ToString();
        EnableRay();
        if (audioSource)
        {
            audioSource.clip = failClip;
            audioSource.Play();
        }
        assessmentFailPanel.SetActive(true);
    }
    public void AssessmentComplete()
    {
        print("Assessment pass") ;
        int level = PlayerPrefs.GetInt("Level", 1);
        level++;
        if (level <= 3)
        {
            PlayerPrefs.SetInt("Level", level);
        }
        isCounting = false;
        stepsTextPass.text = tasksCompleted.ToString();
        totalStepsTextPass.text = totalTasks.ToString();
        EnableRay();
        if (npcManager.is3rdLevel)
        {
            npcManager.AssestmentCompleteAudio();
        }
        else
        {
            if (audioSource)
            {
                audioSource.clip = passClip;
                audioSource.Play();
            }
        }

        assessmentPassPanel.SetActive(true);
    }

    public void LevelComplete()
    {
       
        
        npcManager.StopNPCAudio();
             
        EnableRay();
        levelCompletePanel.SetActive(true);
    }

    public void NPCNextInstruction()
    {
        npcInstIndex++;
        npcManager.NextInstruction();
    }

    public void StartAssessment(int level)
    {
        PlayerPrefs.SetInt("IsAssessment", 1);
        switch (level)
        {
            case 1:
                SceneManager.LoadScene("Level1");
                return;
            case 2:
                SceneManager.LoadScene("Level2");
                return;
            case 3:
                SceneManager.LoadScene("Level3");
                return;
        }
    }


    public void NextLevel(int level)
    {
        PlayerPrefs.SetInt("IsAssessment", 0);
        switch (level)
        {
            case 1:
                SceneManager.LoadScene("Level1");
                return;
            case 2:
                SceneManager.LoadScene("Level2");
                return;
            case 3:
                SceneManager.LoadScene("Level3");
                return;
        }
    }



    public void MainMenu()
    {
        PlayerPrefs.SetInt("IsAssessment", 0);
        SceneManager.LoadScene("MainMenu");
    }


    public void Resume()
    {
        isPaused = false;
        //ray.SetActive(false);
        //locomotionSystem.enabled = true;
        //dynamicMoveProvider.enabled = true;
        pausePanel.SetActive(false);
        autoHandPlayer.useMovement = true;
    }


    public void CorrectAnswer()
    {
        correctScore++;
    }
}

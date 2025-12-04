using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssessmentPass : MonoBehaviour
{
    public TextMeshProUGUI toolSelectionText = null;
    public TextMeshProUGUI quizScoreText = null;
    public TextMeshProUGUI HoverPercentageText;

    [Space]
    public TextMeshProUGUI overallGradeText = null;
    public AudioSource audioSource = null;
    public AudioClip failClip = null;
    public TextMeshProUGUI text = null;
    public string failText = null;
    public GameObject nextBtn = null;
    public GameObject retryBtn = null;
    int overallPerformance;

    int fGrades = 0;

    private void OnEnable()
    {
        UpdateQuizScore();
        UpdateToolSelection();
    }
    private void Start()
    {
        UpdateQuizScore();
        UpdateToolSelection();
    }


    public void UpdateQuizScore()
    {
        if (GameManager.instance.correctScore == 5)
        {
            quizScoreText.text = "A";
        }
        else if (GameManager.instance.correctScore == 4 || GameManager.instance.correctScore == 3)
        {
            quizScoreText.text = "B";
        }
        else if (GameManager.instance.correctScore == 2 || GameManager.instance.correctScore == 1)
        {
            quizScoreText.text = "C";
        }
        else if (GameManager.instance.correctScore == 0)
        {
            quizScoreText.text = "F";

        }
    }


    public void UpdateToolSelection()
    {
        if (GameManager.instance.wrongToolScore == 1)
        {
            toolSelectionText.text = "A (" + GameManager.instance.wrongToolScore + " Wrong Tool)";
        }
        else if (GameManager.instance.wrongToolScore == 2)
        {
            toolSelectionText.text = "B (" + GameManager.instance.wrongToolScore + " Wrong Tools)";
        }
        else if (GameManager.instance.wrongToolScore == 3)
        {
            toolSelectionText.text = "C (" + GameManager.instance.wrongToolScore + " Wrong Tools)";
        }
        else if (GameManager.instance.wrongToolScore >= 4)
        {
            toolSelectionText.text = "F (" + GameManager.instance.wrongToolScore + " Wrong Tools)";
        }

        float hoverPercentage = GameManager.instance.hoverPercentage;
        HoverPercentageText.text = (Mathf.RoundToInt(hoverPercentage)) + " %";

        OverallGrade();
    }

    public void OverallGrade()
    {
        OverallGradesFunc();
    }

    void OverallGradesFunc()
    {
        // Get numeric values for each grade criteria
        int timeGradeValue = GetGradeValue(GetTimeGrade());
        int toolGradeValue = GetGradeValue(GetToolGrade());
        int hoverGradeValue = GetGradeValue(GetHoverGrade());

        // Calculate average grade value (rounded to nearest whole number)
        float averageValue = (timeGradeValue + toolGradeValue + hoverGradeValue) / 3.0f;

        // Round to nearest whole grade
        int roundedValue = Mathf.RoundToInt(averageValue);

        // Convert back to letter grade
        string finalGrade = GetLetterGrade(roundedValue);

        overallGradeText.text = finalGrade;

        // Handle fail condition
        if (finalGrade == "F")
        {
            text.text = failText;
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.clip = failClip;
                audioSource.Play();
            }
           
            nextBtn.SetActive(false);
            retryBtn.SetActive(true);
        }
        else
        {
            if (!GameManager.instance.npcManager.is3rdLevel)
            {
                nextBtn.SetActive(true);
            }            
            retryBtn.SetActive(false);
        }
    }

    int GetGradeValue(string letterGrade)
    {
        switch (letterGrade)
        {
            case "A": return 4;
            case "B": return 3;
            case "C": return 2;
            case "F": return 0; // F is a larger gap from C
            default: return 0;
        }
    }

    string GetLetterGrade(int gradeValue)
    {
        switch (gradeValue)
        {
            case 4: return "A";
            case 3: return "B";
            case 2: return "C";
            default: return "F"; // 0 or 1 is F
        }
    }

    string GetTimeGrade()
    {
        float time = GameManager.instance.elapsedTime;

        if (time <= 60) // 1 min or less
        {
            return "A";
        }
        else if (time <= 120) // 2:00 min
        {
            return "B";
        }
        else if (time <= 150) // 2:30 min
        {
            return "C";
        }
        else // 3:00 min or more
        {
            return "F";
        }
    }

    string GetToolGrade()
    {
        int wrongTools = GameManager.instance.wrongToolScore;

        if (wrongTools == 1)
        {
            return "A";
        }
        else if (wrongTools == 2)
        {
            return "B";
        }
        else if (wrongTools == 3)
        {
            return "C";
        }
        else // 4+ wrong tools
        {
            return "F";
        }
    }

    string GetHoverGrade()
    {
        float hoverPercentage = GameManager.instance.hoverPercentage;

        if (hoverPercentage >= 90)
        {
            return "A";
        }
        else if (hoverPercentage >= 75 && hoverPercentage < 90)
        {
            return "B";
        }
        else if (hoverPercentage >= 50 && hoverPercentage < 75)
        {
            return "C";
        }
        else // Below 50%
        {
            return "F";
        }
    }
}
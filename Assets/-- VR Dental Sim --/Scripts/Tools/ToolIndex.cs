using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolIndex : MonoBehaviour
{
    public int toolIndex;
    public bool isToolGrabbed;
    public void CheckTool()
    {
        if (toolIndex != GameManager.instance.tasksCompleted)
        {
            GameManager.instance.wrongToolScore++;
            if (!GameManager.instance.isAssessment)
            {
                WrongToolInstruction();
                print("Wrong tool grabbed" + gameObject.name);
            }
        }
        else
        {
            if (!isToolGrabbed && GameManager.instance.isAssessment)
            {
                if (SceneManager.GetActiveScene().name == "Level1")
                {
                    print("Level1");
                    isToolGrabbed = true;
                    if (GameManager.instance.tasksCompleted == 0)
                    {

                        float progress = 0.08f;
                        print("Level1 : Add score" + progress);
                        GameManager.instance.progressSlider.value += progress;
                        GameManager.instance.progressTxt.text = Mathf.RoundToInt(GameManager.instance.progressSlider.value * 100f) + "%";
                    }
                    else if (GameManager.instance.tasksCompleted == 1)
                    {

                        float progress = 0.16f;
                        print("Level1 : Add score" + progress);
                        GameManager.instance.progressSlider.value += progress;
                        GameManager.instance.progressTxt.text = Mathf.RoundToInt(GameManager.instance.progressSlider.value * 100f) + "%";
                    }
                    else if (GameManager.instance.tasksCompleted == 2)
                    {

                        float progress = 0.16f;
                        print("Level1 : Add score" + progress);
                        GameManager.instance.progressSlider.value += progress;
                        GameManager.instance.progressTxt.text = Mathf.RoundToInt(GameManager.instance.progressSlider.value * 100f) + "%";
                    }


                }
                if (SceneManager.GetActiveScene().name == "Level2")
                {

                    isToolGrabbed = true;
                    if (GameManager.instance.tasksCompleted == 0)
                    {

                        float progress = 0.16f;
                        print("Level1 : Add score" + progress);
                        GameManager.instance.progressSlider.value += progress;
                        GameManager.instance.progressTxt.text = Mathf.RoundToInt(GameManager.instance.progressSlider.value * 100f) + "%";
                    }
                    else if (GameManager.instance.tasksCompleted == 1)
                    {

                        float progress = 0.16f;
                        print("Level1 : Add score" + progress);
                        GameManager.instance.progressSlider.value += progress;
                        GameManager.instance.progressTxt.text = Mathf.RoundToInt(GameManager.instance.progressSlider.value * 100f) + "%";
                    }
                    else if (GameManager.instance.tasksCompleted == 2)
                    {

                        float progress = 0.16f;
                        print("Level1 : Add score" + progress);
                        GameManager.instance.progressSlider.value += progress;
                        GameManager.instance.progressTxt.text = Mathf.RoundToInt(GameManager.instance.progressSlider.value * 100f) + "%";
                    }

                }
                if (SceneManager.GetActiveScene().name == "Level3")
                {

                    isToolGrabbed = true;
                    float progress = 0.08f;
                    print("Level1 : Add score" + progress);
                    GameManager.instance.progressSlider.value += progress;
                    GameManager.instance.progressTxt.text = Mathf.RoundToInt(GameManager.instance.progressSlider.value * 100f) + "%";
                }
            }
        }
    }
    public void WrongToolInstruction()
    {
        GameManager.instance.npcManager.animator.SetTrigger("Talk");
        GameManager.instance.npcManager.audioSource.loop = false;
        foreach (var instruction in GameManager.instance.npcManager.instructionTexts)
        {
            instruction.gameObject.SetActive(false);
        }
        GameManager.instance.npcManager.instructionTexts[GameManager.instance.npcManager.audioClips.Length - 1].gameObject.SetActive(true);
        if (GameManager.instance.npcManager.audioSource)
        {
            GameManager.instance.npcManager.audioSource.clip = GameManager.instance.npcManager.audioClips[GameManager.instance.npcManager.audioClips.Length - 1];
            GameManager.instance.npcManager.audioSource.Play();
        }
        Invoke("AfterDelay", 5f);
    }
    public void AfterDelay()
    {
        GameManager.instance.npcManager.NextInstruction();
    }

}

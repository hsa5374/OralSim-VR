using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NPCManager : MonoBehaviour
{
    public GameObject npcCharacter = null;
    public Animator animator = null;
    public Canvas npcCanvas = null;

    [Space]
    public TMP_Text[] instructionTexts = null;
    public TMP_Text completionMsgText;

    [Space]
    public AudioSource audioSource = null;
    public AudioClip[] audioClips = null;
    public AudioClip completionAudioClip;
    public bool is3rdLevel;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        animator.SetTrigger("Talk");
        if (audioSource)
        {
            audioSource.clip = audioClips[0];
            audioSource.Play();
        }
        yield return new WaitForSeconds(15f);
        animator.SetTrigger("Talk");
        instructionTexts[0].gameObject.SetActive(false);
        instructionTexts[1].gameObject.SetActive(true);
        audioSource.loop = true;
        if (audioSource)
        {
            audioSource.clip = audioClips[1];
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void NextInstruction()
    {
        animator.SetTrigger("Talk");
        audioSource.loop = true;
        foreach (var instruction in instructionTexts)
        {
            instruction.gameObject.SetActive(false);
        }
        //instructionTexts[GameManager.instance.npcInstIndex - 1].gameObject.SetActive(false);
        instructionTexts[GameManager.instance.npcInstIndex].gameObject.SetActive(true);
        if (audioSource)
        {
            audioSource.clip = audioClips[GameManager.instance.npcInstIndex];
            audioSource.Play();
        }
    }
    public void AssestmentCompleteAudio()
    {
        audioSource.enabled = true ;
        animator.SetTrigger("Talk");
        audioSource.loop = false;
        foreach (var instruction in instructionTexts)
        {
            instruction.gameObject.SetActive(false);
        }
        //instructionTexts[GameManager.instance.npcInstIndex - 1].gameObject.SetActive(false);
        completionMsgText.gameObject.SetActive(true);
        if (audioSource)
        {
            audioSource.clip = completionAudioClip;
            audioSource.Play();
        }
    }

    public void StopNPCAudio()
    {
        if (audioSource)
        {
            audioSource.enabled = false;
        }
    }

}

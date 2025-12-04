using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class DentalExplorer : MonoBehaviour
{
    public float timer = 0;
    bool isExplored = false;
    public float correctHoverTime = 0f;
    public float totalHoverTime = 0f;
    private void OnTriggerStay(Collider other)
    {
        if (gameObject.transform.parent.GetComponent<Grabbable>() && GameManager.instance.isAssessment)
        {
            if (gameObject.transform.parent.GetComponent<Grabbable>().isGrabbed)
            {
                totalHoverTime += Time.deltaTime/3f;;
            }
        }
        if (other.CompareTag("Teeth"))
        {
            if (gameObject.transform.parent.GetComponent<ToolIndex>().toolIndex != 9 && GameManager.instance.isAssessment)
            {
                correctHoverTime += Time.deltaTime;
            }

            timer += Time.deltaTime;
            if (timer > 1f)
            {
                if (other.GetComponent<Level1Teeth>())
                {
                    if (!isExplored)
                    {
                        timer = 0;
                        isExplored = true;
                       // Debug.LogError(" ** EXPLORER ** ");
                        GameManager.instance.TaskCompleted();
                        if (GameManager.instance.isAssessment)
                        {
                            GameManager.instance.hoverPercentage = GetHoverAccuracy();
                        }
                    }
                }
            }
        }
    }
    public float GetHoverAccuracy()
    {
        if (totalHoverTime == 0) return 0;
       float hover= (correctHoverTime / totalHoverTime) * 100f;
        correctHoverTime = 0;
        totalHoverTime = 0;
        return (hover + GameManager.instance.hoverPercentage) / 2;
    }
}


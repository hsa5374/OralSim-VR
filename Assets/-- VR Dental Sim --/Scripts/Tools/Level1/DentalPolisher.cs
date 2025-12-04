using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class DentalPolisher : MonoBehaviour
{
    float timer = 0;
    public int totalTeeths = 0;
    int teethCount = 0;
    float correctHoverTime = 0f;
    float totalHoverTime = 0f;
    private void OnTriggerStay(Collider other)
    {
        if (gameObject.transform.parent.GetComponent<Grabbable>() && GameManager.instance.isAssessment)
        {
            if (gameObject.transform.parent.GetComponent<Grabbable>().isGrabbed)
            {
                totalHoverTime += Time.deltaTime/3f;
            }
        }

        if (other.CompareTag("Teeth"))
        {
            if (gameObject.transform.parent.GetComponent<ToolIndex>().toolIndex != 9 && GameManager.instance.isAssessment)
            {
                correctHoverTime += Time.deltaTime;
            }
            timer += Time.deltaTime;
            if (timer > 3f)
            {
                if(other.GetComponent<Level1Teeth>())
                {
                    if (GameManager.instance.tasksCompleted == 2)
                    {
                        if (!other.GetComponent<Level1Teeth>().isPolishingDone)
                        {
                            timer = 0;
                            other.GetComponent<Level1Teeth>().isPolishingDone = true;
                            other.GetComponent<Renderer>().material = other.GetComponent<Level1Teeth>().whiteTeeth;
                            teethCount++;
                            if (teethCount >= totalTeeths)
                            {
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
        }

    }
    public float GetHoverAccuracy()
    {
        if (totalHoverTime == 0) return 0;
        float hover = (correctHoverTime / totalHoverTime) * 100f;
        return (hover + GameManager.instance.hoverPercentage) / 2;
    }
}

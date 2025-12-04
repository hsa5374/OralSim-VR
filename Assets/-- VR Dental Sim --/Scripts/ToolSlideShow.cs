using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSlideShow : MonoBehaviour
{
    public GameObject[] objects; // Array of GameObjects
    private int currentIndex = 0;

    void Start()
    {
        StartCoroutine(ActivateObjectsSequentially());
    }

    IEnumerator ActivateObjectsSequentially()
    {
        while (true)
        {
            // Deactivate all objects
            foreach (GameObject obj in objects)
            {
                obj.SetActive(false);
            }

            // Activate the current object
            if (objects.Length > 0)
            {
                objects[currentIndex].SetActive(true);
            }

            // Wait for 5 seconds
            yield return new WaitForSeconds(8f);

            if (currentIndex == objects.Length - 1)
            {
                objects[currentIndex].SetActive(false);
                break;
            }
            // Move to the next object
            if (currentIndex < objects.Length - 1)
                currentIndex = (currentIndex + 1) % objects.Length;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHands : MonoBehaviour
{
    [SerializeField] InputActionProperty grip;
    [SerializeField] InputActionProperty trigger;

    [SerializeField] Animator animator;
    
    // Update is called once per frame
    void Update()
    {
        float gripValue = grip.action.ReadValue<float>();
        float triggerValue = trigger.action.ReadValue<float>();
        animator.SetFloat("Trigger", triggerValue);
        animator.SetFloat("Grip", gripValue);
    
    }
}

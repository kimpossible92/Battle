using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MMAnimator
{
    public static void AddAnimatorParamaterIfExists(Animator animator, string parameterName, AnimatorControllerParameterType type, List<string> parameterList)
    {
        if (animator.HasParameterOfType(parameterName, type))
        {
            parameterList.Add(parameterName);
        }
    }
    
    public static void UpdateAnimatorBool(Animator animator, string parameterName, bool value, List<string> parameterList)
    {
        if (parameterList.Contains(parameterName))
        {
            animator.SetBool(parameterName, value);
        }
    }

    public static void UpdateAnimatorTrigger(Animator animator, string parameterName, List<string> parameterList)
    {
        if (parameterList.Contains(parameterName))
        {
            animator.SetTrigger(parameterName);
        }
    }
    
    public static void SetAnimatorTrigger(Animator animator, string parameterName, List<string> parameterList)
    {
        if (parameterList.Contains(parameterName))
        {
            animator.SetTrigger(parameterName);
        }
    }
    
    public static void UpdateAnimatorFloat(Animator animator, string parameterName, float value, List<string> parameterList)
    {
        if (parameterList.Contains(parameterName))
        {
            animator.SetFloat(parameterName, value);
        }
    }
    
    public static void UpdateAnimatorInteger(Animator animator, string parameterName, int value, List<string> parameterList)
    {
        if (parameterList.Contains(parameterName))
        {
            animator.SetInteger(parameterName, value);
        }
    }
    
    public static void UpdateAnimatorBool(Animator animator, string parameterName, bool value)
    {
        animator.SetBool(parameterName, value);
    }
    
    public static void UpdateAnimatorTrigger(Animator animator, string parameterName)
    {
        animator.SetTrigger(parameterName);
    }
    
    public static void SetAnimatorTrigger(Animator animator, string parameterName)
    {
        animator.SetTrigger(parameterName);
    }
    
    public static void UpdateAnimatorFloat(Animator animator, string parameterName, float value)
    {
        animator.SetFloat(parameterName, value);
    }
    
    public static void UpdateAnimatorInteger(Animator animator, string parameterName, int value)
    {
        animator.SetInteger(parameterName, value);
    }
    
    public static void UpdateAnimatorBoolIfExists(Animator animator, string parameterName, bool value)
    {
        if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(parameterName, value);
        }
    }

    public static void UpdateAnimatorTriggerIfExists(Animator animator, string parameterName)
    {
        if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger(parameterName);
        }
    }
    
    public static void SetAnimatorTriggerIfExists(Animator animator, string parameterName)
    {
        if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger(parameterName);
        }
    }
    public static void UpdateAnimatorFloatIfExists(Animator animator, string parameterName, float value)
    {
        if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Float))
        {
            animator.SetFloat(parameterName, value);
        }
    }

    public static void UpdateAnimatorIntegerIfExists(Animator animator, string parameterName, int value)
    {
        if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Int))
        {
            animator.SetInteger(parameterName, value);
        }
    }
}
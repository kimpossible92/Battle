using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class PatientAnimationController : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation patientSpineSitting;
    [SerializeField] private SkeletonAnimation patientSpineAffraid;
    [SerializeField] private SkeletonAnimation patientSpineAttack;


    public void SetPatientSitting()
    {
        if (!patientSpineSitting.gameObject.activeInHierarchy)
            AcctivateSpine("Sitting");
        patientSpineSitting.AnimationState.SetAnimation(0, "Main", true);
    }

    public void SetPatientInteractAnimations()
    {
        if (!patientSpineAffraid.gameObject.activeInHierarchy)
            AcctivateSpine("Affraid");
        patientSpineAffraid.AnimationState.SetAnimation(0, "Idle simple + hands", true);
    }

    public void SetPatientGetHit()
    {
        if (!patientSpineAffraid.gameObject.activeInHierarchy)
            AcctivateSpine("Affraid");
        patientSpineAffraid.AnimationState.SetAnimation(0, "Get hit", false);
    }

    public void SetPatientAttack()
    {
        if (!patientSpineAttack.gameObject.activeInHierarchy)
            AcctivateSpine("Attack");
        patientSpineAttack.AnimationState.SetAnimation(0, "Attack", false);//1.4f
    }

    private void AcctivateSpine(string name)
    {
        patientSpineSitting.gameObject.SetActive(false);
        patientSpineAffraid.gameObject.SetActive(false);
        patientSpineAttack.gameObject.SetActive(false);

        if(name == "Sitting")
            patientSpineSitting.gameObject.SetActive(true);
        else if (name == "Affraid")
            patientSpineAffraid.gameObject.SetActive(true);
        else if (name == "Attack")
            patientSpineAttack.gameObject.SetActive(true);

    }
}

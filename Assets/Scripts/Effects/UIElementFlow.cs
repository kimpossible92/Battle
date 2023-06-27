using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIElementFlow : MonoBehaviour
{
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float flowMaxOffsetOnSpline = 200f;
    [SerializeField] private AnimationCurve elementScaleCurve;
    [SerializeField] private AnimationCurve elementFadeCurve;


    private Image image;
    private TMPro.TMP_Text tmpText;
    private RectTransform rectTransform;
    private UISpline pathSpline;


    public void FlowElement(UISpline pathSpline, string text=null)
    {
        image = GetComponent<Image>();
        tmpText = GetComponent<TMPro.TMP_Text>();
        if (tmpText != null)
            tmpText.text = text;

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        this.pathSpline = pathSpline;

        StartFlowElement(DestroyElement);
    }

    public void DestroyElement()
    {
        Destroy(gameObject);
    }

    private void StartFlowElement(UnityAction onDone)
    {
        flow = true;
        if (IFlowHelper == null)
            IFlowHelper = StartCoroutine(IFlow(onDone));
    }

    private void StopFlowElement()
    {
        flow = false;
        if (IFlowHelper != null)
            StopCoroutine(IFlowHelper);
    }

    #region Coroutines

    private bool flow = false;
    private Coroutine IFlowHelper;
    private IEnumerator IFlow(UnityAction onDone)
    {
        float t = 0f;
        SplinePoint sp;
        Vector3 p;
        Color color;
        while (flow)
        {
            sp = pathSpline.GetSplinePoint(t);
            p = sp.Position;
            p +=sp.Normal * ((Mathf.PerlinNoise(Time.time, 0.0f) - 0.5f) * flowMaxOffsetOnSpline);

            rectTransform.anchoredPosition = p;
           //Debug.Log(p);
            rectTransform.localScale = elementScaleCurve.Evaluate(t) * Vector3.one;

            if (image != null)
            {
                color = image.color;
                color.a = elementFadeCurve.Evaluate(t);
                image.color = color;
            }

            if (tmpText != null)
            {
                color = tmpText.color;
                color.a = elementFadeCurve.Evaluate(t);
                tmpText.color = color;
            }

            t += Time.fixedDeltaTime;
            if (t / animationDuration > 1f)
                flow = false;
            yield return new WaitForFixedUpdate();
        }

        onDone?.Invoke();
        IFlowHelper = null;
    }

    //private bool fade = false;
    //private Coroutine IFadeHelper;
    //private IEnumerator IFade(UnityAction onDone)
    //{
    //    float t = 0f;

    //    while (fade)
    //    {
    //        SplinePoint sp = pathSpline.GetSplinePoint(t);
    //        element.position = sp.Position;

    //        t += Time.fixedDeltaTime;
    //        if (t / animationDuration > 1f)
    //            flow = false;
    //        yield return new WaitForFixedUpdate();
    //    }

    //    onDone?.Invoke();
    //    IFlowHelper = null;
    //}

    #endregion
}

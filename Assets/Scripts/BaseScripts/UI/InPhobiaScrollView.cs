using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InPhobiaScrollView : MonoBehaviour
{
    #region Public Fields

    [Space(20f)]
    [Header("Scrollview Properties")]

    public RectTransform content;

    #endregion

    #region Serialized Fields

    [SerializeField] private RectTransform top;
    [SerializeField] private RectTransform bottom;
    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private float scrollBarElasticity = 10f;
    [SerializeField] private float contentMaxYPos;

    #endregion

    #region Private Fields

    private float contentTargetT;
    private Vector2 contentAnchordPosition;

    #endregion

    #region Public Methods

    public void InitializeScrollView()
    {
        scrollBar.value = 1f;
    }

    public void MoveScrollViewBy(float value)
    {
        float currentT = scrollBar.value;
        float currentHeightPos = Mathf.Lerp(0f, contentMaxYPos, currentT);
        float heightPos = Mathf.Clamp(currentHeightPos + value, 0f, contentMaxYPos);

        contentTargetT = Mathf.InverseLerp(0f, contentMaxYPos, heightPos);
        StartScrollContentByButton();
    }

    public void OnScrollViewDragBegin()
    {
        StartDragScrollView();
    }

    public void OnScrollViewDragEnd()
    {
        StopDragScrollView();
    }

    public void OnScrollBarDragBegin()
    {
        StartDragScrollBar();
    }

    public void OnScrollBarDragEnd()
    {
        StopDragScrollBar();
    }

    #endregion

    #region Private Methods

    private void StartScrollContentByButton()
    {
        scrollContentByButton = true;
        if (IScrollContentByButtonHelper == null)
            IScrollContentByButtonHelper = StartCoroutine(IScrollContentByButton());
    }
    private void StopScrollContentByButton()
    {
        scrollContentByButton = false;
        if (IScrollContentByButtonHelper != null)
            StopCoroutine(IScrollContentByButtonHelper);
        IScrollContentByButtonHelper = null;
    }

    private void StartDragScrollView()
    {
        StopScrollContentByButton();

        scrollViewDraging = true;
        if (IScrollViewDragHelper == null)
            IScrollViewDragHelper = StartCoroutine(IScrollViewDrag());
    }
    private void StopDragScrollView()
    {
        scrollViewDraging = false;
        if (IScrollViewDragHelper != null)
            StopCoroutine(IScrollViewDragHelper);
        IScrollViewDragHelper = null;
    }

    private void StartDragScrollBar()
    {
        StopScrollContentByButton();

        scrollBarDraging = true;
        if (IScrollBarDragHelper == null)
            IScrollBarDragHelper = StartCoroutine(IScrollBarDrag());
    }
    private void StopDragScrollBar()
    {
        scrollBarDraging = false;
        if (IScrollBarDragHelper != null)
            StopCoroutine(IScrollBarDragHelper);
        IScrollBarDragHelper = null;
    }

    #endregion

    #region Coroutines

    private bool scrollViewDraging = false;
    private Coroutine IScrollViewDragHelper;
    private IEnumerator IScrollViewDrag()
    {

        Vector2 mousePrevPose = Input.mousePosition;
        yield return new WaitForFixedUpdate();
        Vector2 mousePose;
        Vector2 deltaPos;
        while (scrollViewDraging)
        {
            mousePose = Input.mousePosition;
            deltaPos = mousePose - mousePrevPose;

            contentAnchordPosition = content.anchoredPosition;
            contentAnchordPosition.y += deltaPos.y;

            content.anchoredPosition = contentAnchordPosition;

            yield return new WaitForFixedUpdate();
            mousePrevPose = mousePose;
        }
        IScrollViewDragHelper = null;
    }

    private bool scrollBarDraging = false;
    private Coroutine IScrollBarDragHelper;
    private IEnumerator IScrollBarDrag()
    {
        float y, t;

        while (scrollBarDraging)
        {
            y = Input.mousePosition.y;
            y *= 1080f / Screen.height;
            //Debug.Log(y);
            t = Mathf.InverseLerp(bottom.anchoredPosition.y, top.anchoredPosition.y, y);

            scrollBar.value = Mathf.Lerp(scrollBar.value, t, Time.fixedDeltaTime * scrollBarElasticity);

            yield return new WaitForFixedUpdate();
        }
        IScrollBarDragHelper = null;
    }


    private bool scrollContentByButton = false;
    private Coroutine IScrollContentByButtonHelper;
    private IEnumerator IScrollContentByButton()
    {
        while (scrollContentByButton)
        {
            scrollBar.value = Mathf.Lerp(scrollBar.value, contentTargetT, Time.fixedDeltaTime * scrollBarElasticity);

            yield return new WaitForFixedUpdate();
        }
        IScrollContentByButtonHelper = null;
    }

    #endregion
}

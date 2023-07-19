using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
//    public bool NotStay = false;
    [SerializeField] bool _isPlayerUnit;
    [SerializeField] BattleHud _hud;
    public Vector2 oldPos;
    public bool IsplayerUnit { get { return _isPlayerUnit; } }

    public BattleHud Hud { get { return _hud; } }

    public Pokemon Pokemon { get; set; }
    Vector3 _orginalPos;
    Image _image;
    Color _orginalColor;
    private void Awake()
    {
        oldPos = GetComponent<Image>().rectTransform.anchoredPosition;
        _image = GetComponent<Image>();
        if (_image == null) return;
        _orginalPos = _image.transform.localPosition;
        _orginalColor = _image.color;

    }
    public bool AnimSp = false;
    public void AnimSprite()
    {
        if (AnimSp)
        {
            GetComponent<Image>().sprite = Pokemon.Base.GetSprites[2];
        }
    }
    public void AnimSprite2()
    {
        if (AnimSp)
        {
            GetComponent<Image>().sprite = Pokemon.Base.GetSprites[0];
        }
    }
    public GameObject GetImageUnit;
    public GameObject GetImage => GetImageUnit;
    // Start is called before the first frame update
    void Start()
    {
       
    }
    private void Update()
    {
        GetImageUnit  = GetComponentInChildren<Image>().gameObject;
       // if (NotStay) GetComponent<Image>().rectTransform.anchoredPosition = oldPos;
    }
    private void OnEnable()
    {
        AnimSp = true;
    }
    public void DeletImage()
    {
        //if (GetImageUnit != null) { GetImageUnit.gameObject.SetActive(false); GetImageUnit = null; }
    }
    public void Setup(Pokemon pokemon)
    {
        Pokemon = pokemon;
        
        if (_isPlayerUnit)
        {

            //_image.enabled = false;
            //GetImageUnit = Instantiate(Pokemon.Base._Image.gameObject, transform); GetImageUnit.gameObject.SetActive(true);
            GetComponent<Image>().sprite = Pokemon.Base._Image.GetComponent<Image>().sprite;
            GetComponent<Animator>().runtimeAnimatorController = Pokemon.Base._Image.GetComponent<Animator>().runtimeAnimatorController;

        }
        else
        {
            _image.sprite = Pokemon.Base.FrontSprite;
            GetComponent<Image>().sprite = Pokemon.Base._Image.GetComponent<Image>().sprite;
            GetComponent<Animator>().runtimeAnimatorController = Pokemon.Base._Image.GetComponent<Animator>().runtimeAnimatorController;
        }
        _hud.gameObject.SetActive(true);
        _hud.SetData(pokemon);

        if (_isPlayerUnit) { transform.localScale = new Vector3(1, 1, 1); }
        else { transform.localScale = new Vector3(-1, 1, 1); }
        _image.color = _orginalColor;
        PlayEnterAnimation();
    }
    public void Clear()
    {
        _hud.gameObject.SetActive(false);
    }
    public void PlayEnterAnimation()
    {
        if (_isPlayerUnit)
            _image.transform.localPosition = new Vector3(-1100f, _orginalPos.y);
        else
            _image.transform.localPosition = new Vector3(1100f, _orginalPos.y);
        _image.transform.DOLocalMoveX(_orginalPos.x, 2f);
    }
    public void PlayAttackAnimation()
    {
        //NotStay = true;
        var sequence = DOTween.Sequence();
        if (_isPlayerUnit)
            sequence.Append(_image.transform.DOLocalMoveX(_orginalPos.x + 50f, 0.25f));
        else
            sequence.Append(_image.transform.DOLocalMoveX(_orginalPos.x - 50f, 0.25f));
        sequence.Append(_image.transform.DOLocalMoveX(_orginalPos.x, 0.25f)); 
        //NotStay = false;
    }
    public void PlayHitAnimation()
    {
        //NotStay = true;
        var sequence = DOTween.Sequence();
        sequence.Append(_image.DOColor(Color.gray, 0.1f));
        sequence.Append(_image.DOColor(_orginalColor, 0.1f));
        //NotStay = false;
    }
    public void PlayFaintAnimation()
    {
        //NotStay = true;
        var sequence = DOTween.Sequence();
        sequence.Append(_image.transform.DOLocalMoveY(_orginalPos.y - 150f, 0.5f));
        sequence.Join(_image.DOFade(0f, 0.5f));
        //NotStay = false;
    }
    public IEnumerator PlayCaptureAnimation()
    {
        //NotStay = true;
        var sequence = DOTween.Sequence();
        sequence.Append(_image.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(_orginalPos.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion(); 
        //NotStay = false;
    }
    public IEnumerator PlayBreakOutAnimation()
    {
        //NotStay = true;
        var sequence = DOTween.Sequence();
        sequence.Append(_image.DOFade(1, 0.5f));
        sequence.Join(transform.DOLocalMoveY(_orginalPos.y, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
        //NotStay = false;
    }
}

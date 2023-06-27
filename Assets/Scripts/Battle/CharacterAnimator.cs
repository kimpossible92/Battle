using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> _walkDownSprite;
    [SerializeField] List<Sprite> _walkUpSprite;
    [SerializeField] List<Sprite> _walkRightSprite;
    [SerializeField] List<Sprite> _walkLeftSprite;
    //[SerializeField] FacingDirection _defaultDirection = FacingDirection.Down;
    //Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }


    //States
    //SpriteAnimator _walkDownAnim;
    //SpriteAnimator _walkUpAnim;
    //SpriteAnimator _walkRightAnim;
    //SpriteAnimator _walkLeftAnim;


    //SpriteAnimator _currentAnim;
    bool _wasPreviouslyMoving;
    //Referances
    SpriteRenderer _spriteRenderer;
}
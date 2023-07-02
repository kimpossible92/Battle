using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : CharAbil
{
    public bool SubjectToGravityPoints = true;
    public bool SubjectToGravityZones = true; protected bool _inAGravityZone = false;
    public bool ReverseHorizontalInputWhenUpsideDown = false;
    public bool ReverseVerticalInputWhenUpsideDown = false;
    public bool ReverseInputOnGravityPoints = false;
    public enum TransitionForcesModes { Reset, Adapt, Nothing }
    public TransitionForcesModes TransitionForcesMode = TransitionForcesModes.Reset;
    public bool ResetCharacterStateOnGravityChange = true;
    [Range(-180, 180)]
    public float InitialGravityAngle = 0f;
    protected float _defaultGravityAngle = 0f;
    protected float _currentGravityAngle;
    protected float _overrideGravityAngle = 0f;
    protected GravityPoint _closestGravityPoint = null;
    protected List<GravityPoint> _gravityPoints;
    protected float _entryTimeStampZones = 0f;
    protected float _entryTimeStampPoints = 0f;
    protected GravityPoint _lastGravityPoint = null;
    protected GravityPoint _newGravityPoint = null;
    public float RotationSpeed = 0f;
    public float InactiveBufferDuration = 0.1f;
    protected float _previousGravityAngle;
    public float GravityAngle { get { return _gravityOverridden ? _overrideGravityAngle : _defaultGravityAngle; } }
    protected bool _gravityOverridden = false;
    protected Vector2 _gravityPointDirection = Vector2.zero; protected Vector3 _newRotationAngle = Vector3.zero;
    protected const float _rotationSpeedMultiplier = 1000f;
    protected override void Awake()
    {
        Initialization();
    }
    protected override void Update()
    {
        if (GetComponent<InputManager>() != null)
        {
            if (
                Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(LoadTickCamera());
            }
            if (
                Input.GetKeyUp(KeyCode.A) ||
                Input.GetKeyUp(KeyCode.D) ||
                Input.GetKeyUp(KeyCode.Space))
            {
                StopCoroutine(LoadTickCamera());
            }
            if (
                Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.S)
                )
            {
                StartCoroutine(LoadTickCamera());
            }
            if (
              Input.GetKeyUp(KeyCode.W) ||
              Input.GetKeyUp(KeyCode.S)
              )
            {
                StopCoroutine(LoadTickCamera());
            }
        }
    }
    public void loadComputePoints()
    {
        //ComputeGravityPoints();
    }
    private IEnumerator LoadTickCamera()
    {
        while (true)
        {
            Camera.main.transform.position = new Vector3(transform.position.x,6, transform.position.z-2);
            yield return null;
        }
    }
    // Start is called before the first frame update    
    protected virtual void ComputeGravityPoints()
    {
        // if we're not affected by gravity points, we do nothing and exit
        if (!SubjectToGravityPoints) { return; }
        // if we're in a gravity zone, we do nothing and exit
        if (_inAGravityZone) { return; }

        // we grab the closest gravity point
        _closestGravityPoint = GetClosestGravityPoint();

        // if it's not null
        if (_closestGravityPoint != null)
        {
            // our new gravity point becomes the closest if we didn't have one already, otherwise we stay on the last gravity point met for now
            _newGravityPoint = (_lastGravityPoint == null) ? _closestGravityPoint : _lastGravityPoint;
            // if we've got a new gravity point
            if ((_lastGravityPoint != _closestGravityPoint) && (_lastGravityPoint != null))
            {
                // if we haven't entered a new gravity point in a while, we switch to that new gravity point
                if (Time.time - _entryTimeStampPoints >= InactiveBufferDuration)
                {
                    _entryTimeStampPoints = Time.time;
                    _newGravityPoint = _closestGravityPoint;
                    Transition(true, _newGravityPoint.transform.position - _controller.ColliderCenterPosition);
                    StartRotating();
                }
            }
            // if we didn't have a gravity point last time, we switch to this new one
            if (_lastGravityPoint == null)
            {
                if (Time.time - _entryTimeStampPoints >= InactiveBufferDuration)
                {
                    _entryTimeStampPoints = Time.time;
                    _newGravityPoint = _closestGravityPoint;
                    Transition(true, _newGravityPoint.transform.position - _controller.ColliderCenterPosition);
                    StartRotating();
                }
            }
            // we override our gravity
            _gravityPointDirection = _newGravityPoint.transform.position - _controller.ColliderCenterPosition;
            float gravityAngle = 180 - MMMaths.AngleBetween(Vector2.up, _gravityPointDirection);
            _gravityOverridden = true;
            _overrideGravityAngle = gravityAngle;
            _lastGravityPoint = _newGravityPoint;
        }
        else
        {
            // if we don't have a gravity point in range, our gravity is not overridden
            if (Time.time - _entryTimeStampPoints >= InactiveBufferDuration)
            {
                if (_lastGravityPoint != null)
                {
                    Transition(false, _newGravityPoint.transform.position - _controller.ColliderCenterPosition);
                    StartRotating();
                }
                _entryTimeStampPoints = Time.time;
                _gravityOverridden = false;
                _lastGravityPoint = null;
            }
        }
    }
    protected float _rotationDirection = 0f;
    protected virtual void StartRotating()
    {
        _rotationDirection = _character.IsFacingRight ? 1 : -1;
    }
    protected virtual void Transition(bool entering, Vector3 gravityDirection)
    {
        float gravityAngle = 180 - MMMaths.AngleBetween(Vector3.up, gravityDirection);
        if (TransitionForcesMode == TransitionForcesModes.Nothing)
        {
            return;
        }
        if (TransitionForcesMode == TransitionForcesModes.Reset)
        {
            _controller.SetForce(Vector3.zero);
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
        }
        if (TransitionForcesMode == TransitionForcesModes.Adapt)
        {
            // the angle is calculated depending on if the player enters or exits a zone and takes _previousGravityAngle as parameter if you glide over from one zone to another
            float rotationAngle = entering ? _previousGravityAngle - gravityAngle : gravityAngle - _defaultGravityAngle;

            _controller.SetForce(Quaternion.Euler(0, 0, rotationAngle) * _controller.Speed);
        }
        if (ResetCharacterStateOnGravityChange)
        {
            if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
            {
                //_character.gameObject.GetComponentNoAlloc<CharacterDash>().StopDash();
            }
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
        }
        _previousGravityAngle = entering ? gravityAngle : _defaultGravityAngle;
    }
    protected virtual void UpdateGravity()
    {
        if (RotationSpeed == 0)
        {
            _currentGravityAngle = GravityAngle;
        }
        else
        {
            float gravityAngle = GravityAngle;
            // if there's a 180° difference between both angles, we force the rotation angle depending on the direction of the character
            if (Mathf.DeltaAngle(_currentGravityAngle, gravityAngle) == 180)
            {

                _currentGravityAngle = _currentGravityAngle % 360;


                if (_rotationDirection > 0)
                {
                    _currentGravityAngle += 0.1f;
                }
                else
                {
                    _currentGravityAngle -= 0.1f;
                }
            }

            if (Mathf.DeltaAngle(_currentGravityAngle, gravityAngle) > 0)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(_currentGravityAngle, gravityAngle)) < Time.deltaTime * RotationSpeed * _rotationSpeedMultiplier)
                {
                    _currentGravityAngle = gravityAngle;
                }
                else
                {
                    _currentGravityAngle += Time.deltaTime * RotationSpeed * _rotationSpeedMultiplier;
                }
            }
            else
            {
                if (Mathf.Abs(Mathf.DeltaAngle(_currentGravityAngle, gravityAngle)) < Time.deltaTime * RotationSpeed * _rotationSpeedMultiplier)
                {
                    _currentGravityAngle = gravityAngle;
                }
                else
                {
                    _currentGravityAngle -= Time.deltaTime * RotationSpeed * _rotationSpeedMultiplier;
                }
            }

        }
        _newRotationAngle.z = _currentGravityAngle;
        if (GetComponent<InputManager>() != null) { transform.localEulerAngles = _newRotationAngle; }
    }
    public virtual void UpdateGravityPointsList()
    {
        if (_gravityPoints.Count != 0)
        {
            _gravityPoints.Clear();
        }

        _gravityPoints.AddRange(FindObjectsOfType(typeof(GravityPoint)) as GravityPoint[]);
    }
    protected override void Initialization()
    {
        base.Initialization();

        // we rotate our character based on our InitialGravityAngle
        _newRotationAngle.z = InitialGravityAngle;
        if (GetComponent<InputManager>() != null)
        { transform.localEulerAngles = _newRotationAngle; }

        // we override our default gravity angle with the initial one and cache it
        _defaultGravityAngle = InitialGravityAngle;
        _currentGravityAngle = _defaultGravityAngle;
        _previousGravityAngle = _defaultGravityAngle;

        _gravityPoints = new List<GravityPoint>();
        UpdateGravityPointsList();
    }
    protected virtual GravityPoint GetClosestGravityPoint()
    {
        if (_gravityPoints.Count == 0)
        {
            return null;
        }

        GravityPoint closestGravityPoint = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = _controller.ColliderCenterPosition;

        foreach (GravityPoint gravityPoint in _gravityPoints)
        {
            Vector3 directionToTarget = gravityPoint.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            // if we're outside of this point's zone of effect, we do nothing and exit
            if (directionToTarget.magnitude > gravityPoint.DistanceOfEffect)
            {
                continue;
            }

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestGravityPoint = gravityPoint;
            }
        }
        return closestGravityPoint;
    }
    public virtual bool ShouldReverseInput()
    {
        bool reverseInput = false;

        if (!ReverseInputOnGravityPoints && (_closestGravityPoint != null))
        {
            return false;
        }

        if (!ReverseHorizontalInputWhenUpsideDown)
        {
            return reverseInput;
        }

        reverseInput = ((GravityAngle < -90) || (GravityAngle > 90));

        return reverseInput;
    }

}

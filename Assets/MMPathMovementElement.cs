﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MMPathMovementElement
{
    public Vector3 PathElementPosition;
    public float Delay;
}
public class MMPath : MonoBehaviour
{
    public enum CycleOptions
    {
        BackAndForth,
        Loop,
        OnlyOnce
    }
    
    public enum MovementDirection
    {
        Ascending,
        Descending
    }

    [Header("Path")]
    public CycleOptions CycleOption;
    public MovementDirection LoopInitialMovementDirection = MovementDirection.Ascending;

    public List<MMPathMovementElement> PathElements;

    public float MinDistanceToGoal = .1f;

    protected Vector3 _originalTransformPosition;

    protected bool _originalTransformPositionStatus = false;

    public virtual bool CanMove { get; set; }

    protected bool _active = false;
    protected IEnumerator<Vector3> _currentPoint;
    protected int _direction = 1;
    protected Vector3 _initialPosition;
    protected Vector3 _initialPositionThisFrame;
    protected Vector3 _finalPosition;
    protected Vector3 _previousPoint = Vector3.zero;
    protected int _currentIndex;
    protected float _distanceToNextPoint;
    protected bool _endReached = false;


    protected virtual void Start()
    {
        Initialization();
    }


    protected virtual void Initialization()
    {
        _active = true;
        _endReached = false;
        CanMove = true;


        if (PathElements == null || PathElements.Count < 1)
        {
            return;
        }


        if (LoopInitialMovementDirection == MovementDirection.Ascending)
        {
            _direction = 1;
        }
        else
        {
            _direction = -1;
        }


        _initialPosition = this.transform.position;
        _currentPoint = GetPathEnumerator();
        _previousPoint = _currentPoint.Current;
        _currentPoint.MoveNext();


        if (!_originalTransformPositionStatus)
        {
            _originalTransformPositionStatus = true;
            _originalTransformPosition = transform.position;
        }
        transform.position = _originalTransformPosition + _currentPoint.Current;
    }

    public int CurrentIndex()
    {
        return _currentIndex;
    }

    public Vector3 CurrentPoint()
    {
        return _initialPosition + _currentPoint.Current;
    }

    public Vector3 CurrentPositionRelative()
    {
        return _currentPoint.Current;
    }
    
    protected virtual void Update()
    {
        if (PathElements == null
            || PathElements.Count < 1
            || _endReached
            || !CanMove
            )
        {
            return;
        }

        ComputePath();
    }
    
    protected virtual void ComputePath()
    {
        _initialPositionThisFrame = transform.position;
        
        _distanceToNextPoint = (transform.position - (_originalTransformPosition + _currentPoint.Current)).magnitude;
        if (_distanceToNextPoint < MinDistanceToGoal)
        {
            _previousPoint = _currentPoint.Current;
            _currentPoint.MoveNext();
        }	
        _finalPosition = transform.position;
    }
    
    public virtual IEnumerator<Vector3> GetPathEnumerator()
    {
        
        if (PathElements == null || PathElements.Count < 1)
        {
            yield break;
        }

        int index = 0;
        _currentIndex = index;
        while (true)
        {
            _currentIndex = index;
            yield return PathElements[index].PathElementPosition;

            if (PathElements.Count <= 1)
            {
                continue;
            }
            
            if (CycleOption == CycleOptions.Loop)
            {
                index = index + _direction;
                if (index < 0)
                {
                    index = PathElements.Count - 1;
                }
                else if (index > PathElements.Count - 1)
                {
                    index = 0;
                }
            }

            if (CycleOption == CycleOptions.BackAndForth)
            {
                if (index <= 0)
                {
                    _direction = 1;
                }
                else if (index >= PathElements.Count - 1)
                {
                    _direction = -1;
                }
                index = index + _direction;
            }

            if (CycleOption == CycleOptions.OnlyOnce)
            {
                if (index <= 0)
                {
                    _direction = 1;
                }
                else if (index >= PathElements.Count - 1)
                {
                    _direction = 0;
                    _endReached = true;
                }
                index = index + _direction;
            }
        }
    }
    
    public virtual void ChangeDirection()
    {
        _direction = -_direction;
        _currentPoint.MoveNext();
    }
    
    protected virtual void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (PathElements == null)
        {
            return;
        }

        if (PathElements.Count == 0)
        {
            return;
        }
        
        if (_originalTransformPositionStatus == false)
        {
            _originalTransformPosition = transform.position;
            _originalTransformPositionStatus = true;
        }
        if (transform.hasChanged && _active == false)
        {
            _originalTransformPosition = transform.position;
        }
        for (int i = 0; i < PathElements.Count; i++)
        {

            MMDebug.DrawGizmoPoint(_originalTransformPosition + PathElements[i].PathElementPosition, 0.2f, Color.green);


            if ((i + 1) < PathElements.Count)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(_originalTransformPosition + PathElements[i].PathElementPosition, _originalTransformPosition + PathElements[i + 1].PathElementPosition);
            }

            if ((i == PathElements.Count - 1) && (CycleOption == CycleOptions.Loop))
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(_originalTransformPosition + PathElements[0].PathElementPosition, _originalTransformPosition + PathElements[i].PathElementPosition);
            }
        }


        if (Application.isPlaying)
        {
            MMDebug.DrawGizmoPoint(_originalTransformPosition + _currentPoint.Current, 0.2f, Color.blue);
            MMDebug.DrawGizmoPoint(_originalTransformPosition + _previousPoint, 0.2f, Color.red);
        }
#endif


    }


    public virtual void UpdateOriginalTransformPosition(Vector3 newOriginalTransformPosition)
    {
        _originalTransformPosition = newOriginalTransformPosition;
    }

    
    public virtual Vector3 GetOriginalTransformPosition()
    {
        return _originalTransformPosition;
    }


    public virtual void SetOriginalTransformPositionStatus(bool status)
    {
        _originalTransformPositionStatus = status;
    }


    public virtual bool GetOriginalTransformPositionStatus()
    {
        return _originalTransformPositionStatus;
    }
}
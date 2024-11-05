//*************************************************************************************************************
/*  Camera pan
 *  A script to allow a destination to be set and the camera will pan to it over a specified number of seconds
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *
 */
//*************************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    private Transform _camera;
    
    private Vector3 _destination;
    private Vector3 _home;
    private Quaternion _destinationRotation;
    private Quaternion _homeRotaion;
    private float _timeElapsed;
    private float _timeToDestination;

    public delegate void TargetReached();
    public event TargetReached OnTargetReached;

    private void OnEnable()
    {
        _camera = GetComponent<Camera>().transform;
        _home = _camera.position;
        _homeRotaion = _camera.rotation;
    }

    private void FixedUpdate()
    {
        if (_timeElapsed >= _timeToDestination) return;
        
        _timeElapsed += Time.deltaTime;
        if (_timeElapsed >= _timeToDestination)
        {
            OnTargetReached?.Invoke();
        }
        
        _camera.position = Vector3.Lerp(_home, _destination, _timeElapsed/_timeToDestination);
        _camera.rotation = Quaternion.Lerp(_homeRotaion, _destinationRotation, _timeElapsed / _timeToDestination);
    }

    public void SetDestination(Vector3 destination, Quaternion destinationRotation, float secondsToDestination)
    {
        _timeToDestination = secondsToDestination;
        _timeElapsed = 0f;
        
        _home = _camera.position;
        _destination = destination;
        
        _homeRotaion = _camera.rotation;
        _destinationRotation = destinationRotation;
    }
}

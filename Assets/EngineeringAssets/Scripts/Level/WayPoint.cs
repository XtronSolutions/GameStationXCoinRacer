using System;
using System.Collections;
using System.Collections.Generic;
using DavidJalbert;
using UniRx;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    private Subject<WayPointData> _wayPointDataSubject = new Subject<WayPointData>();

    public IObservable<WayPointData> WayPointDataObservable => _wayPointDataSubject;
 
    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger) return;

        TinyCarController carController = other.GetComponent<TinyCarController>();
        if (carController != null)
        {
            _wayPointDataSubject.OnNext(new WayPointData(){ CarController = carController,Waypoint = this});
        }
    }
}

public class WayPointData
{
    public TinyCarController CarController;
    public WayPoint Waypoint;
}

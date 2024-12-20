/*
 * Copyright 2021 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

public class WorldManager : MonoBehaviour
{
    public GameObject WorldPrefab;
    
    public SurfaceManager SurfaceManager;
    
    private GameObject WorldInstance;
    
    private ARPlane CurrentPlane;

    private Camera arCamera;
    
    private void Start()
    {
        arCamera = Camera.main;
    }

    private void Update()
    {
        if (SurfaceManager.LockedPlane == null)
        {
            UpdateCurrentPlane();
            TryLockCurrentPlane();
        }

        if (SurfaceManager.LockedPlane != null && WorldInstance != null)
        {
            var worldOriginPosition = WorldInstance.transform.position;
            worldOriginPosition.Set(worldOriginPosition.x, SurfaceManager.LockedPlane.center.y, worldOriginPosition.z);
        }
    }
    
    private void UpdateCurrentPlane()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
         
        var hits = new List<ARRaycastHit>();
        SurfaceManager.RaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinBounds);
         
        CurrentPlane = null;
        ARRaycastHit? hit = null;
        if (hits.Count > 0)
        {
            var lockedPlane = SurfaceManager.LockedPlane;
            hit = lockedPlane == null
                ? hits[0]
                : hits.SingleOrDefault(x => x.trackableId == lockedPlane.trackableId);
        }
        if (hit.HasValue)
        {
            CurrentPlane = SurfaceManager.PlaneManager.GetPlane(hit.Value.trackableId);
        }
    }
    
    private void TryLockCurrentPlane()
    {
        if (
            WorldInstance == null
            && CurrentPlane != null
            && Touchscreen.current != null
            && Touchscreen.current.primaryTouch.press.isPressed
            )
        {
            var touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            var hits = new List<ARRaycastHit>();

            if (SurfaceManager.RaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinBounds))
            {
                foreach (var hit in hits)
                {
                    if (hit.trackableId == CurrentPlane.trackableId)
                    {
                        Vector3 hitPosition = hit.pose.position;

                        WorldInstance = Instantiate(WorldPrefab, hitPosition, Quaternion.identity);

                        SurfaceManager.LockPlane(CurrentPlane);
                        return;
                    }
                }
            }
        }
    }
}

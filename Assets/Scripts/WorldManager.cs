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

    private void Start()
    {
    
    }

    private void Update()
    {
        if (SurfaceManager.LockedPlane == null)
        {
            UpdateCurrentPlane();
            TryLockCurrentPlane();
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
            // If you don't have a locked plane already...
            var lockedPlane = SurfaceManager.LockedPlane;
            hit = lockedPlane == null
                // ... use the first hit in `hits`.
                ? hits[0]
                // Otherwise use the locked plane, if it's there.
                : hits.SingleOrDefault(x => x.trackableId == lockedPlane.trackableId);
        }
        if (hit.HasValue)
        {
            CurrentPlane = SurfaceManager.PlaneManager.GetPlane(hit.Value.trackableId);
            // Move this reticle to the location of the hit.
            // transform.position = hit.Value.pose.position;
        }

        /*if (WorldInstance != null)
        {
            WorldInstance.SetActive(CurrentPlane != null);
        }*/
    }
    
    private void TryLockCurrentPlane()
    {
        if (WorldInstance == null && WasTapped() && CurrentPlane != null)
        {
            // Spawn our car at the reticle location.
            WorldInstance = GameObject.Instantiate(WorldPrefab);
            
            // WorldInstance.transform.position = Vector3.zero(); // тут ворлд в 0 0 0 поместить
            
            SurfaceManager.LockPlane(CurrentPlane);
        }
    }

    private bool WasTapped()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            return true;
        }

        return false;
    }
}

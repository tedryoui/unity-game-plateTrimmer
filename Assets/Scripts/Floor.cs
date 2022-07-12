using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public Transform floorFill;
    public Transform floorTrash;
    public Transform floorBase;

    public Vector2 sizes => Level.level.preferences.floorSizes;
    [Range(1,4)]
    public int validationGridResolution = 1;
    public LayerMask plateLayerMask;
    public float destructionDelay;

    public int lastProgress;

    private void Start()
    {
        floorBase = Instantiate(Level.level.preferences.floorPrefab, transform).transform;
        floorBase.transform.localPosition = Vector3.zero;
    }

    public int GetFillPercentage()
    {
        float sizeX = sizes.x, sizeY = sizes.y;
        float step = 1f / (float)validationGridResolution;
        float offset = step / 2f;
        float startX = -sizeX / 2f + offset;
        float startY = -sizeY / 2f + offset;
        RaycastHit hit;
        int plateHits = 0;
        
        for(float x = startX; x < startX + sizeX; x+=step)
            for (float y = startY; y < startY + sizeY; y+=step)
                if (Physics.Raycast(
                        new Vector3(x, 6f, y), 
                        Vector3.down, 
                        out hit, 
                        7f,
                        plateLayerMask)) plateHits++;

        int percentage =
            (int) (100 * plateHits / ((sizeX * validationGridResolution) * (sizeY * validationGridResolution)));

        lastProgress = percentage;
        
        return percentage;
    }

    public void ClearTrash(GameObject trashObj) => StartCoroutine(DelayedDestruction(trashObj));
    
    IEnumerator DelayedDestruction(GameObject trashObj)
    {
        yield return new WaitForSeconds(destructionDelay);
        trashObj.GetComponent<MeshCollider>().isTrigger = true;
        yield return new WaitForSeconds(destructionDelay);
        Destroy(trashObj);
    }
    
    public void ClearFloor()
    {
        for(int i = 0; i < floorFill.childCount; i++)
            Destroy(floorFill.GetChild(i).gameObject);
    }
    
    /*
    private void OnDrawGizmosSelected()
    {
        float sizeX = sizes.x, sizeY = sizes.y;
        float step = 1f / (float)validationGridResolution;
        float offset = step / 2f;
        float startX = -sizeX / 2f + offset;
        float startY = -sizeY / 2f + offset;
        
        for(float x = startX; x < startX + sizeX; x+=step)
        {
            for (float y = startY; y < startY + sizeY; y+=step)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(new Vector3(x, 3f, y), 0.111f);
            }
        }
    }*/

    public void ResetFloor()
    {
        Destroy(floorBase.gameObject);
        ClearFloor();

        floorBase = Instantiate(Level.level.preferences.floorPrefab, transform).transform;
        floorBase.transform.localPosition = Vector3.zero;
    }
}

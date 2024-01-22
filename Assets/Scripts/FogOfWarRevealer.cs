using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarReaveler : MonoBehaviour
{
    public FogOfWar fogOfWar;
    public Transform secondaryFogOfWar;
    [Range(0, 5)]
    public float sightDistance;
    public float checkInterval;

    private void Start()
    {
        StartCoroutine(CheckFogOfWar(checkInterval));
        secondaryFogOfWar.localScale = new Vector2(sightDistance, sightDistance) * 10f;
    }

    private IEnumerator CheckFogOfWar(float checkInterval)
    {
        while (true)
        {
            fogOfWar.MakeHole(transform.position, sightDistance);
            yield return new WaitForSeconds(checkInterval);
        }
    }
}

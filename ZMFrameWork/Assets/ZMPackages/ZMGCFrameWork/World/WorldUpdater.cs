using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUpdater : MonoBehaviour
{
    private void Update()
    {
        if (WorldManager.Builder)
            WorldManager.Update();
    }
}

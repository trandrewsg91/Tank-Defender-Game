using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSpawnController : MonoBehaviour
{
    private TankController tankController = null;
    public TankController TankController
    {
        get { return tankController; }

        set { tankController = value; }
    }
}

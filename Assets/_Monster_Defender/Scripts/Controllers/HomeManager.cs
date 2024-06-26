using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    private void Start()
    {
        ViewManager.Instance.SetActiveView(ViewType.HomeView);
    }
}

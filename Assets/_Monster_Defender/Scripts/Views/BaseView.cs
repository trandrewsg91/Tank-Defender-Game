using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseView : MonoBehaviour
{
    [SerializeField] ViewType viewType = ViewType.HomeView;

    public ViewType ViewType { get { return viewType; } }

    public virtual void OnShow()
    {

    }

    public virtual void OnHide()
    {

    }
}

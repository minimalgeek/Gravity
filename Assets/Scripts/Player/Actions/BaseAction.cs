using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using System;
using DG.Tweening;

public enum KeyDirection
{
    KeyUp, KeyDown
}

public class BaseAction : GLMonoBehaviour, IAction
{

    public KeyDirection keyDirectionToTrigger = KeyDirection.KeyUp;

    protected bool executionEnabled = false;

    protected GameObject player;
    protected CombinedController characterController;

    protected void Start()
    {
        player = GameObject.FindGameObjectWithTag(TagsAndLayers.PLAYER);
        characterController = player.GetComponent<CombinedController>();
    }

    protected void Update()
    {
        if (!executionEnabled)
        {
            return;
        }

        if (keyDirectionToTrigger == KeyDirection.KeyUp)
        {
            if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
            {
                Execute();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                Execute();
            }
        }
     
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            PressExecute();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            ReleaseExecute();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == TagsAndLayers.PLAYER)
        {
            executionEnabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == TagsAndLayers.PLAYER)
        {
            executionEnabled = false;
            ReleaseExecute();
        }
    }

    public virtual void Execute()
    {
    }

    public virtual void PressExecute()
    {
    }

    public virtual void ReleaseExecute()
    {
    }
}

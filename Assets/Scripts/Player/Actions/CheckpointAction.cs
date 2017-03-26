using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using UnityEngine.SceneManagement;
using System;

public class CheckpointAction : ReachAction
{

    private static CheckpointAction lastCheckpoint;
    private const string LAST_CHECKPOINT = "LastCheckPoint";
    private bool isChecked;
    private string scope;
    private SpriteRenderer spriteRenderer;

    new void Start()
    {
        base.Start();
        scope = SceneManager.GetActiveScene().name;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        RespawnPlayerHere();
    }

    void Update()
    {
        if (lastCheckpoint == this) {
            VisualFeedbackForLatestCheckpoint();
        } else {
            ResetVisualFeedback();
        }
    }

    public override void Execute()
    {
        if (!isChecked)
        {
            isChecked = true;
            string[] checkPoints = GLPlayerPrefs.GetStringArray(scope, LAST_CHECKPOINT);
			var exists = Array.Exists(checkPoints, x => x == GetCode());
            if (!exists)
            {
                Array.Resize(ref checkPoints, checkPoints.Length + 1);
                checkPoints[checkPoints.Length - 1] = GetCode();
                GLPlayerPrefs.SetStringArray(scope, LAST_CHECKPOINT, checkPoints);
                Debug.Log("Checkpoint saved: " + GetCode());
                lastCheckpoint = this;
            }
        }
    }

    private string GetCode()
    {
        return GetInstanceID().ToString();
    }

    public void RespawnPlayerHere()
    {
        string[] checkPoints = GLPlayerPrefs.GetStringArray(scope, LAST_CHECKPOINT);
		if (checkPoints.Length == 0) {
			return;
		}

		string lastCode = checkPoints[checkPoints.Length - 1];
        if (lastCode == GetCode())
        {
            Debug.Log("Respawn player at: " + GetCode());
            player.transform.position = this.transform.position + player.transform.up * 0.5f;
            lastCheckpoint = this;
        }
    }

    private void VisualFeedbackForLatestCheckpoint() {
        spriteRenderer.color = Color.red;
    }

    private void ResetVisualFeedback() {
        spriteRenderer.color = Color.white;
    }
}

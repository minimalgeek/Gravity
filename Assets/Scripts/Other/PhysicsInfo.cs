using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsInfo : MonoBehaviour {

    public GameObject player;
    private Rigidbody2D playerRB;

    private RigidbodyFPS2D pc;

	// Use this for initialization
	void Start () {
        try {
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");
            playerRB = player.GetComponent<Rigidbody2D>();
            pc = player.GetComponent<RigidbodyFPS2D>();
        } catch { }
    }


    void OnGUI()
    {
        try
        {
            float R = player.transform.position.magnitude;
            float stationTangentialSpeed = RotationField.Instance.GetTangentialSpeed(R);
            float playerRelativeTangentialSpeed = Vector3.Dot(playerRB.velocity, playerRB.transform.right);

            GUI.Label(new Rect(5, 5, Screen.width - 5, Screen.height - 5), "r =       " + R.ToString("F2").PadLeft(7) + "\n" +
                "v_s =    " + stationTangentialSpeed.ToString("F2").PadLeft(7) + "\n" +
                "r. v_p = " + playerRelativeTangentialSpeed.ToString("F2").PadLeft(7) + "\n" +
                "a. v_p = " + (stationTangentialSpeed + playerRelativeTangentialSpeed).ToString("F2").PadLeft(7) + (pc != null ? ("\n" +
                "\n\n\n\n\n\n\n\n\n" +
                (pc.Grounded ? "G" : "\tA")) : ""));
        }
        catch { }
    }
}

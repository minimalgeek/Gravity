using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Assertions;
using Gamelogic.Extensions;

public class ItemPickupAction : MouseBaseAction
{

    public float throwSpeedMin = 5f;
    public float throwSpeedMax = 30f;
    public float maxReachTime = 2f;
    public float maxDistanceFromPlayer = 3f;

    [Tooltip("If the GameObject doesn't have a triggering collider" +
    "(but has one for the physics collision), it generates one, " +
    "with this radius increment.")]
    public float autoColliderSizeIncrement = 0.2f;
    private bool pickedUpByPlayer;
    private Transform holdingPoint;
    private Collider2D physicsCollider;
    private Collider2D pickupCollider;
    private Rigidbody2D rb;
    private FaceAxis toCenterRotator;
    private float throwSpeedMultiplier;

    new void Start()
    {
        base.Start();
        holdingPoint = characterController.ItemHoldingTransform;
        toCenterRotator = GetComponent<FaceAxis>();

        rb = GetComponent<Rigidbody2D>();
        physicsCollider = Array.Find(GetComponents<Collider2D>(), x => x.isTrigger == false);

        Assert.IsNotNull(rb);
        Assert.IsNotNull(physicsCollider);

        pickupCollider = Array.Find(GetComponents<Collider2D>(), x => x.isTrigger == true);
        if (!pickupCollider)
        {
            pickupCollider = this.gameObject.AddComponent<CircleCollider2D>();
            ((CircleCollider2D)pickupCollider).radius *= Mathf.Sqrt(2f);// * (1f + autoColliderSizeIncrement);
            ((CircleCollider2D)pickupCollider).radius += autoColliderSizeIncrement;
            pickupCollider.offset = physicsCollider.offset;
            pickupCollider.isTrigger = true;
        }
    }

    public override void PressExecute()
    {
        if (!pickedUpByPlayer)
        {
			SetPickedUp();
			executionEnabled = false;
            this.transform.SetParent(holdingPoint);
            this.transform.DOLocalMove(Vector2.zero, 1f).OnComplete(() => executionEnabled = true);
        }
        else
        {
            StartCoroutine(ThrowSpeedMultiplierCalculation());
            StartCoroutine(VisualFeedback());
        }
    }

    public override void ReleaseExecute()
    {
        if (pickedUpByPlayer)
        {
			SetPickedUp();
            this.transform.SetParent(null);
            rb.velocity = GetThrowSpeedAsVector() + characterController.velocity;
            StopAllCoroutines();
            throwSpeedMultiplier = 0;
            characterController.throwCross.gameObject.SetActive(false);
        }
    }

    private IEnumerator ThrowSpeedMultiplierCalculation()
    {
        while (true)
        {
            throwSpeedMultiplier += (Time.deltaTime / maxReachTime);
            throwSpeedMultiplier = Mathf.Clamp01(throwSpeedMultiplier);
            yield return null;
        }
    }

    private IEnumerator VisualFeedback()
    {
        characterController.throwCross.gameObject.SetActive(true);
		LineRenderer renderer = characterController.throwCross.GetComponentInChildren<LineRenderer>();
        while (true)
        {
            Vector2 shiftingVector = GetThrowSpeedAsVector();
            float shiftingMagnitude = shiftingVector.magnitude;
            shiftingVector = shiftingVector * (shiftingMagnitude/throwSpeedMax) * (maxDistanceFromPlayer/throwSpeedMax);

            characterController.throwCross.position = player.transform.position.To2DXY() + shiftingVector;
			renderer.SetPosition(0, characterController.transform.position);
			renderer.SetPosition(1, characterController.throwCross.position);
            yield return null;
        }
    }

    private void SetPickedUp()
    {
		pickedUpByPlayer = !pickedUpByPlayer;

        physicsCollider.isTrigger = pickedUpByPlayer;
        rb.simulated = !pickedUpByPlayer;
        if (toCenterRotator)
            toCenterRotator.enabled = !pickedUpByPlayer;
    }

    private Vector2 GetThrowSpeedAsVector()
    {
        float throwSpeed = FloatLerp(throwSpeedMin, throwSpeedMax, throwSpeedMultiplier);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionVector = mousePos.To2DXY() - player.transform.position.To2DXY();
        directionVector.Normalize();
        Vector3 throwVector = directionVector * throwSpeed;

        return throwVector;
    }

    private float FloatLerp(float from, float to, float multiplier)
    {
        float diff = to - from;
        return from + diff * multiplier;
    }

}

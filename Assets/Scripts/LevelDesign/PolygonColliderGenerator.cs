using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PolygonColliderGenerator : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private Sprite previousSprite;

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer.sprite != previousSprite)
            {
                DestroyImmediate(GetComponent<PolygonCollider2D>());
                gameObject.AddComponent<PolygonCollider2D>();
            }

            previousSprite = spriteRenderer.sprite;
        }
#endif
    }
}

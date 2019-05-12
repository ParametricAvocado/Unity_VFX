using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteFlip : MonoBehaviour {

    ///The property "_Flip" is already in use by default sprite shaders(although it doesn't seem to be used nowadays), 
    ///which would lead to double-flipping vertices by doing it once during batching
    ///and again during the vertex function of the default shader.
    [SerializeField]
    string m_FlipProperty = "_SpriteFlip";

    SpriteRenderer spriteRenderer;
    private int flipPropertyID;
    MaterialPropertyBlock propertyBlock;
    bool isDirty;


    bool flipX = false;
    bool flipY = false;

    public bool FlipX
    {
        get
        {
            return flipX;
        }

        set
        {
            if (value == flipX) return;
            flipX = value;
            isDirty = true;
        }
    }

    public bool FlipY
    {
        get
        {
            return flipY;
        }

        set
        {
            if (value == flipY) return;
            flipY = value;
            isDirty = true;
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        flipPropertyID = Shader.PropertyToID(m_FlipProperty);
        propertyBlock = new MaterialPropertyBlock();
        isDirty = true;
    }

    void LateUpdate () {
        if (!spriteRenderer) return;

        FlipX = spriteRenderer.flipX;
        FlipY = spriteRenderer.flipY;

        if(isDirty)
        {
            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetVector(flipPropertyID, new Vector4(FlipX ? -1 : 1, FlipY ? -1 : 1));
            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
	}
}

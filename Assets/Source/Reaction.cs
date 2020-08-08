using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    public class Reaction : MonoBehaviour
    {
        [SerializeField] private List<Sprite> sprites;

        private SpriteRenderer spriteRenderer;

        public void Initialize()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void SetReaction(float reaction)
        {
            reaction = Mathf.Clamp01(reaction);
            spriteRenderer.sprite = sprites[Mathf.RoundToInt((1 - reaction) * (sprites.Count - 1))];
        }
    }
}
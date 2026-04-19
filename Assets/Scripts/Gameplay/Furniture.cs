using UnityEngine;

public class Furniture : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _brokenDropDistance = 0.75f;
    [SerializeField] private float _brokenFallSpeed = 6f;

    private FurnitureDefinition _currentDefinition;

    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public Sprite BrokenSprite => _currentDefinition.BrokenSprite;
    public float BrokenTargetY => _spriteRenderer.transform.position.y - _brokenDropDistance;
    public float BrokenFallSpeed => _brokenFallSpeed;

    public void ResetFurniture(FurnitureDefinition definition)
    {
        _currentDefinition = definition;

        if (_spriteRenderer != null && _currentDefinition != null)
        {
            _spriteRenderer.sprite = _currentDefinition.DefaultSprite;
            _spriteRenderer.transform.localScale = _currentDefinition.Scale;
            _spriteRenderer.gameObject.SetActive(true);
        }
    }

    public void ShowTilt()
    {
        if (_spriteRenderer != null && _currentDefinition != null)
        {
            _spriteRenderer.sprite = _currentDefinition.TiltSprite;
        }
    }

    public void HideFurniture()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.gameObject.SetActive(false);
        }
    }
}

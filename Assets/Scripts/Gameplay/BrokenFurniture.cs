using UnityEngine;

public class BrokenFurniture : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private float _floorY;
    private float _fallSpeed;
    private bool _isFalling;

    public void Initialize(Sprite brokenSprite, SpriteRenderer sourceRenderer, float floorY, float fallSpeed)
    {
        transform.position = sourceRenderer.transform.position;
        transform.rotation = sourceRenderer.transform.rotation;
        transform.localScale = sourceRenderer.transform.lossyScale;

        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = brokenSprite;
            _spriteRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
            _spriteRenderer.sortingOrder = sourceRenderer.sortingOrder;
            _spriteRenderer.flipX = sourceRenderer.flipX;
            _spriteRenderer.flipY = sourceRenderer.flipY;
            _spriteRenderer.color = sourceRenderer.color;
        }

        _floorY = floorY;
        _fallSpeed = fallSpeed;
        _isFalling = true;
    }

    private void Update()
    {
        if (!_isFalling)
        {
            return;
        }

        var position = transform.position;
        position.y -= _fallSpeed * Time.deltaTime;

        if (position.y <= _floorY)
        {
            position.y = _floorY;
            _isFalling = false;
        }

        transform.position = position;
    }
}

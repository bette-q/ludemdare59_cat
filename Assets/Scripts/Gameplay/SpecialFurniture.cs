using System.Collections;
using UnityEngine;

public class SpecialFurniture : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private FurnitureDefinition _definition;

    private Coroutine _restoreCoroutine;

    private void OnEnable()
    {
        ApplyDefaultSprite();
    }

    private void OnDisable()
    {
        StopRestoreCoroutine();
    }

    public void ShowFail(float failDuration)
    {
        StopRestoreCoroutine();

        if (_spriteRenderer != null && _definition != null)
        {
            _spriteRenderer.sprite = _definition.BrokenSprite;
        }

        _restoreCoroutine = StartCoroutine(RestoreAfter(failDuration));
    }

    private IEnumerator RestoreAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        _restoreCoroutine = null;
        ApplyDefaultSprite();
    }

    private void ApplyDefaultSprite()
    {
        if (_spriteRenderer != null && _definition != null)
        {
            _spriteRenderer.sprite = _definition.DefaultSprite;
            _spriteRenderer.gameObject.SetActive(true);
        }
    }

    private void StopRestoreCoroutine()
    {
        if (_restoreCoroutine == null)
        {
            return;
        }

        StopCoroutine(_restoreCoroutine);
        _restoreCoroutine = null;
    }
}

using System.Collections;
using UnityEngine;

public class SpecialFurniture : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private FurnitureDefinition _definition;

    private Coroutine _restoreCoroutine;

    private void OnEnable()
    {
        GameEvents.Instance.OnSpecialCatResolved += OnSpecialCatResolved;
        ApplyDefaultSprite();
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnSpecialCatResolved -= OnSpecialCatResolved;
        StopRestoreCoroutine();
    }

    private void OnSpecialCatResolved(bool isSuccess, float failDuration)
    {
        if (isSuccess)
        {
            return;
        }

        ShowFailSprite(failDuration);
    }

    private void ShowFailSprite(float failDuration)
    {
        StopRestoreCoroutine();

        if (_spriteRenderer != null && _definition != null)
        {
            _spriteRenderer.sprite = _definition.TiltSprite;
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

using System.Collections;
using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _catSpriteRenderer;
    [SerializeField] private SpriteRenderer _bubbleSpriteRenderer;
    [SerializeField] private float _validationSpriteDuration = 0.4f;
    [SerializeField] private float _runningSpriteDuration = 0.4f;
    [SerializeField] private float _interactionRadius = 2.5f;

    private CatDefinition _currentDefinition;
    private CatRequestDefinition _currentRequest;
    private Furniture _currentFurniture;
    private Coroutine _hideCoroutine;
    private Coroutine _resolveCoroutine;
    private bool _acceptsItemDrop;
    public bool CanReceiveItem => _acceptsItemDrop;

    private void OnDisable()
    {
        StopHideCoroutine();
        StopResolveCoroutine();
        _currentDefinition = null;
        _currentRequest = null;
        _currentFurniture = null;
        _acceptsItemDrop = false;
    }

    public bool ContainsWorldPoint(Vector2 worldPoint)
    {
        return Vector2.Distance(worldPoint, transform.position) <= _interactionRadius;
    }

    public void ReceiveItem(E_CatItem catItem)
    {
        if (!_acceptsItemDrop)
        {
            return;
        }

        var isValid = _currentRequest != null && catItem == _currentRequest.RequiredItem;
        Resolve(isValid);
    }

    public void Show(CatDefinition definition, CatRequestDefinition request, Furniture furniture, float duration)
    {
        _currentDefinition = definition;
        _currentRequest = request;
        _currentFurniture = furniture;
        _acceptsItemDrop = true;
        StopResolveCoroutine();
        ApplySprites(definition, request);
        AudioManager.Instance.PlayCatLoad();
        AudioManager.Instance.PlayCatRequest(request.RequestSound);
        StopHideCoroutine();
        _hideCoroutine = StartCoroutine(HideAfter(duration));
    }

    private void ApplySprites(CatDefinition definition, CatRequestDefinition request)
    {
        if (_catSpriteRenderer != null)
        {
            _catSpriteRenderer.sprite = definition.DefaultSprite;
        }

        if (_bubbleSpriteRenderer != null)
        {
            _bubbleSpriteRenderer.sprite = request.BubbleSprite;
            _bubbleSpriteRenderer.gameObject.SetActive(true);
        }
    }

    private IEnumerator HideAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        _hideCoroutine = null;
        Resolve(false);
    }

    private void Resolve(bool isSuccess)
    {
        if (!_acceptsItemDrop)
        {
            return;
        }

        _acceptsItemDrop = false;
        StopHideCoroutine();
        Debug.Log($"Cat Resolve: target={gameObject.name}, success={isSuccess}");

        var view = GameManager.Instance.GetView<MainView>();
        if (isSuccess)
        {
            view.UpdateSuccess();
        }
        else
        {
            view.UpdateFail();
        }

        if (_bubbleSpriteRenderer != null)
        {
            _bubbleSpriteRenderer.gameObject.SetActive(false);
        }

        _resolveCoroutine = StartCoroutine(PlayResolveFlow(isSuccess));
    }

    private IEnumerator PlayResolveFlow(bool isSuccess)
    {
        if (_catSpriteRenderer != null)
        {
            _catSpriteRenderer.sprite = isSuccess ? _currentDefinition?.GoodSprite : _currentDefinition?.EvilSprite;
        }

        if (isSuccess)
        {
            AudioManager.Instance.PlayCorrectMatch();
        }

        if (!isSuccess)
        {
            _currentFurniture?.ShowTilt();
        }

        yield return new WaitForSeconds(_validationSpriteDuration);

        if (_catSpriteRenderer != null)
        {
            _catSpriteRenderer.sprite = _currentDefinition?.RunningSprite;
        }

        if (!isSuccess)
        {
            AudioManager.Instance.PlayGlassBreak();
            CatManager.Instance.SpawnBrokenFurniture(_currentFurniture);
            _currentFurniture.HideFurniture();
        }

        yield return new WaitForSeconds(_runningSpriteDuration);

        _resolveCoroutine = null;
        transform.parent.gameObject.SetActive(false);
    }

    private void StopHideCoroutine()
    {
        if (_hideCoroutine == null)
        {
            return;
        }

        StopCoroutine(_hideCoroutine);
        _hideCoroutine = null;
    }

    private void StopResolveCoroutine()
    {
        if (_resolveCoroutine == null)
        {
            return;
        }

        StopCoroutine(_resolveCoroutine);
        _resolveCoroutine = null;
    }

}

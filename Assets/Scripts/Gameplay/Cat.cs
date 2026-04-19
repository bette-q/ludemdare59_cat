using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cat : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _catSpriteRenderer;
    [SerializeField] private SpriteRenderer _bubbleSpriteRenderer;
    [SerializeField] private float _validationSpriteDuration = 0.4f;
    [SerializeField] private float _runningSpriteDuration = 0.4f;

    private CatDefinition _currentDefinition;
    private CatRequestDefinition _currentRequest;
    private Furniture _currentFurniture;
    private Coroutine _hideCoroutine;
    private Coroutine _resolveCoroutine;
    private bool _isResolved;

    private void OnEnable()
    {
        var listener = EventTriggerListener.Get(gameObject);
        listener.OnDropEvent -= OnDrop;
        listener.OnDropEvent += OnDrop;
    }

    private void OnDisable()
    {
        var listener = EventTriggerListener.Get(gameObject);
        listener.OnDropEvent -= OnDrop;
        StopHideCoroutine();
        StopResolveCoroutine();
        _currentDefinition = null;
        _currentRequest = null;
        _currentFurniture = null;
        _isResolved = false;
    }

    private void OnDrop(GameObject go, PointerEventData ev)
    {
        Debug.Log($"Cat OnDrop entered: target={go.name}, position={ev.position}, isResolved={_isResolved}");
        var view = GameManager.Instance.GetView<MainView>();
        if (view == null)
        {
            Debug.Log("Cat OnDrop ignored: MainView is null");
            return;
        }

        if (_isResolved)
        {
            Debug.Log("Cat OnDrop ignored: cat is already resolved");
            return;
        }

        if (!view.TryConsumeDrag(out var catItem))
        {
            Debug.Log("Cat OnDrop ignored: no active drag item to consume");
            return;
        }

        var isValid = _currentRequest != null && catItem == _currentRequest.RequiredItem;
        Debug.Log($"Cat received item: {catItem}, required item: {_currentRequest?.RequiredItem.ToString() ?? "None"}, valid: {isValid}");
        Resolve(isValid);
    }

    public void Show(CatDefinition definition, CatRequestDefinition request, Furniture furniture, float duration)
    {
        _currentDefinition = definition;
        _currentRequest = request;
        _currentFurniture = furniture;
        _isResolved = false;
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
        if (_isResolved)
        {
            return;
        }

        _isResolved = true;
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

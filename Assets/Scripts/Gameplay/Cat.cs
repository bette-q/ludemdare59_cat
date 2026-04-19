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
    private Coroutine _bubbleCoroutine;
    private bool _acceptsItemDrop;

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
        StopBubbleCoroutine();
        _currentDefinition = null;
        _currentRequest = null;
        _currentFurniture = null;
        _acceptsItemDrop = false;
    }

    private void OnDrop(GameObject go, PointerEventData ev)
    {
        var view = GameManager.Instance.GetView<MainView>();
        if (view == null)
        {
            return;
        }

        if (!_acceptsItemDrop)
        {
            return;
        }

        if (!view.TryConsumeDrag(out var catItem))
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
        StopBubbleCoroutine();
        _bubbleCoroutine = StartCoroutine(HideBubbleAfterRequest(AudioManager.Instance.PlayCatRequest(request.RequestSound)));
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

    private IEnumerator HideBubbleAfterRequest(float duration)
    {
        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
        }

        _bubbleCoroutine = null;
        if (_acceptsItemDrop && _bubbleSpriteRenderer != null)
        {
            _bubbleSpriteRenderer.gameObject.SetActive(false);
        }
    }

    private void Resolve(bool isSuccess)
    {
        if (!_acceptsItemDrop)
        {
            return;
        }

        _acceptsItemDrop = false;
        StopHideCoroutine();
        StopBubbleCoroutine();
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

    private void StopBubbleCoroutine()
    {
        if (_bubbleCoroutine == null)
        {
            return;
        }

        StopCoroutine(_bubbleCoroutine);
        _bubbleCoroutine = null;
    }
}

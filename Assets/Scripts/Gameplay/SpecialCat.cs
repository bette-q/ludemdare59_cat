using System.Collections;
using UnityEngine;

public class SpecialCat : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _catSpriteRenderer;
    [SerializeField] private SpriteRenderer _leftBubbleSpriteRenderer;
    [SerializeField] private SpriteRenderer _rightBubbleSpriteRenderer;
    [SerializeField] private float _validationSpriteDuration = 0.4f;
    [SerializeField] private float _runningSpriteDuration = 0.4f;
    [SerializeField] private float _interactionRadius = 5f;

    private SpecialCatDefinition _currentDefinition;
    private CatRequestDefinition _leftRequest;
    private CatRequestDefinition _rightRequest;
    private Coroutine _hideCoroutine;
    private Coroutine _resolveCoroutine;
    private Coroutine _requestAudioCoroutine;
    private bool _acceptsItemDrop;
    private bool _leftResolved;
    private bool _rightResolved;
    private float _specialFurnitureFailDuration;
    private SpecialFurniture _pairedSpecialFurniture;
    public bool CanReceiveItem => _acceptsItemDrop;

    private void OnDisable()
    {
        StopHideCoroutine();
        StopResolveCoroutine();
        StopRequestAudioCoroutine();
        _currentDefinition = null;
        _leftRequest = null;
        _rightRequest = null;
        _acceptsItemDrop = false;
        _leftResolved = false;
        _rightResolved = false;
        _specialFurnitureFailDuration = 0f;
        _pairedSpecialFurniture = null;
    }

    public void Show(
        SpecialCatDefinition definition,
        float duration,
        float specialFurnitureFailDuration,
        SpecialFurniture pairedSpecialFurniture)
    {
        _currentDefinition = definition;
        _leftRequest = definition.LeftRequest;
        _rightRequest = definition.RightRequest;
        _acceptsItemDrop = true;
        _leftResolved = false;
        _rightResolved = false;
        _specialFurnitureFailDuration = specialFurnitureFailDuration;
        _pairedSpecialFurniture = pairedSpecialFurniture;

        StopResolveCoroutine();
        StopRequestAudioCoroutine();
        ApplySprites(definition);
        AudioManager.Instance.PlayCatLoad();
        _requestAudioCoroutine = StartCoroutine(PlayRequestAudioSequence());

        StopHideCoroutine();
        _hideCoroutine = StartCoroutine(HideAfter(duration));
    }

    private void ApplySprites(SpecialCatDefinition definition)
    {
        if (_catSpriteRenderer != null)
        {
            _catSpriteRenderer.sprite = definition.DefaultSprite;
        }

        ApplyBubble(_leftBubbleSpriteRenderer, _leftRequest);
        ApplyBubble(_rightBubbleSpriteRenderer, _rightRequest);
    }

    private static void ApplyBubble(SpriteRenderer bubbleRenderer, CatRequestDefinition request)
    {
        if (bubbleRenderer == null)
        {
            return;
        }

        bubbleRenderer.sprite = request != null ? request.BubbleSprite : null;
        bubbleRenderer.gameObject.SetActive(request != null);
    }

    private IEnumerator PlayRequestAudioSequence()
    {
        yield return PlayRequestAudio(_leftRequest);
        yield return PlayRequestAudio(_rightRequest);
        _requestAudioCoroutine = null;
    }

    private static IEnumerator PlayRequestAudio(CatRequestDefinition request)
    {
        if (request == null)
        {
            yield break;
        }

        var duration = AudioManager.Instance.PlayCatRequest(request.RequestSound);
        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
        }
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

        if (TryResolveSingleRequest(catItem))
        {
            if (_leftResolved && _rightResolved)
            {
                Resolve(true);
            }

            return;
        }

        Resolve(false);
    }

    private bool TryResolveSingleRequest(E_CatItem catItem)
    {
        if (!_leftResolved && _leftRequest != null && catItem == _leftRequest.RequiredItem)
        {
            _leftResolved = true;
            HideBubble(_leftBubbleSpriteRenderer);
            return true;
        }

        if (!_rightResolved && _rightRequest != null && catItem == _rightRequest.RequiredItem)
        {
            _rightResolved = true;
            HideBubble(_rightBubbleSpriteRenderer);
            return true;
        }

        return false;
    }

    private static void HideBubble(SpriteRenderer bubbleRenderer)
    {
        if (bubbleRenderer != null)
        {
            bubbleRenderer.gameObject.SetActive(false);
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
        StopRequestAudioCoroutine();
        HideBubble(_leftBubbleSpriteRenderer);
        HideBubble(_rightBubbleSpriteRenderer);

        var view = GameManager.Instance.GetView<MainView>();
        if (view != null)
        {
            if (isSuccess)
            {
                view.UpdateSuccess();
            }
            else
            {
                view.UpdateFail();
            }
        }

        _resolveCoroutine = StartCoroutine(PlayResolveFlow(isSuccess));
        if (!isSuccess && _pairedSpecialFurniture != null)
        {
            _pairedSpecialFurniture.ShowFail(_specialFurnitureFailDuration);
        }
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

        yield return new WaitForSeconds(_validationSpriteDuration);

        if (_catSpriteRenderer != null)
        {
            _catSpriteRenderer.sprite = _currentDefinition?.RunningSprite;
        }

        yield return new WaitForSeconds(_runningSpriteDuration);

        _resolveCoroutine = null;
        var specialCatGroup = transform.parent.gameObject;
        CatManager.Instance.NotifySpecialCatFinished(this);
        specialCatGroup.SetActive(false);
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

    private void StopRequestAudioCoroutine()
    {
        if (_requestAudioCoroutine == null)
        {
            return;
        }

        StopCoroutine(_requestAudioCoroutine);
        _requestAudioCoroutine = null;
    }
}

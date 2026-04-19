using UnityEngine;

[CreateAssetMenu(menuName = "Special Cat Definition")]
public class SpecialCatDefinition : ScriptableObject
{
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _goodSprite;
    [SerializeField] private Sprite _evilSprite;
    [SerializeField] private Sprite _runningSprite;
    [SerializeField] private CatRequestDefinition _leftRequest;
    [SerializeField] private CatRequestDefinition _rightRequest;

    public Sprite DefaultSprite => _defaultSprite;
    public Sprite GoodSprite => _goodSprite;
    public Sprite EvilSprite => _evilSprite;
    public Sprite RunningSprite => _runningSprite;
    public CatRequestDefinition LeftRequest => _leftRequest;
    public CatRequestDefinition RightRequest => _rightRequest;
}

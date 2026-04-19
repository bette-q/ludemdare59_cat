using UnityEngine;

[CreateAssetMenu(menuName = "Cat Definition")]
public class CatDefinition : ScriptableObject
{
    [SerializeField] private E_CatType _catType;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _goodSprite;
    [SerializeField] private Sprite _evilSprite;
    [SerializeField] private Sprite _runningSprite;

    public E_CatType CatType => _catType;
    public Sprite DefaultSprite => _defaultSprite;
    public Sprite GoodSprite => _goodSprite;
    public Sprite EvilSprite => _evilSprite;
    public Sprite RunningSprite => _runningSprite;
}

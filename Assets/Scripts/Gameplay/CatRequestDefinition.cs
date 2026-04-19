using UnityEngine;

[CreateAssetMenu(menuName = "Cat Request Definition")]
public class CatRequestDefinition : ScriptableObject
{
    [SerializeField] private E_CatItem _requiredItem;
    [SerializeField] private E_CatRequestSound _requestSound;
    [SerializeField] private Sprite _bubbleSprite;

    public E_CatItem RequiredItem => _requiredItem;
    public E_CatRequestSound RequestSound => _requestSound;
    public Sprite BubbleSprite => _bubbleSprite;
}

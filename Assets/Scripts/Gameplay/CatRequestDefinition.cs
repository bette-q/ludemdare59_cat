using UnityEngine;

[CreateAssetMenu(menuName = "Cat Request Definition")]
public class CatRequestDefinition : ScriptableObject
{
    [SerializeField] private E_CatItem _requiredItem;
    [SerializeField] private AudioClip _requestAudio;
    [SerializeField] private Sprite _bubbleSprite;

    public E_CatItem RequiredItem => _requiredItem;
    public AudioClip RequestAudio => _requestAudio;
    public Sprite BubbleSprite => _bubbleSprite;
}

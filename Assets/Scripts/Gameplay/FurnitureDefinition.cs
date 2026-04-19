using UnityEngine;

[CreateAssetMenu(menuName = "Furniture Definition")]
public class FurnitureDefinition : ScriptableObject
{
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _tiltSprite;
    [SerializeField] private Sprite _brokenSprite;
    [SerializeField] private Vector3 _scale = Vector3.one;

    public Sprite DefaultSprite => _defaultSprite;
    public Sprite TiltSprite => _tiltSprite;
    public Sprite BrokenSprite => _brokenSprite;
    public Vector3 Scale => _scale;
}

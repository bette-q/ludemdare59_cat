using UnityEngine;

public enum E_SpawnPointKind
{
    Normal,
    Special,
}

public class SpawnPos : MonoBehaviour
{
    public E_SpawnPointKind SpawnPointKind;
    public FurnitureDefinition[] AllowedFurnitureDefinitions;

    public bool IsSpecialOnly => SpawnPointKind == E_SpawnPointKind.Special;
}

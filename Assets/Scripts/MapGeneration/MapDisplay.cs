using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private Renderer _mapRender;
    [FormerlySerializedAs("_landTilemap")] [SerializeField] private Tilemap _tilemap;
    
    public Tilemap Tilemap => _tilemap;

    public void DrawTexture(Texture2D mapTexture)
    {
        _mapRender.sharedMaterial.mainTexture = mapTexture;
        _mapRender.transform.localScale = new Vector3(mapTexture.width, 1, mapTexture.height);
    }
}
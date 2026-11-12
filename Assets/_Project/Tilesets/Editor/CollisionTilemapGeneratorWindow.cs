using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CollisionTilemapGeneratorWindow : EditorWindow
{
    [SerializeField] private Tilemap sourceTilemap;
    [SerializeField] private Tilemap targetCollisionTilemap;
    [SerializeField] private TileBase collisionTile;
    [SerializeField] private List<TileBase> ignoredTiles = new List<TileBase>();

    private SerializedObject serializedWindow;

    [MenuItem("Tools/Cuckoo Clock/Generate Collision Tilemap")]
    public static void Open()
    {
        GetWindow<CollisionTilemapGeneratorWindow>("Collision Tilemap");
    }

    private void OnEnable()
    {
        serializedWindow = new SerializedObject(this);
    }

    private void OnGUI()
    {
        if (serializedWindow == null)
        {
            serializedWindow = new SerializedObject(this);
        }

        serializedWindow.Update();

        EditorGUILayout.PropertyField(serializedWindow.FindProperty(nameof(sourceTilemap)));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty(nameof(targetCollisionTilemap)));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty(nameof(collisionTile)));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty(nameof(ignoredTiles)), true);

        serializedWindow.ApplyModifiedProperties();

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(!CanGenerate()))
        {
            if (GUILayout.Button("Generate"))
            {
                Generate();
            }
        }
    }

    private bool CanGenerate()
    {
        return sourceTilemap != null
            && targetCollisionTilemap != null
            && collisionTile != null;
    }

    private void Generate()
    {
        if (!CanGenerate())
        {
            Debug.LogWarning("CollisionTilemapGenerator: assign sourceTilemap, targetCollisionTilemap, and collisionTile before generating.");
            return;
        }

        Undo.RegisterCompleteObjectUndo(targetCollisionTilemap, "Generate Collision Tilemap");
        targetCollisionTilemap.ClearAllTiles();

        HashSet<TileBase> ignoredTileSet = new HashSet<TileBase>();
        foreach (TileBase ignoredTile in ignoredTiles)
        {
            if (ignoredTile != null)
            {
                ignoredTileSet.Add(ignoredTile);
            }
        }

        BoundsInt bounds = sourceTilemap.cellBounds;
        int generatedTiles = 0;

        foreach (Vector3Int cellPosition in bounds.allPositionsWithin)
        {
            TileBase sourceTile = sourceTilemap.GetTile(cellPosition);
            if (sourceTile == null || ignoredTileSet.Contains(sourceTile))
            {
                continue;
            }

            targetCollisionTilemap.SetTile(cellPosition, collisionTile);
            generatedTiles++;
        }

        ConfigureTargetCollisionTilemap();

        EditorUtility.SetDirty(targetCollisionTilemap);
        EditorUtility.SetDirty(targetCollisionTilemap.gameObject);
        Debug.Log($"CollisionTilemapGenerator: generated {generatedTiles} collision tiles on {targetCollisionTilemap.name}.");
    }

    private void ConfigureTargetCollisionTilemap()
    {
        TilemapRenderer tilemapRenderer = targetCollisionTilemap.GetComponent<TilemapRenderer>();
        if (tilemapRenderer == null)
        {
            tilemapRenderer = Undo.AddComponent<TilemapRenderer>(targetCollisionTilemap.gameObject);
        }

        Undo.RecordObject(tilemapRenderer, "Disable Collision Tilemap Renderer");
        tilemapRenderer.enabled = false;
        EditorUtility.SetDirty(tilemapRenderer);

        Rigidbody2D rigidbody2D = targetCollisionTilemap.GetComponent<Rigidbody2D>();
        if (rigidbody2D == null)
        {
            rigidbody2D = Undo.AddComponent<Rigidbody2D>(targetCollisionTilemap.gameObject);
        }

        Undo.RecordObject(rigidbody2D, "Configure Collision Tilemap Rigidbody");
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        EditorUtility.SetDirty(rigidbody2D);

        CompositeCollider2D compositeCollider = targetCollisionTilemap.GetComponent<CompositeCollider2D>();
        if (compositeCollider == null)
        {
            compositeCollider = Undo.AddComponent<CompositeCollider2D>(targetCollisionTilemap.gameObject);
        }

        Undo.RecordObject(compositeCollider, "Configure Collision Tilemap Composite Collider");
        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Outlines;
        EditorUtility.SetDirty(compositeCollider);

        TilemapCollider2D tilemapCollider = targetCollisionTilemap.GetComponent<TilemapCollider2D>();
        if (tilemapCollider == null)
        {
            tilemapCollider = Undo.AddComponent<TilemapCollider2D>(targetCollisionTilemap.gameObject);
        }

        Undo.RecordObject(tilemapCollider, "Configure Collision Tilemap Collider");
        tilemapCollider.usedByComposite = true;
        EditorUtility.SetDirty(tilemapCollider);
    }
}

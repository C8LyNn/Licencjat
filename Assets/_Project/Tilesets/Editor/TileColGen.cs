using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileColGen : EditorWindow
{
    private enum TileRole
    {
        Horizontal,
        Vertical,
        Block
    }

    private TilemapCollider2D sourceTilemapCollider;
    private Transform outputParent;

    private bool clearOldGenerated = true;
    private bool combineIntoSingleCollider = false;
    private bool mergeNeighbouringRects = true;
    private bool useCompositeCollider = true;
    private bool disableSourceColliderAfterGenerate = true;
    private bool useGridCellBounds = true;
    private bool shrinkExposedSides = true;

    private float mergeGap = 0.08f;
    private float mergeTolerance = 0.02f;
    private float exposedShrinkX = 0.08f;
    private float exposedShrinkY = 0.08f;

    private float horizontalFixedHeight = 0.28f;
    private float verticalFixedWidth = 0.28f;
    private float endInset = 0.02f;
    private float blockInsetX = 0.04f;
    private float blockInsetY = 0.04f;
    private float orientationTolerance = 0.08f;
    private float minColliderSize = 0.01f;

    private const string GeneratedPrefix = "TCG_";

    [MenuItem("Tools/Cuckoo Clock/TileColGen")]
    public static void Open()
    {
        GetWindow<TileColGen>("TileColGen");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Tile collider generator", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Tworzy rowne collidery z tilemapy. Dlugosc bierze ze sprite'a, ale grubosc pionowych/poziomych tile'i jest stala. Nie uzywa starych poszarpanych physics shapes.",
            MessageType.Info
        );

        sourceTilemapCollider = (TilemapCollider2D)EditorGUILayout.ObjectField(
            "Source TilemapCollider2D",
            sourceTilemapCollider,
            typeof(TilemapCollider2D),
            true
        );

        outputParent = (Transform)EditorGUILayout.ObjectField(
            "Output Parent",
            outputParent,
            typeof(Transform),
            true
        );

        EditorGUILayout.Space();

        clearOldGenerated = EditorGUILayout.Toggle("Clear Old Generated", clearOldGenerated);
        combineIntoSingleCollider = EditorGUILayout.Toggle("Combine Into One Collider", combineIntoSingleCollider);
        mergeNeighbouringRects = EditorGUILayout.Toggle("Merge Neighbouring", mergeNeighbouringRects);
        disableSourceColliderAfterGenerate = EditorGUILayout.Toggle(
            "Disable Source After Generate",
            disableSourceColliderAfterGenerate
        );

        if (mergeNeighbouringRects)
        {
            mergeGap = EditorGUILayout.Slider("Merge Gap", mergeGap, 0f, 1f);
            mergeTolerance = EditorGUILayout.Slider("Merge Tolerance", mergeTolerance, 0.001f, 0.2f);
        }

        using (new EditorGUI.DisabledScope(combineIntoSingleCollider))
        {
            useCompositeCollider = EditorGUILayout.Toggle("Use Composite Collider", useCompositeCollider);
        }

        if (combineIntoSingleCollider)
        {
            EditorGUILayout.HelpBox(
                "Polaczone prostokaty trafia do jednego PolygonCollider2D i sa scalane przez CompositeCollider2D, ktory usuwa wewnetrzne krawedzie.",
                MessageType.Info
            );
        }
        else if (useCompositeCollider)
        {
            EditorGUILayout.HelpBox(
                "Zalecany tryb: scalone BoxCollider2D tworza jeden niewypelniony obrys CompositeCollider2D bez wewnetrznych krawedzi.",
                MessageType.Info
            );
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid geometry", EditorStyles.boldLabel);

        useGridCellBounds = EditorGUILayout.Toggle("Use Grid Cell Bounds", useGridCellBounds);

        if (useGridCellBounds)
        {
            shrinkExposedSides = EditorGUILayout.Toggle("Shrink Exposed Sides", shrinkExposedSides);

            if (shrinkExposedSides)
            {
                exposedShrinkX = EditorGUILayout.Slider("Exposed X Shrink", exposedShrinkX, 0f, 0.45f);
                exposedShrinkY = EditorGUILayout.Slider("Exposed Y Shrink", exposedShrinkY, 0f, 0.45f);
            }

            EditorGUILayout.HelpBox(
                "Shrink is a fraction of one grid cell (0.30 = 30%). Only sides without a neighbouring wall tile are moved inward. Shared sides stay connected and can be merged.",
                MessageType.Info
            );
        }
        else
        {
            EditorGUILayout.LabelField("Fixed thickness", EditorStyles.boldLabel);
            horizontalFixedHeight = EditorGUILayout.Slider("Horizontal Fixed Height", horizontalFixedHeight, 0.01f, 2f);
            verticalFixedWidth = EditorGUILayout.Slider("Vertical Fixed Width", verticalFixedWidth, 0.01f, 2f);
            endInset = EditorGUILayout.Slider("End Inset", endInset, 0f, 1f);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Block tiles", EditorStyles.boldLabel);
            blockInsetX = EditorGUILayout.Slider("Block Inset X", blockInsetX, 0f, 1f);
            blockInsetY = EditorGUILayout.Slider("Block Inset Y", blockInsetY, 0f, 1f);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Detection", EditorStyles.boldLabel);

        if (!useGridCellBounds)
        {
            orientationTolerance = EditorGUILayout.Slider("Orientation Tolerance", orientationTolerance, 0f, 1f);
        }

        minColliderSize = EditorGUILayout.Slider("Min Collider Size", minColliderSize, 0.001f, 0.2f);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
        {
            Generate();
        }
    }

    private void Generate()
    {
        if (sourceTilemapCollider == null)
        {
            Debug.LogError("TileColGen: Source TilemapCollider2D is not assigned.");
            return;
        }

        if (outputParent == null)
        {
            Debug.LogError("TileColGen: Output Parent is not assigned.");
            return;
        }

        Tilemap tilemap = sourceTilemapCollider.GetComponent<Tilemap>();

        if (tilemap == null)
        {
            Debug.LogError("TileColGen: Source object has no Tilemap component.");
            return;
        }

        if (clearOldGenerated)
        {
            ClearOldGenerated();
        }

        ConfigureOutputParent(combineIntoSingleCollider || useCompositeCollider);

        BoundsInt bounds = tilemap.cellBounds;

        int foundTiles = 0;
        int generated = 0;
        int horizontal = 0;
        int vertical = 0;
        int blocks = 0;
        int skipped = 0;
        List<RectData> rectangles = new List<RectData>();

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                if (!HasCollisionTile(tilemap, cell))
                {
                    continue;
                }

                foundTiles++;

                Sprite sprite = tilemap.GetSprite(cell);
                RectData rect = useGridCellBounds
                    ? BuildGridCellRect(tilemap, cell)
                    : BuildSpriteRect(tilemap, cell, sprite);

                if (rect.size.x < minColliderSize || rect.size.y < minColliderSize)
                {
                    skipped++;
                    continue;
                }

                rectangles.Add(rect);

                if (rect.role == TileRole.Horizontal)
                {
                    horizontal++;
                }
                else if (rect.role == TileRole.Vertical)
                {
                    vertical++;
                }
                else
                {
                    blocks++;
                }
            }
        }

        int unmergedRectangleCount = rectangles.Count;

        if (mergeNeighbouringRects)
        {
            rectangles = MergeRectangles(rectangles);
        }

        if (combineIntoSingleCollider)
        {
            generated = CreateCombinedPolygonCollider(rectangles) ? 1 : 0;
        }
        else
        {
            for (int i = 0; i < rectangles.Count; i++)
            {
                CreateBoxCollider(rectangles[i].center, rectangles[i].size, i);
                generated++;
            }
        }

        bool sourceColliderDisabled = false;
        if (generated > 0 && disableSourceColliderAfterGenerate)
        {
            Undo.RecordObject(sourceTilemapCollider, "Disable Source Tilemap Collider");
            sourceTilemapCollider.enabled = false;
            EditorUtility.SetDirty(sourceTilemapCollider);
            sourceColliderDisabled = true;
        }

        Debug.Log(
            "TileColGen finished. Tiles: " + foundTiles +
            ", rectangles before merge: " + unmergedRectangleCount +
            ", paths after merge: " + rectangles.Count +
            ", collider components generated: " + generated +
            ", horizontal: " + horizontal +
            ", vertical: " + vertical +
            ", blocks: " + blocks +
            ", skipped: " + skipped +
            ", source collider disabled: " + sourceColliderDisabled +
            ", layer: " + LayerMask.LayerToName(sourceTilemapCollider.gameObject.layer)
        );
    }

    private List<RectData> MergeRectangles(List<RectData> source)
    {
        List<RectData> merged = new List<RectData>(source);

        for (int pass = 0; pass < 16; pass++)
        {
            int previousCount = merged.Count;
            merged = MergeAlongX(merged);
            merged = MergeAlongY(merged);

            if (merged.Count == previousCount)
            {
                break;
            }
        }

        return merged;
    }

    private List<RectData> MergeAlongX(List<RectData> source)
    {
        List<RectData> sorted = new List<RectData>(source);
        sorted.Sort((a, b) =>
        {
            int role = a.role.CompareTo(b.role);
            if (role != 0) return role;

            int minY = a.MinY.CompareTo(b.MinY);
            if (minY != 0) return minY;

            int maxY = a.MaxY.CompareTo(b.MaxY);
            if (maxY != 0) return maxY;

            return a.MinX.CompareTo(b.MinX);
        });

        List<RectData> result = new List<RectData>();

        foreach (RectData rect in sorted)
        {
            int lastIndex = result.Count - 1;

            if (lastIndex >= 0 && CanMergeAlongX(result[lastIndex], rect))
            {
                result[lastIndex] = RectData.FromBounds(
                    Mathf.Min(result[lastIndex].MinX, rect.MinX),
                    Mathf.Max(result[lastIndex].MaxX, rect.MaxX),
                    Mathf.Min(result[lastIndex].MinY, rect.MinY),
                    Mathf.Max(result[lastIndex].MaxY, rect.MaxY),
                    rect.role
                );
            }
            else
            {
                result.Add(rect);
            }
        }

        return result;
    }

    private List<RectData> MergeAlongY(List<RectData> source)
    {
        List<RectData> sorted = new List<RectData>(source);
        sorted.Sort((a, b) =>
        {
            int role = a.role.CompareTo(b.role);
            if (role != 0) return role;

            int minX = a.MinX.CompareTo(b.MinX);
            if (minX != 0) return minX;

            int maxX = a.MaxX.CompareTo(b.MaxX);
            if (maxX != 0) return maxX;

            return a.MinY.CompareTo(b.MinY);
        });

        List<RectData> result = new List<RectData>();

        foreach (RectData rect in sorted)
        {
            int lastIndex = result.Count - 1;

            if (lastIndex >= 0 && CanMergeAlongY(result[lastIndex], rect))
            {
                result[lastIndex] = RectData.FromBounds(
                    Mathf.Min(result[lastIndex].MinX, rect.MinX),
                    Mathf.Max(result[lastIndex].MaxX, rect.MaxX),
                    Mathf.Min(result[lastIndex].MinY, rect.MinY),
                    Mathf.Max(result[lastIndex].MaxY, rect.MaxY),
                    rect.role
                );
            }
            else
            {
                result.Add(rect);
            }
        }

        return result;
    }

    private bool CanMergeAlongX(RectData a, RectData b)
    {
        if (a.role != b.role || a.role == TileRole.Vertical)
        {
            return false;
        }

        return Mathf.Abs(a.MinY - b.MinY) <= mergeTolerance &&
               Mathf.Abs(a.MaxY - b.MaxY) <= mergeTolerance &&
               b.MinX <= a.MaxX + mergeGap;
    }

    private bool CanMergeAlongY(RectData a, RectData b)
    {
        if (a.role != b.role || a.role == TileRole.Horizontal)
        {
            return false;
        }

        return Mathf.Abs(a.MinX - b.MinX) <= mergeTolerance &&
               Mathf.Abs(a.MaxX - b.MaxX) <= mergeTolerance &&
               b.MinY <= a.MaxY + mergeGap;
    }

    private RectData BuildGridCellRect(Tilemap tilemap, Vector3Int cell)
    {
        Vector3 worldCenter3D = tilemap.GetCellCenterWorld(cell);
        Vector2 worldCenter = new Vector2(worldCenter3D.x, worldCenter3D.y);
        Vector2 worldSize = GetFallbackCellWorldSize(tilemap);

        float minX = worldCenter.x - worldSize.x * 0.5f;
        float maxX = worldCenter.x + worldSize.x * 0.5f;
        float minY = worldCenter.y - worldSize.y * 0.5f;
        float maxY = worldCenter.y + worldSize.y * 0.5f;

        if (shrinkExposedSides)
        {
            float insetX = worldSize.x * Mathf.Clamp(exposedShrinkX, 0f, 0.45f);
            float insetY = worldSize.y * Mathf.Clamp(exposedShrinkY, 0f, 0.45f);

            if (!HasCollisionTile(tilemap, cell + Vector3Int.left)) minX += insetX;
            if (!HasCollisionTile(tilemap, cell + Vector3Int.right)) maxX -= insetX;
            if (!HasCollisionTile(tilemap, cell + Vector3Int.down)) minY += insetY;
            if (!HasCollisionTile(tilemap, cell + Vector3Int.up)) maxY -= insetY;
        }

        return RectData.FromBounds(minX, maxX, minY, maxY, TileRole.Block);
    }

    private static bool HasCollisionTile(Tilemap tilemap, Vector3Int cell)
    {
        return tilemap.HasTile(cell) && tilemap.GetColliderType(cell) != Tile.ColliderType.None;
    }

    private RectData BuildSpriteRect(Tilemap tilemap, Vector3Int cell, Sprite sprite)
    {
        Vector3 worldCenter = tilemap.GetCellCenterWorld(cell);
        Vector2 worldSize = GetFallbackCellWorldSize(tilemap);

        if (sprite != null)
        {
            Vector2 spriteWorldSize = GetSpriteWorldSize(sprite, tilemap.transform);

            if (spriteWorldSize.x > minColliderSize && spriteWorldSize.y > minColliderSize)
            {
                worldSize = spriteWorldSize;
            }

            Vector2 spriteWorldOffset = GetSpriteWorldOffset(sprite, tilemap.transform);
            worldCenter += new Vector3(spriteWorldOffset.x, spriteWorldOffset.y, 0f);
        }

        TileRole role = DetectRole(worldSize);

        if (role == TileRole.Horizontal)
        {
            float width = Mathf.Max(minColliderSize, worldSize.x - endInset * 2f);
            float height = Mathf.Max(minColliderSize, horizontalFixedHeight);

            return new RectData(
                new Vector2(worldCenter.x, worldCenter.y),
                new Vector2(width, height),
                role
            );
        }

        if (role == TileRole.Vertical)
        {
            float width = Mathf.Max(minColliderSize, verticalFixedWidth);
            float height = Mathf.Max(minColliderSize, worldSize.y - endInset * 2f);

            return new RectData(
                new Vector2(worldCenter.x, worldCenter.y),
                new Vector2(width, height),
                role
            );
        }

        float blockWidth = Mathf.Max(minColliderSize, worldSize.x - blockInsetX * 2f);
        float blockHeight = Mathf.Max(minColliderSize, worldSize.y - blockInsetY * 2f);

        return new RectData(
            new Vector2(worldCenter.x, worldCenter.y),
            new Vector2(blockWidth, blockHeight),
            role
        );
    }

    private TileRole DetectRole(Vector2 size)
    {
        float tolerance = 1f + orientationTolerance;

        if (size.x > size.y * tolerance)
        {
            return TileRole.Horizontal;
        }

        if (size.y > size.x * tolerance)
        {
            return TileRole.Vertical;
        }

        return TileRole.Block;
    }

    private Vector2 GetSpriteWorldSize(Sprite sprite, Transform sourceTransform)
    {
        Vector3 lossyScale = sourceTransform.lossyScale;

        return new Vector2(
            Mathf.Abs(sprite.bounds.size.x * lossyScale.x),
            Mathf.Abs(sprite.bounds.size.y * lossyScale.y)
        );
    }

    private Vector2 GetSpriteWorldOffset(Sprite sprite, Transform sourceTransform)
    {
        Vector3 lossyScale = sourceTransform.lossyScale;

        return new Vector2(
            sprite.bounds.center.x * lossyScale.x,
            sprite.bounds.center.y * lossyScale.y
        );
    }

    private Vector2 GetFallbackCellWorldSize(Tilemap tilemap)
    {
        Vector3 cellSize = tilemap.layoutGrid != null ? tilemap.layoutGrid.cellSize : Vector3.one;
        Vector3 lossyScale = tilemap.transform.lossyScale;

        return new Vector2(
            Mathf.Abs(cellSize.x * lossyScale.x),
            Mathf.Abs(cellSize.y * lossyScale.y)
        );
    }

    private void CreateBoxCollider(Vector2 worldCenter, Vector2 worldSize, int index)
    {
        GameObject colliderObject = new GameObject(GeneratedPrefix + index);
        Undo.RegisterCreatedObjectUndo(colliderObject, "Generate Tile Collider");

        colliderObject.layer = sourceTilemapCollider.gameObject.layer;

        colliderObject.transform.position = new Vector3(worldCenter.x, worldCenter.y, 0f);
        colliderObject.transform.rotation = Quaternion.identity;
        colliderObject.transform.localScale = Vector3.one;
        colliderObject.transform.SetParent(outputParent, true);

        BoxCollider2D boxCollider = colliderObject.AddComponent<BoxCollider2D>();
        boxCollider.size = worldSize;

        if (useCompositeCollider)
        {
            TrySetCompositeOperation(boxCollider, "Merge");
        }

        EditorUtility.SetDirty(colliderObject);
    }

    private bool CreateCombinedPolygonCollider(List<RectData> rectangles)
    {
        if (rectangles.Count == 0)
        {
            return false;
        }

        Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        foreach (RectData rect in rectangles)
        {
            min.x = Mathf.Min(min.x, rect.MinX);
            min.y = Mathf.Min(min.y, rect.MinY);
            max.x = Mathf.Max(max.x, rect.MaxX);
            max.y = Mathf.Max(max.y, rect.MaxY);
        }

        Vector2 worldCenter = (min + max) * 0.5f;
        GameObject colliderObject = new GameObject(GeneratedPrefix + "Combined");
        Undo.RegisterCreatedObjectUndo(colliderObject, "Generate Combined Tile Collider");

        colliderObject.layer = sourceTilemapCollider.gameObject.layer;
        colliderObject.transform.position = new Vector3(worldCenter.x, worldCenter.y, 0f);
        colliderObject.transform.rotation = Quaternion.identity;
        colliderObject.transform.localScale = Vector3.one;
        colliderObject.transform.SetParent(outputParent, true);

        PolygonCollider2D polygon = colliderObject.AddComponent<PolygonCollider2D>();
        polygon.pathCount = rectangles.Count;

        for (int i = 0; i < rectangles.Count; i++)
        {
            RectData rect = rectangles[i];
            Vector2[] worldPoints =
            {
                new Vector2(rect.MinX, rect.MinY),
                new Vector2(rect.MaxX, rect.MinY),
                new Vector2(rect.MaxX, rect.MaxY),
                new Vector2(rect.MinX, rect.MaxY)
            };
            Vector2[] localPoints = new Vector2[worldPoints.Length];

            for (int pointIndex = 0; pointIndex < worldPoints.Length; pointIndex++)
            {
                Vector3 local = colliderObject.transform.InverseTransformPoint(worldPoints[pointIndex]);
                localPoints[pointIndex] = new Vector2(local.x, local.y);
            }

            polygon.SetPath(i, localPoints);
        }

        if (!TrySetCompositeOperation(polygon, "Merge"))
        {
            Debug.LogWarning("TileColGen: could not connect the combined polygon to CompositeCollider2D.");
        }

        EditorUtility.SetDirty(colliderObject);
        return true;
    }

    private void ConfigureOutputParent(bool enableComposite)
    {
        outputParent.gameObject.layer = sourceTilemapCollider.gameObject.layer;

        Rigidbody2D rb = outputParent.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = outputParent.gameObject.AddComponent<Rigidbody2D>();
        }

        rb.bodyType = RigidbodyType2D.Static;

        CompositeCollider2D composite = outputParent.GetComponent<CompositeCollider2D>();

        if (enableComposite)
        {
            if (composite == null)
            {
                composite = outputParent.gameObject.AddComponent<CompositeCollider2D>();
            }

            composite.isTrigger = false;
            composite.geometryType = CompositeCollider2D.GeometryType.Outlines;
            composite.generationType = CompositeCollider2D.GenerationType.Synchronous;
        }
        else if (composite != null)
        {
            Undo.DestroyObjectImmediate(composite);
        }

        EditorUtility.SetDirty(outputParent.gameObject);
    }

    private void ClearOldGenerated()
    {
        for (int i = outputParent.childCount - 1; i >= 0; i--)
        {
            Transform child = outputParent.GetChild(i);

            if (child.name.StartsWith(GeneratedPrefix) || child.name.StartsWith("Generated_"))
            {
                Undo.DestroyObjectImmediate(child.gameObject);
            }
        }
    }

    private bool TrySetCompositeOperation(Collider2D collider, string operationName)
    {
        PropertyInfo property = typeof(Collider2D).GetProperty(
            "compositeOperation",
            BindingFlags.Public | BindingFlags.Instance
        );

        if (property == null || !property.CanWrite)
        {
            return false;
        }

        object mergeValue = null;

        foreach (object enumValue in Enum.GetValues(property.PropertyType))
        {
            if (enumValue.ToString() == operationName)
            {
                mergeValue = enumValue;
                break;
            }
        }

        if (mergeValue == null)
        {
            return false;
        }

        property.SetValue(collider, mergeValue);
        return true;
    }

    private struct RectData
    {
        public Vector2 center;
        public Vector2 size;
        public TileRole role;

        public RectData(Vector2 center, Vector2 size, TileRole role)
        {
            this.center = center;
            this.size = size;
            this.role = role;
        }

        public float MinX => center.x - size.x * 0.5f;
        public float MaxX => center.x + size.x * 0.5f;
        public float MinY => center.y - size.y * 0.5f;
        public float MaxY => center.y + size.y * 0.5f;

        public static RectData FromBounds(float minX, float maxX, float minY, float maxY, TileRole role)
        {
            return new RectData(
                new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f),
                new Vector2(maxX - minX, maxY - minY),
                role
            );
        }
    }
}

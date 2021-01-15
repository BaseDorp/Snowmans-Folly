﻿using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Camera generationRangeCamera = null;
    [SerializeField] private Transform player = null;
    [SerializeField] private TerrainComponent[] terrainPieces = null;

    [SerializeField] private CurveMover2D leftHandTerrainCollider = null;
    [SerializeField] private CurveMover2D rightHandTerrainCollider = null;

    [SerializeField] private int poolSize = 1;
    [SerializeField] private float waterStep = 1f;

    private Vector2 currentLocation;

    private bool onLandSegment;

    private enum BehaviourMode : byte
    {
        Generation,
        Preview
    }

    [SerializeField] private BehaviourMode behaviour = BehaviourMode.Generation;

    [Serializable]
    private sealed class TerrainComponent
    {
        public GameObject prefab = null;
        public Vector2[] distributionAreas = null;
    }

    private void OnValidate()
    {
        if (poolSize < 1)
            poolSize = 1;
        if (terrainPieces != null)
        {
            foreach (TerrainComponent component in terrainPieces)
            {
                if (component.distributionAreas != null)
                {
                    // Force distribution areas to not overlap.
                    for (int i = 1; i < component.distributionAreas.Length; i++)
                    {
                        if (component.distributionAreas[i].x <
                            component.distributionAreas[i - 1].x)
                            component.distributionAreas[i].x =
                                component.distributionAreas[i - 1].x;
                    }
                    // Ensure no weights of less than zero.
                    for (int i = 0; i < component.distributionAreas.Length; i++)
                        if (component.distributionAreas[i].y < 0f)
                            component.distributionAreas[i].y = 0f;
                }
            }
        }
    }

    private TerrainSegment[][] terrainPools;
    private int[] poolCycleIndices;

    private Queue<TerrainSegment> newSegmentsToPass;


    private Dictionary<TerrainSegmentType, List<int>> typedIndices;

    private void Awake()
    {
        onLandSegment = false;

        poolCycleIndices = new int[terrainPieces.Length];
        typedIndices = new Dictionary<TerrainSegmentType, List<int>>();
        foreach (TerrainSegmentType value in Enum.GetValues(typeof(TerrainSegmentType)))
            typedIndices.Add(value, new List<int>());

        terrainPools = new TerrainSegment[terrainPieces.Length][];
        for (int i = 0; i < terrainPieces.Length; i++)
        {
            terrainPools[i] = new TerrainSegment[poolSize];
            for (int j = 0; j < poolSize; j++)
                terrainPools[i][j] =
                    Instantiate(terrainPieces[i].prefab).GetComponent<TerrainSegment>();
            TerrainSegment segment = terrainPools[i][0];
            typedIndices[segment.SegmentType].Add(i);
        }
        ResetGeneration();
    }

    public void ResetGeneration()
    {
        newSegmentsToPass = new Queue<TerrainSegment>();
        // Reset all pooled objects to their initial position.
        foreach (TerrainSegment[] pool in terrainPools)
            foreach (TerrainSegment segment in pool)
                segment.transform.position = Vector2.down * 10f;
        // Generate the first two segments, this ensures
        // that both terrain colliders always have target curves.
        currentLocation = transform.position;
        for (int i = 0; i < 2; i++)
            GenerateNextSegment();
        // Immediately dequeue the registered segments
        // and assign them to the curve movers.
        leftHandTerrainCollider.Curve = newSegmentsToPass.Dequeue().Curve;
        rightHandTerrainCollider.Curve = newSegmentsToPass.Dequeue().Curve;
    }
    private void GenerateNextSegment()
    {
        int nextSegment = typedIndices[TerrainSegmentType.Continuous].RandomElement();
        terrainPools[nextSegment][poolCycleIndices[nextSegment]].SnapLeftTo(currentLocation);
        newSegmentsToPass.Enqueue(terrainPools[nextSegment][poolCycleIndices[nextSegment]]);
        currentLocation += terrainPools[nextSegment][poolCycleIndices[nextSegment]].DeltaPosition;
        if (poolCycleIndices[nextSegment] < poolSize - 1)
            poolCycleIndices[nextSegment]++;
        else
            poolCycleIndices[nextSegment] = 0;
    }
    private void Update()
    {
        switch (behaviour)
        {
            case BehaviourMode.Preview: UpdatePreview(); break;
            case BehaviourMode.Generation: UpdateGeneration(); break;
            default: throw new NotImplementedException();
        }
        void UpdatePreview()
        {

        }
        void UpdateGeneration()
        {
            Rect cameraBounds = generationRangeCamera.GetWorldSpace2DRect();
            while (cameraBounds.xMax > currentLocation.x)
                GenerateNextSegment();
            while (true)
            {
                if (newSegmentsToPass.Count == 0)
                    break;
                if (player.position.x >= newSegmentsToPass.Peek().Curve.Left)
                {
                    leftHandTerrainCollider.Curve = rightHandTerrainCollider.Curve;
                    rightHandTerrainCollider.Curve = newSegmentsToPass.Dequeue().Curve;
                }
                else
                    break;
            }
        }
    }
    private void FixedUpdate()
    {
        leftHandTerrainCollider.MoveTo(player.position.x);
        rightHandTerrainCollider.MoveTo(player.position.x);
    }
}

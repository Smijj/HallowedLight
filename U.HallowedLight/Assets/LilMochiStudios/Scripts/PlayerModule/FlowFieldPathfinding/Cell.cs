using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.PlayerModule {
    public class Cell
    {
        public Vector3 WorldPosition { get; private set; }
        public Vector2Int GridIndex { get; private set; }
        public byte Cost;
        public ushort BestCost;
        public GridDirection BestDirection;

        public Cell(Vector3 _worldPosition, Vector2Int _gridIndex) {
            WorldPosition = _worldPosition;
            GridIndex = _gridIndex;
            Cost = 1;
            BestCost = ushort.MaxValue;
            BestDirection = GridDirection.None;
        }

        public void UpdateWorldPosition(Vector3 newWorldPosition) {
            WorldPosition = newWorldPosition;
        }

        public void SetCost(int amount) {
            if (amount >= 255) Cost = byte.MaxValue;
            else Cost = (byte)amount;
        }
    }
}

using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;

using Game.Managers;

namespace Game.Tiles
{
    public class TileInfo : MonoBehaviour
    {
        [Title("Movement Info")]
        [SerializeField] private int _costToMoveHere;

        [Title("References")]
        [SerializeField] private List<Transform> _edges = new List<Transform>();

        public int costToMoveHere => _costToMoveHere;
        public List<Transform> edges => _edges;

        private TileType _tileType;
        private GridTile _gridTile;

        private void Awake()
        {
            _tileType = GetComponent<TileType>();
            _gridTile = GetComponent<GridTile>();
        }

        private void Start()
        {
            _tileType.onTileSpawn.AddListener(SetMovementCost);
        }

        /// <summary>
        /// Sets the movement cost for both the GridTile and the TileInfo
        /// </summary>
        private void SetMovementCost()
        {
            switch (_tileType.tileType)
            {
                case Managers.TileManager.TileType.Plain:
                    _costToMoveHere = TileManager.instance.plainCost;
                    _gridTile.m_costOfMovingToTile = TileManager.instance.plainCost;
                    break;
                case Managers.TileManager.TileType.Forest:
                    _costToMoveHere = TileManager.instance.forestCost;
                    _gridTile.m_costOfMovingToTile = TileManager.instance.forestCost;
                    break;
                case Managers.TileManager.TileType.Hill:
                    _costToMoveHere = TileManager.instance.hillCost;
                    _gridTile.m_costOfMovingToTile = TileManager.instance.hillCost;
                    break;
                case Managers.TileManager.TileType.Mountain:
                    _costToMoveHere = TileManager.instance.mountainCost;
                    _gridTile.m_costOfMovingToTile = TileManager.instance.mountainCost;
                    break;
                case Managers.TileManager.TileType.Swamp:
                    _costToMoveHere = TileManager.instance.swampCost;
                    _gridTile.m_costOfMovingToTile = TileManager.instance.swampCost;
                    break;
                case Managers.TileManager.TileType.Island:
                    _costToMoveHere = TileManager.instance.islandCost;
                    _gridTile.m_costOfMovingToTile = TileManager.instance.islandCost;
                    break;
                case Managers.TileManager.TileType.Water:
                    _costToMoveHere = 1000;
                    _gridTile.m_costOfMovingToTile = 1000;
                    break;
            }
        }

        /// <summary>
        /// Returns the positions in world space of this tile's edges
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetEdges()
        {
            Vector3[] edgesPositions = new Vector3[edges.Count];

            for (int i = 0; i < _edges.Count; i++)
            {
                edgesPositions[i] = _edges[i].position;
            }

            return edgesPositions;
        }
    }
}
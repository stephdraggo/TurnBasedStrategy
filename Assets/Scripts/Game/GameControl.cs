using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Serializable = System.SerializableAttribute;

namespace TurnBasedStrategy.Gameplay
{
    /// <summary>
    /// Singleton class that handles progression and events with each turn
    /// </summary>
    public class GameControl : MonoBehaviour
    {
        #region instance
        public static GameControl instance;
        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        #endregion

        #region spawn starting units
        [Serializable]
        struct StartingUnit
        {
            public Unit unitPrefab;
            public Vector2Int startPosition;
        }
        [SerializeField] List<StartingUnit> startingUnits;

        private void Start() => StartCoroutine(SpawnStartingUnits());

        bool spawning;

        public IEnumerator SpawnStartingUnits()
        {
            spawning = true;
            StartCoroutine(SpawnStartingFishRoutine());

            yield return new WaitUntil(() => !spawning);
            foreach (StartingUnit _unit in startingUnits)
            {
                if (!UnitSpawner.instance.SpawnUnit(_unit.unitPrefab, _unit.startPosition))
                {
                    Debug.Log("No free space to spawn starting unit!");
                }
                yield return null;
            }

            TurnControl.instance.StartGame();
        }
        #endregion

        public void StartPlayerTurn()
        {
            //Things to happen at the start of the player turn...
        }

        public void StartEnemyTurn()
        {
            //Spawn hooks and stuff
        }

        #region Fish spawning

        [Header("Fish spawning")]
        [SerializeField] Unit fishPrefab;
        [SerializeField] int maxFishOnBoard = 5;
        [SerializeField] int startingFishOnBoard = 2;
        [SerializeField] float fishSpawnChance = 0.5f;
        [SerializeField] int minFishSpawnY = 1;
        [SerializeField] int maxFishSpawnY = 4;

        public void SpawnFish() => SpawnFish(fishSpawnChance);

        /// <summary>
        /// Has a chance of spawning a fish if there is room for one
        /// </summary>
        /// <param name="_chance">Chance between 0 and 1 of spawning a fish</param>
        void SpawnFish(float _chance)
        {
            //if there are too many fish on the board, return
            if (TurnControl.instance.GetAllUnitsInTeam(UnitTeam.fish).Count >= maxFishOnBoard) return;

            //Chance of not spawning fish and returning
            if (Random.Range(0f, 1f) >= fishSpawnChance) return;

            //Get possible positions for fish to spawn
            List<Vector2Int> possibleSpawnPositions = new List<Vector2Int>();

            for (int i = Mathf.Max(0, minFishSpawnY); i <= Mathf.Min(maxFishSpawnY, Map.instance.GridSize.y - 1); i++)
            {
                //Get all the tiles on the left and right side of the map up to the maximum y
                Tile checkTileLeft = Map.instance.Tiles[0, i];
                Tile checkTileRight = Map.instance.Tiles[Map.instance.GridSize.x - 1, i];

                //if they are walkable and don't have a unit on them, add them to the possible spawn positions
                if (checkTileLeft.IsTileWalkable(fishPrefab) && checkTileLeft.CurrentUnit == null) 
                    possibleSpawnPositions.Add(checkTileLeft.GridPosition);

                if (checkTileRight.IsTileWalkable(fishPrefab) && checkTileRight.CurrentUnit == null)
                    possibleSpawnPositions.Add(checkTileRight.GridPosition);
            }

            //if no spawn positions are available, return
            if (possibleSpawnPositions.Count == 0) return;

            //get a random spawn position
            Vector2Int spawnPosition = possibleSpawnPositions[Random.Range(0, possibleSpawnPositions.Count)];

            //Spawn the fish
            UnitSpawner.instance.SpawnUnit(fishPrefab, spawnPosition);
        }

        public IEnumerator SpawnStartingFishRoutine()
        {
            for(int i = 0; i < startingFishOnBoard; i++)
            {
                Vector2Int minSpawnPos = new Vector2Int(0, minFishSpawnY);
                Vector2Int maxSpawnPos = new Vector2Int(Map.instance.GridSize.x - 1, maxFishSpawnY);
                Tile spawnTile = Map.instance.GetRandomFreeTile(fishPrefab, minSpawnPos, maxSpawnPos);
                if (spawnTile != null)
                {
                    UnitSpawner.instance.SpawnUnit(fishPrefab, spawnTile.GridPosition);
                    yield return null;
                    
                }
                else Debug.Log("No free tile to spawn fish!");

            }

            spawning = false;
        }
        #endregion
    }
}
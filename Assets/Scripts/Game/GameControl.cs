using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BigBoi.Menus;

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

        private GameSceneMenuManager menuManager;

        private void Start()
        {
            StartCoroutine(SpawnStartingUnits());
            SetWinConditionText();

            menuManager = FindObjectOfType<GameSceneMenuManager>();
        }

        #region win/lose game

        enum WinCondition
        {
            surviveTurns,
            defeatBoats
        }

        [SerializeField] WinCondition winCondition;
        [SerializeField] int winConditionValue;

        [SerializeField] Text winConditionText;

        public void StartPlayerTurn()
        {
            //if enough turns have passed, win the game
            if (winCondition == WinCondition.surviveTurns && TurnControl.instance.TurnNumber > winConditionValue) WinGame();
            //bug: if goal is 3 turns, this only triggers at start of 5th turn
            //suggestion: call this on end turn and >= instead of >

        }

        public void WinGame()
        {
            //To do: Win screen

            menuManager.EnablePanelByIndex(3); //win panel
            //suggestion: disable selecting units here?
            //same for lose game method

            //SceneManager.LoadScene(0);
        }

        public void LoseGame()
        {
            //To do: Lose screen

            menuManager.EnablePanelByIndex(4); //lose panel

            //SceneManager.LoadScene(0);
        }

        void SetWinConditionText()
        {
            switch (winCondition)
            {
                case WinCondition.surviveTurns: winConditionText.text = "Survive " + winConditionValue + " turn" + (winConditionValue - boatsDestroyed != 1 ? "s" : "");
                    break;
                case WinCondition.defeatBoats: winConditionText.text = "Defeat " + (winConditionValue - boatsDestroyed) + " boat" + (winConditionValue - boatsDestroyed != 1 ? "s" : "");
                    break;
            }
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

        bool spawning;

        public IEnumerator SpawnStartingUnits()
        {
            spawning = true;
            StartCoroutine(SpawnStartingFishRoutine());

            yield return new WaitUntil(() => !spawning);
            foreach (StartingUnit _unit in startingUnits)
            {
                if (UnitSpawner.instance.SpawnUnit(_unit.unitPrefab, _unit.startPosition) == null)
                {
                    Debug.Log("No free space to spawn starting unit!");
                }
                yield return null;
            }

            SpawnNewBoat();
            boatSpawnCount = boatSpawnInterval;

            TurnControl.instance.StartGame();
        }
        #endregion

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

        #region boats
        [Header("Enemies")]
        [SerializeField] Boat boatPrefab;
        [SerializeField] Enemy hookPrefab;
        //List of boats
        List<Boat> boats = new List<Boat>();

        [Header("Boat spawning")]
        [SerializeField] int maxBoats = 2;
        [SerializeField] int boatSpawnInterval = 5;
        int boatSpawnCount;
        [SerializeField] float boatSpawnY, boatSpawnZ;

        int boatsDestroyed = 0;

        public void SpawnEnemies()
        {
            if (boatSpawnCount > 0) boatSpawnCount--;
            else if (boats.Count < maxBoats)
            {
                SpawnNewBoat();
                boatSpawnCount = boatSpawnInterval;
            }
            SpawnHooks();
        }

        List<int> GetValidBoatLocations()
        {
            List<int> validBoatLocations = new List<int>();

            //Get a list of all possible boat locations,
            //gridsize - 1 is used because boats should not spawn on the last tile or they will be half off the map
            for (int i = 0; i < Map.instance.GridSize.x - 1; i++) validBoatLocations.Add(i);

            //For each boat on the map
            foreach(Boat boat in boats)
            {
                //Remove the locations this boat is taking up from the list
                int[] locationsToRemove = new int[3] { boat.PositionX - 1, boat.PositionX, boat.PositionX + 1 };
                foreach (int location in locationsToRemove) if (validBoatLocations.Contains(location)) validBoatLocations.Remove(location);
            }
            

            return validBoatLocations;
        }

        /// <summary>
        /// Places a new boat in the scene
        /// </summary>
        void SpawnNewBoat()
        {
            List<int> validBoatLocations = GetValidBoatLocations();
            if (validBoatLocations.Count == 0) return;

            //Get a random spawn position from the boat locations list
            int spawnPositionX = validBoatLocations[Random.Range(0, validBoatLocations.Count)];

            //spawn the boat and set it up
            Boat newBoat = Instantiate(boatPrefab, new Vector3(transform.position.x, boatSpawnY, boatSpawnZ), transform.rotation) as Boat;
            newBoat.Setup(spawnPositionX, hookPrefab);

            //Add the boat to the boats list
            boats.Add(newBoat);

        }

        public void RemoveBoat(Boat _boat)
        {
            if (boats.Contains(_boat)) boats.Remove(_boat);

            boatsDestroyed++;

            if (winCondition == WinCondition.defeatBoats && boatsDestroyed >= winConditionValue) WinGame();
        }

        void SpawnHooks()
        {
            foreach (Boat boat in boats) boat.SpawnHook();
        }
        #endregion


    }
}
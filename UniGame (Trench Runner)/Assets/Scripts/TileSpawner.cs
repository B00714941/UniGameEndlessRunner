using System.Collections.Generic;
using UnityEngine;

namespace Runner
{
    public class TileSpawner : MonoBehaviour
    {
        //SetUp Tile Lists
        [SerializeField]
        private int tileStartCount = 5;
        [SerializeField]
        private int minStraightTiles = 3;
        [SerializeField]
        private int maxStraightTiles = 9;
        [SerializeField]
        private GameObject startingTile;
        [SerializeField]
        private List<GameObject> turnTiles;
        [SerializeField]
        private List<GameObject> obstacles;
        [SerializeField]
        private List<GameObject> straights;
        [SerializeField]
        private List<GameObject> powerups;

        private Vector3 currentTileLocation = Vector3.zero;
        private Vector3 currentTileDirection = Vector3.forward;
        private GameObject prevTile;

        private List<GameObject> currentTiles;
        private List<GameObject> currentObstacles;

        //Connected to health in the playerController
        public GameObject curHealthGO;
        [SerializeField]
        int curHealth;


        //Once player passes an obstacle and turns a corner, delete previous row of tiles and obstacles
        private void DeletePrevious()
        {
            //change to ObjectPooling Later

            while (currentTiles.Count != 1)
            {
                GameObject tile = currentTiles[0];
                currentTiles.RemoveAt(0);
                Destroy(tile);
            }

            while (currentObstacles.Count != 0)
            {
                GameObject obstacle = currentObstacles[0];
                currentObstacles.RemoveAt(0);
                Destroy(obstacle);
            }
        }


        //Randomly spawn a obstacle along the path
        private void SpawnObstacle()
        {
            if (Random.value > 0.8f) return;

            GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
            Quaternion newObjectRotation = obstaclePrefab.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

            GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObjectRotation);
            currentObstacles.Add(obstacle);

        }

        //Randomly spawn a bandage along the path
        private void SpawnPowerUp()
        {
            if (Random.value > 0.2f) return;

            GameObject powerupPrefab = SelectRandomGameObjectFromList(powerups);
            Quaternion newObjectRotation = powerupPrefab.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

            GameObject powerup = Instantiate(powerupPrefab, (currentTileLocation + new Vector3(0,1,0)), newObjectRotation);
            currentObstacles.Add(powerup);

        }



        public void AddNewDirection(Vector3 direction)
        {
            currentTileDirection = direction;
            DeletePrevious();

            Vector3 tilePlacemenetScale;
            if (prevTile.GetComponent<Tile>().type == TileType.SIDEWAYS)
            {
                tilePlacemenetScale = Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size / 2 + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
            }
            else
            {
                tilePlacemenetScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
            }

            currentTileLocation += tilePlacemenetScale;

            int currentPathLength = Random.Range(minStraightTiles, maxStraightTiles);
            for (int i = 0; i < currentPathLength; ++i)
            {
                if (Random.value > 0.2f)
                {
                    SpawnTile(SelectRandomGameObjectFromList(straights).GetComponent<Tile>(), false);
                }
                    SpawnTile(startingTile.GetComponent<Tile>(), (i == 0) ? false : true);
            }

            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), false);

        }

        private void SpawnTile(Tile tile, bool spawnObstacle)
        {

            Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

            prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
            currentTiles.Add(prevTile);

            //randomly triggers the spawning of obstacles in the binary true or false
            if (spawnObstacle) SpawnObstacle();

            //if your health drops below 100, begin spawning random bandages
            if (curHealth < 100) SpawnPowerUp();

            if (tile.type == TileType.STRAIGHT) {
                currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
            }
            
        }

        //Select a random object from your list
        private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
        {
            if (list.Count == 0) return null;
            return list[Random.Range(0, list.Count)];
        }

        //Initialise a random start position on a straight tile and get the current health and lists of tiles and objects
        private void Start()
        {
            curHealth = curHealthGO.GetComponent<PlayerController>().curHealth;
            currentTiles = new List<GameObject>();
            currentObstacles = new List<GameObject>();

            Random.InitState(System.DateTime.Now.Millisecond);

            for (int i = 0; i < tileStartCount; ++i)
            {
                SpawnTile(startingTile.GetComponent<Tile>(), false);
            }

            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), false);

        }

        //update the health for the spawning of powerups mechanic
        void Update()
        {
            curHealth = curHealthGO.GetComponent<PlayerController>().curHealth;
        }

    }
}

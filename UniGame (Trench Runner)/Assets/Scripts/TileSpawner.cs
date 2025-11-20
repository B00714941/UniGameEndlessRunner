using System.Collections.Generic;
using UnityEngine;

namespace Runner
{
    public class TileSpawner : MonoBehaviour
    {
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

        private Vector3 currentTileLocation = Vector3.zero;
        private Vector3 currentTileDirection = Vector3.forward;
        private GameObject prevTile;

        private List<GameObject> currentTiles;
        private List<GameObject> currentObstacles;

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

        private void SpawnObstacle()
        {
            if (Random.value > 0.2f) return;

            GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
            Quaternion newObjectRotation = obstaclePrefab.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

            GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObjectRotation);
            currentObstacles.Add(obstacle);

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
                if (Random.value > 0.6f)
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

            if (spawnObstacle) SpawnObstacle();

            // (3 , 4 , 5) * (0, 0, 1) => (0, 0, 5)
            if (tile.type == TileType.STRAIGHT) {
                currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
            }
            
        }

        private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
        {
            if (list.Count == 0) return null;
            return list[Random.Range(0, list.Count)];
        }

        private void Start()
        {
            currentTiles = new List<GameObject>();
            currentObstacles = new List<GameObject>();

            Random.InitState(System.DateTime.Now.Millisecond);

            for (int i = 0; i < tileStartCount; ++i)
            {
                SpawnTile(startingTile.GetComponent<Tile>(), false);
            }

            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), false);
            //SpawnTile(turnTiles[0].GetComponent<Tile>(),false);
            //AddNewDirection(Vector3.left);
        }
    }
}

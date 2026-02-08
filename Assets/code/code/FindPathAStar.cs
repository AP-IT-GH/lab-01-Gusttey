using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathMarker {

    public MapLocation location;
    public float G, H, F;
    public GameObject marker;
    public PathMarker parent;

    public PathMarker(MapLocation l, float g, float h, float f, GameObject m, PathMarker p) {

        location = l;
        G = g;
        H = h;
        F = f;
        marker = m;
        parent = p;
    }

    public override bool Equals(object obj) {

        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;
        else
            return location.Equals(((PathMarker)obj).location);
    }

   
}

public class FindPathAStar : MonoBehaviour {

    public Maze maze;
    public Material closedMaterial;
    public Material openMaterial;
    public GameObject start;
    public GameObject end;
    public GameObject pathP;
    public GameObject playerPrefab; // VOEG DIT TOE: Voor de automatische wandeling

    PathMarker startNode;
    PathMarker goalNode;
    PathMarker lastPos;
    bool done = false;
    bool hasStarted = false;

    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();
    List<PathMarker> path = new List<PathMarker>(); // Voor reconstructie

    void RemoveAllMarkers() {

        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");

        foreach (GameObject m in markers) Destroy(m);

        //GameObject goal = GameObject.FindGameObjectWithTag("Goal");
        //GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Destroy(goal);
        //Destroy(player);

        // Verwijder oude speler als die bestaat
        GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
        if (oldPlayer != null) Destroy(oldPlayer);
    }

    void BeginSearch() {
        done = false;
        hasStarted = false;
        path.Clear();
        RemoveAllMarkers();

        List<MapLocation> locations = new List<MapLocation>();

        for (int z = 1; z < maze.depth - 1; ++z) {
            for (int x = 1; x < maze.width - 1; ++x) {

                if (maze.map[x, z] != 1) {
                    locations.Add(new MapLocation(x, z));
                }
            }
        }
        //locations.Shuffle();

        // Start en Goal posities instellen
        Vector3 startLocation = new Vector3(1, 0.5f, 1);
        startNode = new PathMarker(new MapLocation(1, 1),
            0.0f, 0.0f, 0.0f, Instantiate(start, startLocation, Quaternion.identity), null);

        Vector3 endLocation = new Vector3(Random.Range(5, 8), 0.5f, Random.Range(5, 8));
        goalNode = new PathMarker(new MapLocation((int)endLocation.x, (int)endLocation.z),
            0.0f, 0.0f, 0.0f, Instantiate(end, endLocation, Quaternion.identity), null);

        open.Clear();
        closed.Clear();

        open.Add(startNode);
        lastPos = startNode;
    }

    void Search(PathMarker thisNode) {

        if (thisNode.Equals(goalNode)) {

              done = true;
            
            return;
        }

        foreach (MapLocation dir in maze.directions) {

            MapLocation neighbour = dir + thisNode.location;

            if (neighbour.x < 1 || neighbour.x > maze.width || neighbour.z < 1 || neighbour.z > maze.depth) continue;

            if (maze.map[neighbour.x, neighbour.z] == 1) continue;
            if (IsClosed(neighbour)) continue;

            float g = Vector2.Distance(thisNode.location.ToVector(), neighbour.ToVector()) + thisNode.G;
            float h = Vector2.Distance(neighbour.ToVector(), goalNode.location.ToVector());
            float f = g + h;

            GameObject pathBlock = Instantiate(pathP, new Vector3(neighbour.x * maze.scale, 0.0f, neighbour.z * maze.scale), Quaternion.identity);
            pathBlock.tag = "marker";

            if (!UpdateMarker(neighbour, g, h, f, thisNode)) {

                open.Add(new PathMarker(neighbour, g, h, f, pathBlock, thisNode));
            }
        }
        // Sorteren op laagste F waarde
        open = open.OrderBy(p => p.F).ThenBy(p => p.H).ToList<PathMarker>();
        PathMarker pm = (PathMarker)open.ElementAt(0);
        closed.Add(pm);

        open.RemoveAt(0);
        //pm.marker.GetComponent<Renderer>().material = closedMaterial;

        lastPos = pm;
    }

    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt) {

        foreach (PathMarker p in open) {

            if (p.location.Equals(pos)) {

                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = prt;
                return true;
            }
        }
        return false;
    }

    bool IsClosed(MapLocation marker) {

        foreach (PathMarker p in closed) {

            if (p.location.Equals(marker)) return true;
        }
        return false;
    }

    void Start() {

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.P)) {
            BeginSearch();
            // Start direct de zoek-coroutine
            StartCoroutine(SearchStepByStep());
        }
    }

    IEnumerator SearchStepByStep()
    {
        while (!done && open.Count > 0)
        {
            Search(lastPos);
            yield return new WaitForSeconds(0.01f); // Visualiseer het zoekproces
        }

        if (done)
        {
            ReconstructPath(); // Taak 2.1
            StartCoroutine(WalkPath()); // Taak 2.3
        }
    }

    void ReconstructPath()
    {
        path.Clear();
        PathMarker temp = lastPos;
        // Werk terug van Goal naar Start via 'parent'
        while (temp != null)
        {
            path.Insert(0, temp);
            temp = temp.parent;
        }
    }

    IEnumerator WalkPath()
    {
        // Instantiate de speler op de startpositie
        GameObject player = Instantiate(playerPrefab, new Vector3(path[0].location.x, 0.5f, path[0].location.z), Quaternion.identity);
        player.tag = "Player";

        // Loop het pad af: 1 stap per seconde
        foreach (PathMarker step in path)
        {
            player.transform.position = new Vector3(step.location.x, 0.5f, step.location.z);
            yield return new WaitForSeconds(1.0f);
        }
        Debug.Log("Doel bereikt!");
    }
}

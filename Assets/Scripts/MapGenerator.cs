using System.Collections.Generic;
using UnityEngine;
using System;

using AI_Utils;
using Utils;

public class MapGenerator : MonoBehaviour
{
    public int xVal;
    public int yVal;
    private Transform bloc;
    public List<GameObject> blocsPrefab;
    
    [Header("Map Loading")]
    public List<Map> maps;
    
    public int usedMapId;
    public bool useMap;

    public Vector2Int startPosition;
    
    public enum Case
    {
        Empty,
        Start,
        Goal,
        Obstacle,
        Crate,
        TargetCrate,
        CrateOnTarget
    }

    public void Awake()
    {
        xVal = maps[usedMapId].mapSize.x;
        yVal = maps[usedMapId].mapSize.y;
        GameObject cam = GameObject.FindGameObjectsWithTag("MainCamera")[0];
        cam.transform.position = new Vector3(xVal/2, (xVal+yVal)*0.65f , yVal/2);
    }

    public void GenerateMap(ref List<List<Bloc>> mapBlocs, ref Case[,] mapCase, out IntList startState, out int nCrates, out int nTargets)
    {
        startState = new IntList();
        nCrates = 0;
        nTargets = 0;

        string name = "GeneratedMap";
        if (transform.Find(name))
        {
            DestroyImmediate(transform.Find(name).gameObject);
        }

        Transform map = new GameObject(name).transform;
        map.parent = transform;
        
        if (!useMap)
        {
            for (int x = 0; x < xVal; x++)
            {
                mapBlocs.Add(new List<Bloc>());
                for (int y = 0; y < yVal; y++)
                {
                    mapCase[x,y] = Case.Empty;
                    mapBlocs[x].Add(null);
                }
            }
            mapCase[0,0] = Case.Start;//Début
            mapCase[3,3] = Case.Goal;//Fin
            mapCase[1,2] = Case.Obstacle;//Obstacle
            mapCase[2,1] = Case.Obstacle;//Obstacle

            startState.Add(0);
            startState.Add(0);
        }
        else
        {
            startState.Add(0);
            startState.Add(0);

            for (int x = 0; x < xVal; x++)
            {
                mapBlocs.Add(new List<Bloc>());
                for (int y = 0; y < yVal; y++)
                {
                    try
                    {
                        mapCase[x,y] = maps[usedMapId].blocId[x].ligne[y];
                    }
                    catch (Exception e)
                    {
                        mapCase[x,y] = Case.Empty;
                    }
                    
                    mapBlocs[x].Add(null);
                }
            }
        }
        
        
        for (int x = 0;x<xVal;x++)
        {
            for (int y = 0;y<yVal;y++)
            {
                Vector3 tilePos = new Vector3(x, 0, y);
                
                if (mapCase[x, y] == Case.Crate)
                {
                    mapBlocs[x][y] = new BlocCrate();
                    BlocCrate crate = mapBlocs[x][y] as BlocCrate;
                    crate.blocUnderMeGO = blocsPrefab[(int)Case.Empty];
                    crate.blocUnderMe = new Bloc();
                    crate.blocUnderMe.ID = 0;

                    startState.Add(x);
                    startState.Add(y);
                    nCrates++;
                }
                else if(mapCase[x,y] == Case.CrateOnTarget)
                {
                    mapBlocs[x][y] = new BlocCrate();
                    BlocCrate crate = mapBlocs[x][y] as BlocCrate;
                    crate.blocUnderMeGO = blocsPrefab[(int)Case.TargetCrate];
                    crate.onTarget = true;
                    
                    crate.blocUnderMe = new Bloc();
                    crate.blocUnderMe.ID = (int)Case.TargetCrate;

                    startState.Add(x);
                    startState.Add(y);
                    nCrates++;
                    nTargets++;
                }
                else if(mapCase[x, y]== Case.Obstacle)
                {
                    mapBlocs[x][y] = new Bloc();
                    mapBlocs[x][y].wall = true;
                }
                else
                {
                    if(mapCase[x, y] == Case.Start)
                    {
                        startState[0] = x;
                        startState[1] = y;
                        startPosition.x = x;
                        startPosition.y = y;
                    }

                    if(mapCase[x, y] == Case.TargetCrate)
                    {
                        nTargets++;
                    }

                    mapBlocs[x][y] = new Bloc();
                }
                
                if (mapCase[x, y] == Case.CrateOnTarget)
                {
                    mapBlocs[x][y].myGo = blocsPrefab[(int)Case.Crate];
                    mapBlocs[x][y].ID = (int)mapCase[x, y];
                    mapBlocs[x][y].Spawn();
                    mapBlocs[x][y].myGo.transform.position = tilePos;
                    mapBlocs[x][y].myGo.transform.parent = map;
                    BlocCrate crate = mapBlocs[x][y] as BlocCrate;
                    crate.ChangeColor();
                }

                else
                {
                    mapBlocs[x][y].myGo = blocsPrefab[(int)mapCase[x, y]];
                    mapBlocs[x][y].ID = (int)mapCase[x, y];
                    mapBlocs[x][y].Spawn();
                    mapBlocs[x][y].myGo.transform.position = tilePos;
                    mapBlocs[x][y].myGo.transform.parent = map;
                }
            }
        }
    }


    //GenerateStateMap Pseudo code:
    // for x
    //     for y

    //     end for
    // end for

    // for e existing element
    //     listSize <- list.size

    //     for n existing listSize
    //         for i existing pair
    //             if i != pair.size - 1
    //                 currentList <- create newList in list from list[n]
    //             else
    //                 currentList <- list[n]
    //             end

    //             append pair to currentList
    //         end for
    //     end for
    // end for

    // Add endList to mapstate dictionary, do not add forbidden states
    // Generate possible actions per state, in a way so that we cannot get into a forbidden state
    public void GenerateStateMap(ref Dictionary<IntList, State> mapState, ref Case[,] mapCase, int nCrates, int nTargets)
    {
        //Generate all possible pairs
        List<IntList> pairs = new List<IntList>();

        for(int x = 0; x < xVal; x++)
        {
            for(int y = 0; y < yVal; y++)
            {
                if(mapCase[x, y] != Case.Obstacle)
                {
                    pairs.Add(new IntList());

                    pairs[pairs.Count-1].Add(x);
                    pairs[pairs.Count-1].Add(y);
                }
            }
        }

        //Generate all possible keys
        //new List<IntList>(pairs) would copy refernces to our intList in our new list
        List<IntList> keys = new List<IntList>();

        for(int p = 0; p < pairs.Count; p++)
        {
            keys.Add(new IntList(pairs[p]));
        }

        //keys.EnsureCapacity(Mathf.Pow(pairs.Count, nCrates+1)); Not available in unity C# version
        int keysSize;

        for(int n = 0; n < nCrates; n++)
        {
            keysSize = keys.Count;

            for(int keyIndex = 0; keyIndex < keysSize; keyIndex++)
            {
                for(int p = 0; p < pairs.Count; p++)
                {
                    int index = keyIndex;

                    if(p != pairs.Count-1)
                    {
                        keys.Add(new IntList(keys[keyIndex]));
                        index = keys.Count-1;
                    }

                    keys[index].AddRange(new IntList(pairs[p]));
                }
            }
        }

        for(int i = 0; i < keys.Count; i++)
        {
            State newState;

            if(CheckState(keys[i], ref mapCase, out newState, nTargets, nCrates) == true)
            {
                mapState.Add(keys[i], newState);
            }
        }

        foreach(KeyValuePair<IntList, State> kvp in mapState)
        {
            Vector2Int move = Vector2Int.zero;

            AddAction(kvp.Key, new Left(), mapState, mapCase);

            AddAction(kvp.Key, new Right(), mapState, mapCase);

            AddAction(kvp.Key, new Down(), mapState, mapCase);

            AddAction(kvp.Key, new Up(), mapState, mapCase);
        }
    }

    public void AddAction(IntList key, AI_Utils.Action action, Dictionary<IntList, State> mapState, in Case[,] mapCase)
    {
        IntList newKey = action.Act(key, false);

        //Check that everything is in bound of the map
        for(int i = 0; i < key.Count; i+=2)
        {
            if(IsOnMap(newKey[i], newKey[i+1], mapCase) == false)
            {
                return;
            }
        }

        if(mapState.ContainsKey(newKey) == true)
        {
            mapState[key].AddAction(action);
        }
    }

    public bool CheckState(IntList key, ref Case[,] mapCase, out State currentState, int nTargets, int nCrates)
    {
        int blocked = 0;//Number of crates blocked

        int score = 0;//Number of crates on target

        currentState = new StandardState();

        for(int a = 0; a < key.Count; a+=2)
        {
            //Check for overllaping objects
            for(int b = a + 2; b < key.Count; b+=2)
            {
                if(key[a] == key[b] && key[a+1] == key[b+1])
                {
                    return false;
                }
            }


            //Check for objects overllaping with obstacles or targets
            switch(mapCase[key[a], key[a+1]])
            {
                case Case.Obstacle:
                    return false;
                case Case.CrateOnTarget:
                case Case.TargetCrate:
                    if(a > 0)
                    {
                        score++;
                    }
                    break;
                case Case.Goal:
                    if(a == 0)
                    {
                        score++;
                    }
                    
                    break;
                default:
                    //Check for a crate blocked
                    if(a > 0)
                    {
                        int countX = 0;
                        int countY = 0;

                        //Check for crate blocked  by obstacles
                        if(IsBlocked(key[a]+1, key[a+1], ref mapCase) == true) countX++;

                        if(IsBlocked(key[a]-1, key[a+1], ref mapCase) == true) countX++;

                        if(IsBlocked(key[a], key[a+1]-1, ref mapCase) == true) countY++;

                        if(IsBlocked(key[a], key[a+1]+1, ref mapCase) == true) countY++;

                        if(countX > 0 && countY > 0)
                        {
                            blocked++;
                        }else if(countX > 0 || countY > 0)
                        {
                            //Check for crate blocked by another obstacle and crate, herself blocked by current crate and an obstacle
                            for(int b = a + 2; b < key.Count; b+=2)
                            {
                                int count = 0;

                                int x = Mathf.Abs(key[a] - key[b]);
                                int y = Mathf.Abs(key[a+1] - key[b+1]);

                                if(x == 1 && y == 0 && countY > 0)
                                {
                                    if(IsBlocked(key[b], key[b+1]-1, ref mapCase) == true) count++;

                                    if(IsBlocked(key[b], key[b+1]+1, ref mapCase) == true) count++;

                                }else if(x == 0 && y == 1 && countX > 0)
                                {
                                    if(IsBlocked(key[b]+1, key[b+1], ref mapCase) == true) count++;

                                    if(IsBlocked(key[b]-1, key[b+1], ref mapCase) == true) count++;
                                }

                                if(count > 0)
                                {
                                    blocked++;
                                    break;
                                }
                            }
                        }
                    }

                    break;
            }
        }

        if((score >= nTargets && nTargets > 0) || (nTargets == 0 && score == 1))
        {
            currentState = new FinalGoal();
        }else if(blocked > (nCrates - nTargets))
        {
            currentState.final = true;
        }

        return true;
    }

    public bool IsOnMap(int x, int y, in Case[,] mapCase)
    {
        if(x < 0 || x >= mapCase.GetLength(0) || y < 0 || y >= mapCase.GetLength(1))
        {
            return false;
        }

        return true;
    }

    public bool IsBlocked(int x, int y, ref Case[,] mapCase)
    {
        if(IsOnMap(x, y, mapCase) == false)
        {
            return true;
        }

        if(mapCase[x, y] == Case.Obstacle)
        {
            return true;
        }

        return false;
    }
}
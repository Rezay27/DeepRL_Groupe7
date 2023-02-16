using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI_Utils;
using Action = AI_Utils.Action;

public class ValueIteration : MonoBehaviour
{
    public MapGenerator _mapGenerator;
    public List<List<GameObject>> ListeBloc = new List<List<GameObject>>();
    
    public void Start()
    {
        ListeBloc = _mapGenerator.blocs;
        Debug.Log(ListeBloc.Count);
       
    }
    
    // algorithme de Value Iteration
    static public void ValueIterationAlgorithm(ref List<List<State>> mapState)
    {
        float delta = float.MinValue;
        //On boucle pour minimiser la valeur de Delta   
        do
        {
            for (int x = 0; x < mapState.Count; x++)
            {
                for (int y = 0; y < mapState[x].Count; y++)
                {
                    float maxA = -1;
                    int indexActionSelected = -1;
                    Vector2Int nextState = mapState[x][y].actions[mapState[x][y].currentAction].Act(new Vector2Int(x, y));
                    foreach (var actions in mapState[x][y].actions)
                    {
                        float tmp = mapState[x][y].reward + mapState[nextState.x][nextState.y].score * 0.5f;
                        if (maxA < tmp)
                        {
                            maxA = tmp;
                            indexActionSelected = mapState[x][y].actions.IndexOf(actions);
                        }
                    }
                    mapState[x][y].futureScore = maxA;
                    mapState[x][y].currentAction = indexActionSelected;
                    delta = Mathf.Max(delta, mapState[x][y].score - mapState[x][y].futureScore);
                }
            }
        } while (delta <0);
        // On créer le chemin avec les meilleurs actions 
        for(int x = 0; x < mapState.Count; x++)
        {
            for(int y = 0; y < mapState[x].Count; y++)
            {
                int bestAction = 0;
                float bestScore = 0;

                for(int a = 0; a < mapState[x][y].actions.Count; a++)
                {
                    Vector2Int nextState = mapState[x][y].actions[a].Act(new Vector2Int(x, y));

                    float tmp = mapState[x][y].reward + mapState[nextState.x][nextState.y].score * 0.5f; 

                    if(tmp > bestScore)
                    {
                        bestScore = tmp;
                        bestAction = a;
                    }
                }
                mapState[x][y].currentAction = bestAction;
            }
        }
        
    }
}

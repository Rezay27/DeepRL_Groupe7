using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI_Utils;

public class MonteCarlo : MonoBehaviour
{
    static public void Simulation(ref List<List<State>> mapState, int maxTurn, int nEpisode, bool everyVisit, Vector2Int pos, float epsilon)
    {
        //Set to zero
        for(int x = 0; x < mapState.Count; x++)
        {
            for(int y = 0; y < mapState[x].Count; y++)
            {
                for(int a = 0; a < mapState[x][y].actions.Count; a++)
                {
                    mapState[x][y].totalScore[a] = 0;
                    mapState[x][y].timePlayed[a] = 0;
                    mapState[x][y].visited = 0;
                }
            }
        }

        //Episodes loop
        for(int episode = 0; episode < nEpisode; episode++)
        {
            List<Vector2Int> states = new List<Vector2Int>();
            List<int> actionTaken = new List<int>();

            Vector2Int currentState = pos;

            //Generate an episode sequence
            for(int turn = 0; turn < maxTurn; turn++)
            {
                mapState[currentState.x][currentState.y].visited++;
                states.Add(currentState);

                if(mapState[currentState.x][currentState.y].final == true)
                {
                    break;
                }

                //Select an action
                int action = 0;

                //Exploration
                if(Random.Range(0.0f, 1.0f) < epsilon)
                {
                    action = Random.Range(0, mapState[currentState.x][currentState.y].actions.Count);
                    
                }else{//Exploitation
                    action = mapState[currentState.x][currentState.y].currentAction;
                }

                actionTaken.Add(action);

                currentState = mapState[currentState.x][currentState.y].actions[action].Act(currentState);
            }

            //Retropropagation
            float reward = 0;

            for(int i = states.Count-2; i >= 0; i++)
            {
                reward += mapState[states[i+1].x][states[i+1].y].reward;

                if(mapState[states[i].x][states[i].y].visited == 1 || everyVisit == true)
                {
                    mapState[states[i].x][states[i].y].timePlayed[actionTaken[i]]++;//Check state-action pair with teacher
                    mapState[states[i].x][states[i].y].totalScore[actionTaken[i]] += reward;
                }

                mapState[states[i].x][states[i].y].visited--;
            }
        }

        //Update policy loop
        for(int x = 0; x < mapState.Count; x++)
        {
            for(int y = 0; y < mapState[x].Count; y++)
            {
                int bestAction = 0;
                float bestScore = mapState[x][y].totalScore[0] / mapState[x][y].timePlayed[0];

                for(int a = 1; a < mapState[x][y].actions.Count; a++)
                {
                    float tmp = mapState[x][y].totalScore[a] / mapState[x][y].timePlayed[a]; 

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

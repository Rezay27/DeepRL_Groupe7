using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI_Utils;
using Utils;

public class MonteCarlo : MonoBehaviour
{
    static public void Simulation(ref Dictionary<IntList, State> mapState, int maxTurn, float nEpisode, bool everyVisit, 
    float epsilon, bool onPolicy, bool greedyEpsilon)
    {
        float maxEpsilon = epsilon;

        //Set to zero
        foreach(KeyValuePair<IntList, State> kvp in mapState)
        {
            for(int a = 0; a < kvp.Value.actions.Count; a++)
            {
                kvp.Value.totalScore[a] = 0;
                kvp.Value.timePlayed[a] = 0;
            }
        }

        IntList currentState = new IntList();

        //Episodes loop
        for(float episode = 0; episode < nEpisode; episode++)
        {
            if(greedyEpsilon == true)
            {
                epsilon = maxEpsilon * ((nEpisode - episode) / nEpisode);
            }

            //Reset visited count
            foreach(KeyValuePair<IntList, State> kvp in mapState)
            {
                for(int a = 0; a < kvp.Value.actions.Count; a++)
                {
                    kvp.Value.visited[a] = 0;
                }
            }

            List<IntList> states = new List<IntList>();
            List<int> actionTaken = new List<int>();

            Dictionary<IntList, State>.Enumerator it = mapState.GetEnumerator();

            do{
                int index = Random.Range(0, mapState.Count);

                it = mapState.GetEnumerator();
                it.MoveNext();

                for(int i = 0; i < index; i++)
                {
                    it.MoveNext();
                }

            }while(it.Current.Value.final == true || it.Current.Value.actions.Count == 0);

            currentState = it.Current.Key;

            //Generate an episode sequence
            for(int turn = 0; turn < maxTurn; turn++)
            {
                states.Add(currentState);

                if(mapState[currentState].final == true || mapState[currentState].actions.Count == 0)
                {
                    break;
                }

                //Select an action
                int action = 0;

                //Exploration
                if(Random.Range(0.0f, 1.0f) < epsilon)
                {
                    action = Random.Range(0, mapState[currentState].actions.Count);
                    
                }else{//Exploitation
                    action = mapState[currentState].currentAction;
                }

                if(mapState[currentState].actions.Count == 0)
                {
                    Debug.Log(currentState);
                }

                actionTaken.Add(action);
                mapState[currentState].visited[action]++;

                currentState = mapState[currentState].actions[action].Act(currentState);
            }

            //Retropropagation
            float reward = 0;

            for(int i = states.Count-2; i >= 0; i--)
            {
                reward += mapState[states[i+1]].reward;

                if(mapState[states[i]].visited[actionTaken[i]] == 1 || everyVisit == true)
                {
                    mapState[states[i]].timePlayed[actionTaken[i]]++;//Check state-action pair with teacher
                    mapState[states[i]].totalScore[actionTaken[i]] += reward;
                }

                mapState[states[i]].visited[actionTaken[i]]--;
            }

            if(onPolicy == true || episode == (nEpisode-1))
            {
                //Update V(s)
                foreach(KeyValuePair<IntList, State> kvp in mapState)
                {
                    for(int a = 0; a < kvp.Value.actions.Count; a++)
                    {
                        if(kvp.Value.timePlayed[a] == 0)
                        {
                            kvp.Value.vs[a] = 0;
                        }else{
                            kvp.Value.vs[a] = kvp.Value.totalScore[a] / kvp.Value.timePlayed[a]; 
                        }
                    }
                }

                //Update policy loop
                foreach(KeyValuePair<IntList, State> kvp in mapState)
                {
                    if(kvp.Value.vs.Count == 0)
                    {
                        continue;
                    }

                    int bestAction = 0;
                    float bestScore = kvp.Value.vs[0];

                    for(int a = 1; a < kvp.Value.actions.Count; a++)
                    {
                        float tmp = kvp.Value.vs[a]; 

                        if(tmp > bestScore)
                        {
                            bestScore = tmp;
                            bestAction = a;
                        }
                    }

                    kvp.Value.currentAction = bestAction;
                }
            }
        }
    }
}

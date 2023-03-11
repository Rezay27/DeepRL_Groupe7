using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI_Utils;
using Action = AI_Utils.Action;
using Utils;

public class ValueIteration 
{
    // algorithme de Value Iteration
    public static void ValueIterationAlgorithm(ref Dictionary<IntList, State> mapState, float theta, float gamma)
    {
         float delta = 0;

         foreach (var kvp in mapState)
         {
                kvp.Value.score = 0;
         }
         do
         {
             foreach (KeyValuePair<IntList, State> kvp in mapState)
             {
                 if(kvp.Value.final == true || kvp.Value.actions.Count == 0)
                {
                    kvp.Value.futureScore = kvp.Value.reward + kvp.Value.score * gamma;
                }
                else
                {
                    float maxA = -1;
                    int indexActionSelected = -1;

                    IntList nextState = new IntList();

                    for (int a = 0; a < kvp.Value.actions.Count; a++)
                    {
                        nextState = kvp.Value.actions[a].Act(kvp.Key);

                        float tmp = kvp.Value.reward + mapState[nextState].score * gamma;

                        if (maxA < tmp)
                        {
                            maxA = tmp;
                            indexActionSelected = a;
                        }
                    }

                    kvp.Value.futureScore = maxA;
                    kvp.Value.currentAction = indexActionSelected;

                    delta = Mathf.Max(delta, Mathf.Abs(kvp.Value.score - kvp.Value.futureScore));
                }
             }

            foreach (KeyValuePair<IntList, State> kvp in mapState)
             {
                kvp.Value.score = kvp.Value.futureScore;
             }

         } while (delta < theta);
         
         // On créer le chemin avec les meilleurs actions 
         foreach (KeyValuePair<IntList, State> kvp in mapState)
         {

             int bestAction = 0;
             float bestScore = -1;

             for(int a = 0; a < kvp.Value.actions.Count; a++)
             {
                 IntList nextState = kvp.Value.actions[a].Act(kvp.Key);

                 float tmp = kvp.Value.reward + mapState[nextState].score * gamma; 

                 if(tmp > bestScore)
                 {
                     bestScore = tmp;
                     bestAction = a;
                 }
             } 
             
             kvp.Value.currentAction = bestAction;
             //Debug.Log("best action : " + kvp.Value.actions[bestAction].GetId());
         }
    }
}

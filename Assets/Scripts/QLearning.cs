using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI_Utils;
using Utils;

public class QLearning  
{
   public static void Qlearning(ref Dictionary<IntList, State> mapState, float gamma, int nbEpisode, float epsilon,
      float tauxDapprentissage)
   {
      
     //Initialisation des valeurs de Q(s,a) à 0 
      foreach (KeyValuePair<IntList,State> state in mapState)
      { 
         foreach (var action in state.Value.actions)
         {
            action.q = 0;
         }
      }
      
       for (int episode = 0; episode < nbEpisode; episode++)
       {
          IntList xy = new IntList();
          KeyValuePair<IntList, State> curentState = new KeyValuePair<IntList, State>();
          foreach (var kvp in mapState)
          {
             if (kvp.Value.start)
             {
                curentState = kvp;
                xy = kvp.Key;
             }
          }
          
          int currentAction = SARSA.EpsilonGreedy(mapState,xy,epsilon);
          int iteration = 0;


          do
          {
             // On exectue l'action initiale

             IntList nextStateCoord = curentState.Value.actions[currentAction].Act(curentState.Key, false);
             State nextState = mapState[nextStateCoord];


             // On utilise l'algo d'exploration/exploitation 
             xy = nextStateCoord;
             int nextAction = SARSA.EpsilonGreedy(mapState, xy, epsilon);
            
             float currentQ = curentState.Value.actions[currentAction].q;
             float maxnextQ = 0;

             foreach (var action in  nextState.actions)
             {
                maxnextQ = Mathf.Max(maxnextQ, action.q);
             }
             
             float reward = curentState.Value.reward;
             curentState.Value.actions[currentAction].q =
                currentQ + tauxDapprentissage * (reward + gamma * maxnextQ - currentQ);

             mapState[curentState.Key].currentAction = currentAction;
             curentState = new KeyValuePair<IntList, State>(nextStateCoord, nextState);
             currentAction = nextAction;
            
             iteration++;
             
          } while ( iteration <= 100);
       }
       
       foreach (KeyValuePair<IntList, State> kvp in mapState)
       {
          int bestAction = 0;
          float bestScore = -1;

          for(int a = 0; a < kvp.Value.actions.Count; a++)
          {
             float tmp = kvp.Value.actions[a].q ; 

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

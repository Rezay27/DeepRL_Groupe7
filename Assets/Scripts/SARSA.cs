using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI_Utils;

using Utils;

public class SARSA
{
  public static void SarsaAlgorithm(ref Dictionary<IntList, State> mapState ,  float gamma ,int nbEpisode, float epsilon, float tauxDapprentissage)
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
          
          int currentAction = EpsilonGreedy(mapState,xy,epsilon);
          int iteration = 0;


          do
          {
             // On exectue l'action initiale

             IntList nextStateCoord = curentState.Value.actions[currentAction].Act(curentState.Key, false);
             State nextState = mapState[nextStateCoord];


             // On utilise l'algo d'exploration/exploitation 
             xy = nextStateCoord;
             int nextAction = EpsilonGreedy(mapState, xy, epsilon);

             //mise a jour de Q(s,a)

             float currentQ = curentState.Value.actions[currentAction].q;
             //Debug.Log("nextState.actions.count : " + nextState.actions.Count);
             //Debug.Log("nextAction : " + nextAction);
             float nextQ = nextState.actions[nextAction].q;
             float reward = curentState.Value.reward;
             curentState.Value.actions[currentAction].q =
                currentQ + tauxDapprentissage * (reward + gamma * nextQ - currentQ);

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

   // Algorithme d'exploration / exploitation
   public static int EpsilonGreedy(Dictionary<IntList, State> mapState,IntList Xy, float epsilon)
   {
      State actuState = mapState[Xy];
       if (Random.Range(0f, 1f) < epsilon)
       {
          //Exploration
          //Debug.Log("explore");
          return Random.Range(0, actuState.actions.Count);
       }
       else
       {

          // Exploitation
          //Debug.Log("exploit");
          float bestScore = mapState[Xy].actions[0].q;
          int bestAction = 0;

          for (int i = 1; i < actuState.actions.Count; i++)
          {
             IntList nextStateCoordonee = actuState.actions[i].Act(Xy, false);
             State nextState = mapState[nextStateCoordonee];

             if (nextState.actions[nextState.currentAction].q > bestScore)
             {
                bestScore = nextState.actions[nextState.currentAction].q;
                bestAction = i;
             }
          }
          return bestAction;
       }
   }
}

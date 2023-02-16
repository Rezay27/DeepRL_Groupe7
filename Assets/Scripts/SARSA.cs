using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI_Utils;

public class SARSA 
{
   static void SarsaAlgorithm(ref List<List<State>> mapState , float gamma ,int nbEpisode, float epsilon, float tauxDapprentissage)
   {
      for (int episode = 0; episode < nbEpisode; episode++)
      {
         // On recupere la zone initial
         // a changé quand on aura les coordoné de la zone de debut
         int x = 0, y = 0;
         int actionInit = Random.Range(0,mapState[x][y].actions.Count);
         //Initialisation des valeurs de Q(s,a) à 0 
         foreach (var liststate in mapState)
         {
            foreach (var state in liststate)
            {
               foreach (var action in state.actions)
               {
                  action.q = 0f;
               }
            }
         }
         
         while (true)
         {
            // On exectue l'action initiale
            Vector2Int nextState = mapState[x][y].actions[actionInit].Act(new Vector2Int(x, y));
            float reward = mapState[x][y].reward;
            float nextScore = mapState[nextState.x][nextState.y].actions.Count;
            
            // On utilise l'algo d'exploration/exploitation 
            int nextAction = EpsilonGreedy(mapState,mapState[x][y],nextState,epsilon);;
            
            //mise a jour de Q(s,a)

            float currentQ = mapState[x][y].actions[actionInit].q;
            float nextQ = mapState[nextState.x][nextState.y].actions[nextAction].q;
            mapState[x][y].actions[actionInit].q = currentQ + tauxDapprentissage * (reward + gamma * nextQ - currentQ);

            x = nextState.x;
            y = nextState.y;
            actionInit = nextAction;
            // a changé quand on aura les coordoné de la zone de fin
            if (x == 3 && y == 3)
            {
               break;
            }
            
         }
      }
   }

   // Algorithme d'exploration / exploitation
   public static int EpsilonGreedy(List<List<State>> mapState,State state,Vector2Int PosState, float epsilon)
   {
      if (Random.Range(0f, 1f) < epsilon)
      {
         //Exploration
         return Random.Range(0, state.actions.Count);
      }
      else
      {
         // Exploitation
         float bestScore = float.MinValue;
         int bestAction = 0;

         for (int i = 0; i < state.actions.Count; i++)
         {
            Vector2Int nextStateCoordonee = state.actions[i].Act(new Vector2Int(PosState.x, PosState.y));
            State nextState = mapState[nextStateCoordonee.x][nextStateCoordonee.y];

            if (nextState.score > bestScore)
            {
               bestScore = nextState.score;
               bestAction = i;
            }
         }
         return bestAction;
      }
   }
}

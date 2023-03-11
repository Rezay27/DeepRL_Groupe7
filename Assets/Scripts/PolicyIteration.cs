using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI_Utils;
using Utils;

public class PolicyIteration
{
    static void PolicyEvaluation(ref Dictionary<IntList, State> mapState, float theta, float gamma)
    {
        float delta;

        do{
            delta = 0;

            foreach(KeyValuePair<IntList, State> kvp in mapState)
            {
                if(kvp.Value.final == true || kvp.Value.actions.Count == 0)
                {
                    kvp.Value.futureScore = kvp.Value.reward;
                }else{
                    IntList nextState = kvp.Value.actions[kvp.Value.currentAction].Act(kvp.Key, false);

                    kvp.Value.futureScore = kvp.Value.reward + mapState[nextState].score * gamma;
                }

                delta = Mathf.Max(delta, Mathf.Abs(kvp.Value.score - kvp.Value.futureScore));
            }

            foreach(KeyValuePair<IntList, State> kvp in mapState)
            {
                kvp.Value.score = kvp.Value.futureScore;
            }

        }while(delta >= theta);
    }

    static bool PolicyImprovements(ref Dictionary<IntList, State> mapState, float gamma)
    {
        bool stable = true;

        foreach(KeyValuePair<IntList, State> kvp in mapState)
        {
            bool first = true;
            int bestAction = 0;
            float bestScore = 0;

            for(int a = 0; a < kvp.Value.actions.Count; a++)
            {
                IntList nextState = kvp.Value.actions[a].Act(kvp.Key, false);

                float tmp = kvp.Value.reward + mapState[nextState].score * gamma; 

                if(tmp > bestScore || first == true)
                {
                    first = false;
                    bestScore = tmp;
                    bestAction = a;
                }
            }

            if(bestAction != kvp.Value.currentAction)
            {
                stable = false;
            }

            kvp.Value.currentAction = bestAction;
        }

        return stable;
    }

    public static void Iteration(ref Dictionary<IntList, State> mapState, float theta, float gamma)
    {
        do{
            PolicyEvaluation(ref mapState, theta, gamma);

        }while(PolicyImprovements(ref mapState, gamma) == false);
    }
}

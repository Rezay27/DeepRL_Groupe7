using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

namespace AI_Utils
{

    public abstract class State
    {
        public List<Action> actions = new List<Action>();
        public float score = 0;
        public float futureScore = 0;
        public int currentAction = 0;
        public float reward = 0;
        public bool final = false;
        public bool start = false;

        //For MonteCarlo
        public List<int> visited = new List<int>();//Counts the number of time this state was visited in a episode
        public List<float> totalScore = new List<float>();//Score accumulated by exploitation and exploartion pour chaque action
        public List<float> timePlayed = new List<float>();

        public List<float> vs = new List<float>();//V(s)

        public void AddAction(Action a)
        {
            actions.Add(a);
            totalScore.Add(0);
            timePlayed.Add(0);
            visited.Add(0);
            vs.Add(0);
        }

        public abstract string GetId();
        
        //For SARSA

        public MapGenerator.Case typeBlock; 

    }

    public class StandardState: State
    {
        public override string GetId()
        {
            return "StandardState";
        }

        public StandardState()
        {
            currentAction = Random.Range(0, actions.Count);
        }
    }

    public class FinalGoal: StandardState
    {
        public override string GetId()
        {
            return "FinalGoal";
        }

        public FinalGoal()
        {
            reward = 10;
            final = true;
        }
    }

    public abstract class Action
    {
        public abstract string GetId();

        public abstract IntList Act(in IntList id, bool real);
        
        static public IntList Move(in IntList id, Vector2Int dir, bool real)
        {
            IntList newId = new IntList(id);

            newId[0] += dir.x;
            newId[1] += dir.y;

            //Push the crates
            for(int i = 2; i < newId.Count; i+=2)
            {
                
                if(newId[0] == newId[i] && newId[1] == newId[i+1])
                {
                    if(real){
                        BlocCrate crate = GameManager.Instance._mapBlocs[newId[i]][newId[i+1]] as BlocCrate;
                        GameManager.Instance._mapBlocs = crate.move(GameManager.Instance._mapBlocs,new Vector2Int(newId[i],newId[i+1]), dir);
                    }
                    newId[i] += dir.x;
                    newId[i+1] += dir.y;
                }
            }

            return newId;
        }


        //For SARSA

        public float q = 0;
    }

    public class Right: Action
    {
        public override string GetId()
        {
            return "right";
        }

        public override IntList Act(in IntList id, bool real)
        {
            return Move(id, new Vector2Int(1, 0), real);
        }
    }

    public class Left: Action
    {
        public override string GetId()
        {
            return "left";
        }

        public override IntList Act(in IntList id, bool real)
        {
            return Move(id, new Vector2Int(-1, 0), real);
        }
    }

    public class Up: Action
    {
        public override string GetId()
        {
            return "up";
        }

        public override IntList Act(in IntList id, bool real)
        {
            return Move(id, new Vector2Int(0, 1), real);
        }
    }

    public class Down: Action
    {
        public override string GetId()
        {
            return "down";
        }

        public override IntList Act(in IntList id, bool real)
        {
            return Move(id, new Vector2Int(0, -1), real);
        }
    }
}
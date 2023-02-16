using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Utils
{

    public abstract class State
    {
        public List<Action> actions;
        public float score = 0;
        public float futureScore = 0;
        public int currentAction;
        public float reward = 0;
    }

    public class Gridcase: State
    {
        public Gridcase()
        {
            actions.Add(new Right());
            actions.Add(new Left());
            actions.Add(new Down());
            actions.Add(new Up());

            currentAction = Random.Range(0, actions.Count);
        }
    }

    public class FinalCase: Gridcase
    {
        public FinalCase()
        {
            reward = 1;
        }
    }

    public class Wall: Gridcase
    {
        public Wall()
        {
            reward = -100;
        }
    }


    public abstract class Action
    {
        public abstract string GetId();

        public abstract Vector2Int Act(Vector2Int id);
    }

    public class Right: Action
    {
        public override string GetId()
        {
            return "right";
        }

        public override Vector2Int Act(Vector2Int id)
        {
            id.x++;
            return id;
        }
    }

    public class Left: Action
    {
        public override string GetId()
        {
            return "left";
        }

        public override Vector2Int Act(Vector2Int id)
        {
            id.x--;
            return id;
        }
    }

    public class Up: Action
    {
        public override string GetId()
        {
            return "up";
        }

        public override Vector2Int Act(Vector2Int id)
        {
            id.y++;
            return id;
        }
    }

    public class Down: Action
    {
        public override string GetId()
        {
            return "down";
        }

        public override Vector2Int Act(Vector2Int id)
        {
            id.y--;
            return id;
        }
    }

}
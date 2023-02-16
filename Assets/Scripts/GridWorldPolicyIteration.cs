using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI_Utils;

public class GridWorldPolicyIteration : MonoBehaviour
{
    public MapGenerator _mapGenerator;

    List<List<State>> mapState = new List<List<State>>();

    public GridWorldPolicyIteration()
    {

        for(int x = 0; x < _mapGenerator.mapSize.x; x++)
        {
            mapState.Add(new List<State>());

            for(int y = 0; y < _mapGenerator.mapSize.y; y++)
            {
                switch(_mapGenerator.GetBlocId(new Vector2Int(x, y)))
                {
                    case MapGenerator.Case.Empty:
                    case MapGenerator.Case.Start:
                       mapState[x].Add(new Gridcase());
                       break; 

                    case MapGenerator.Case.Goal:
                       mapState[x].Add(new FinalCase());
                       break; 

                    case MapGenerator.Case.Obstacle:
                       mapState[x].Add(new Wall());
                       break; 
                }
            }
        }


    }
}

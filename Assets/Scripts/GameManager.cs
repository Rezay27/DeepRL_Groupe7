using System.Collections.Generic;
using AI_Utils;
using UnityEngine;
using Utils;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private AlgoApply selectedAlgorithm;
    [SerializeField] private float awaitTimer;

    private float _timer;
    
    //AI
    private Dictionary<IntList, State> _mapState;
    private IntList currentState;
    // MAP
    private List<List<Bloc>> _mapBlocs;
    private MapGenerator.Case[,] _map;
    enum AlgoApply
    {
        ValueIterator,
        PolicyIteration,
        Montecarlo,
        Sarsa,
        Qlearning
    }
    
    void Awake()
    {
        _mapState = new Dictionary<IntList, State>();
        _mapBlocs = new List<List<Bloc>>();
    }
    
    void Start()
    {
        int nCrate;
        int nTarget;

        _map = new MapGenerator.Case[mapGenerator.xVal, mapGenerator.yVal];
        mapGenerator.GenerateMap(ref _mapBlocs, ref _map, out currentState, out nCrate, out nTarget);
        mapGenerator.GenerateStateMap(ref _mapState, ref _map, nCrate, nTarget);
        _mapState[currentState].start = true;

        player.Init(new Vector3(mapGenerator.startPosition.x, 1, mapGenerator.startPosition.y));
        
        switch(selectedAlgorithm)
        {
            case AlgoApply.ValueIterator:
                ValueIteration.ValueIterationAlgorithm(ref _mapState, 1000f, 1.001f);
                break;
            case AlgoApply.PolicyIteration:
                PolicyIteration.Iteration(ref _mapState, 0.01f, 0.5f);
                break;
            case AlgoApply.Montecarlo:
                MonteCarlo.Simulation(ref _mapState, 50, 10000, true, 0.4f, true, false);
                break;
            case AlgoApply.Sarsa:
                SARSA.SarsaAlgorithm(ref _mapState,0.5f,10000,0.4f,0.01f);
                break;
            case AlgoApply.Qlearning:
                QLearning.Qlearning(ref _mapState,0.5f,10000,0.4f,0.01f);
                break;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= awaitTimer)
        {
            _timer = 0.0f;
            ApplyStateMap();
        }
    }

    private void ApplyStateMap()
    {
        State current = _mapState[currentState];
        if(current.final == true) return;

        switch(current.actions[current.currentAction].GetId())
        {
            case "right":
                player.Right();
                break;

            case "left":
                player.Left();
                break;

            case "down":
                player.Down();
                break;
                
            case "up":
                player.Up();
                break;
        }

        currentState = current.actions[current.currentAction].Act(currentState);
    }
}

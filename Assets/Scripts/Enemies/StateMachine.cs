using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private RewindAbstract _rewindAbstract;
    private RewindController _rewindManager;

    public BaseState activeState;

    public void Initialize(BaseState intializeState)
    {
        _rewindManager = RewindController.instance;
        _rewindAbstract = GetComponent<RewindAbstract>();

        ChangeState(intializeState);
    }

    void Update()
    {
        if (activeState != null
            && (_rewindAbstract == null || !_rewindManager.IsBeingRewinded))
        {
            activeState.Perform();
        }
    }

    public void ChangeState(BaseState newState)
    {
        if (activeState != null)
        {
            activeState.Exit();
        }

        activeState = newState;

        if (activeState != null)
        {
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<Enemy>();
            activeState.Enter();
        }
    }
}
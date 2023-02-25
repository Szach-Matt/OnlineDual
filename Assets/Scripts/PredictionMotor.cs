
using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

public class PredictionMotor : NetworkBehaviour
{
    #region Types.

    private struct MoveData
    {
        public float Horizonatal;
        public float Vertical;

        public MoveData(float horizontal, float vertical)
        {
            Horizonatal = horizontal;
            Vertical = vertical;
        }
    }
    private struct ReconcileData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;

        public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
        {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
        }
    }

    #endregion

    #region Misc

    public float MoveRate = 30f;
    private Rigidbody _rigidbody;
    private bool _subscribed = false;

    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void SubscribeToTimeManager(bool subscribe)
    {
        if (base.TimeManager == null) return;
        if (subscribe == _subscribed) return;
        _subscribed = subscribe;

        if (subscribe)
        {
            base.TimeManager.OnTick += TimeManager_OnTick;
            base.TimeManager.OnPostTick += TimeManager_OnPostTick;
        }
        else
        {
            base.TimeManager.OnTick -= TimeManager_OnTick;
            base.TimeManager.OnPostTick -= TimeManager_OnPostTick;
        }
    }


    private void OnDestroy()
    {
        SubscribeToTimeManager(false);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        SubscribeToTimeManager(true);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        SubscribeToTimeManager(true);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        SubscribeToTimeManager(false);
    }

    private void TimeManager_OnTick()
    {
        if (base.IsOwner)
        {
            Reconciliation(default, false);
            MoveData data;
            GatherInput(out data);
            Move(data, false);
        }
        if (base.IsServer)
        {
            Move(default, true);
        }
    }

    private void TimeManager_OnPostTick()
    {
        ReconcileData data = new ReconcileData(transform.position, transform.rotation, _rigidbody.velocity, _rigidbody.angularVelocity);
        Reconciliation(data, true);
    }

    private void GatherInput(out MoveData data)
    {
        data = default;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0f && vertical == 0f) return;

        data = new MoveData(horizontal, vertical);
    }

    [Replicate]
    private void Move(MoveData data, bool asServer, bool replaying = false)
    {
        Vector3 force = new Vector3(data.Horizonatal, 0f, data.Vertical) * MoveRate;
        _rigidbody.AddForce(force);
    }

    [Reconcile]
    private void Reconciliation(ReconcileData data, bool asServer)
    {
        transform.position = data.Position;
        transform.rotation = data.Rotation;
        _rigidbody.velocity = data.Velocity;
        _rigidbody.angularVelocity = data.AngularVelocity;
    }
}


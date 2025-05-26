public class RewindHealth : RewindAbstract
{
    HealthAbstract healthAbstract;

    CircularArray<float> trackedHealth;

    protected void TrackHealth()
    {
        if (healthAbstract != null)
        {
            var healthValue = healthAbstract.health;
            trackedHealth.Write(healthValue);
        }
    }
    protected void RestoreHealth(float seconds)
    {
        float value = trackedHealth.GetValue(seconds);

        healthAbstract.SetHealth(value);
    }

    public override void Track()
    {
        if (healthAbstract != null)
        {
            TrackHealth();
        }
    }

    public override void Rewind(float seconds)
    {
        if (healthAbstract != null)
        {
            RestoreHealth(seconds);
        }
    }

    private void Start()
    {
        healthAbstract = GetComponent<HealthAbstract>();
        if (healthAbstract)
        {
            trackedHealth = new CircularArray<float>();
        }

        MainInit();
    }
}

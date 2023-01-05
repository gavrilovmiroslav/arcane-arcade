public interface IUnitPropertyChanged : IReactiveEventInterface
{
    public delegate void HealthChanged(int oldHealth, int newHealth);
    public delegate void PowerChanged(int oldPower, int newPower);

    public event HealthChanged OnHealthChanged;
    public event PowerChanged OnPowerChanged;
}
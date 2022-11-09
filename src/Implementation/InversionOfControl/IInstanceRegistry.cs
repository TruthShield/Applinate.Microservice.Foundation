namespace Applinate
{

    public interface IInstanceRegistry
    {
        object? GetInstance(Type serviceType);

       void RegisterSingleton<TAbstraction, TConcretion>()
            where TAbstraction : class
            where TConcretion : class, TAbstraction;

        void RegisterSingleton<TAbstraction>(Func<TAbstraction> factory)
            where TAbstraction : class;

        void RegisterSingleton<TAbstraction, TConcretion>(Func<TAbstraction, TConcretion> factory)
            where TAbstraction : class
            where TConcretion : class, TAbstraction;

        void RegisterTransient<TAbstraction>(Func<TAbstraction> factory)
            where TAbstraction : class;
    }
}

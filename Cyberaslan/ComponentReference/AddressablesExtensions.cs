namespace CyberAslan
{
    public static class AddressablesExtensions
    {
        public static System.Type GetDerivedComponentType<TComponent>(this ComponentReference<TComponent> reference)
            where TComponent : IComponentReference
        {
            return ComponentReference<TComponent>.GetDerivedReference(reference).DerivedType;
        }
    }
}

using System;

namespace CyberAslan
{
    public interface IComponentReference
    {
        Type DerivedType => GetType();
    }
}
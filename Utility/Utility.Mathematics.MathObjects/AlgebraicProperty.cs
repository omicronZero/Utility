using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Mathematics.MathObjects
{
    //TODO: make serializable
    public struct AlgebraicProperty
    {
        public string Name { get; }
        public int Id { get; }

        public AlgebraicProperty(string name)
        {
            Name = name;
            Id = AlgebraicProperties.Resolve(name);
        }
    }
}

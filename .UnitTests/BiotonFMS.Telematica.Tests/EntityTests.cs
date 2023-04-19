using BioTonFMS.Domain;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiotonFMS.Telematica.Tests
{
    public class EntityTests
    {
        [Fact]
        public void EntityBaseEqOperator_NullEqNull_ShoulBeTrue()
        {
            FuelType? entityA = null;
            FuelType? entityB = null;
            (entityA == entityB).Should().BeTrue();
        }

        [Fact]
        public void EntityBaseNotEqOperator_NotNullObjectNotEqNull_ShoulBeTrue()
        {
            FuelType? entityA = new FuelType();
            (entityA is not null).Should().BeTrue();
        }
    }
}

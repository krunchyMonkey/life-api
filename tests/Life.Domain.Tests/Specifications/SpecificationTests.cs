using FluentAssertions;
using Life.Domain.Specifications;

namespace Life.Domain.Tests.Specifications
{
    public class SpecificationTests
    {
        private class TestEntity
        {
            public int Value { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private class PositiveValueSpecification : Specification<TestEntity>
        {
            public override bool IsSatisfiedBy(TestEntity entity)
            {
                return entity.Value > 0;
            }
        }

        private class EvenValueSpecification : Specification<TestEntity>
        {
            public override bool IsSatisfiedBy(TestEntity entity)
            {
                return entity.Value % 2 == 0;
            }
        }

        private class NameStartsWithASpecification : Specification<TestEntity>
        {
            public override bool IsSatisfiedBy(TestEntity entity)
            {
                return entity.Name.StartsWith("A", StringComparison.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public void Specification_Should_Check_Single_Condition()
        {
            // Arrange
            var positiveSpec = new PositiveValueSpecification();
            var positiveEntity = new TestEntity { Value = 5 };
            var negativeEntity = new TestEntity { Value = -3 };

            // Act & Assert
            positiveSpec.IsSatisfiedBy(positiveEntity).Should().BeTrue();
            positiveSpec.IsSatisfiedBy(negativeEntity).Should().BeFalse();
        }

        [Fact]
        public void And_Specification_Should_Require_Both_Conditions()
        {
            // Arrange
            var positiveSpec = new PositiveValueSpecification();
            var evenSpec = new EvenValueSpecification();
            var combinedSpec = positiveSpec.And(evenSpec);

            var positiveEvenEntity = new TestEntity { Value = 4 }; // Positive and even
            var positiveOddEntity = new TestEntity { Value = 3 };  // Positive but odd
            var negativeEvenEntity = new TestEntity { Value = -2 }; // Even but negative
            var negativeOddEntity = new TestEntity { Value = -3 };  // Neither

            // Act & Assert
            combinedSpec.IsSatisfiedBy(positiveEvenEntity).Should().BeTrue();
            combinedSpec.IsSatisfiedBy(positiveOddEntity).Should().BeFalse();
            combinedSpec.IsSatisfiedBy(negativeEvenEntity).Should().BeFalse();
            combinedSpec.IsSatisfiedBy(negativeOddEntity).Should().BeFalse();
        }

        [Fact]
        public void Or_Specification_Should_Require_At_Least_One_Condition()
        {
            // Arrange
            var positiveSpec = new PositiveValueSpecification();
            var evenSpec = new EvenValueSpecification();
            var combinedSpec = positiveSpec.Or(evenSpec);

            var positiveEvenEntity = new TestEntity { Value = 4 };  // Both conditions
            var positiveOddEntity = new TestEntity { Value = 3 };   // Only positive
            var negativeEvenEntity = new TestEntity { Value = -2 }; // Only even
            var negativeOddEntity = new TestEntity { Value = -3 };  // Neither

            // Act & Assert
            combinedSpec.IsSatisfiedBy(positiveEvenEntity).Should().BeTrue();
            combinedSpec.IsSatisfiedBy(positiveOddEntity).Should().BeTrue();
            combinedSpec.IsSatisfiedBy(negativeEvenEntity).Should().BeTrue();
            combinedSpec.IsSatisfiedBy(negativeOddEntity).Should().BeFalse();
        }

        [Fact]
        public void Not_Specification_Should_Invert_Condition()
        {
            // Arrange
            var positiveSpec = new PositiveValueSpecification();
            var notPositiveSpec = positiveSpec.Not();

            var positiveEntity = new TestEntity { Value = 5 };
            var negativeEntity = new TestEntity { Value = -3 };
            var zeroEntity = new TestEntity { Value = 0 };

            // Act & Assert
            notPositiveSpec.IsSatisfiedBy(positiveEntity).Should().BeFalse();
            notPositiveSpec.IsSatisfiedBy(negativeEntity).Should().BeTrue();
            notPositiveSpec.IsSatisfiedBy(zeroEntity).Should().BeTrue();
        }

        [Fact]
        public void Complex_Specification_Combinations_Should_Work()
        {
            // Arrange
            var positiveSpec = new PositiveValueSpecification();
            var evenSpec = new EvenValueSpecification();
            var nameSpec = new NameStartsWithASpecification();

            // (Positive AND Even) OR NameStartsWithA
            var complexSpec = positiveSpec.And(evenSpec).Or(nameSpec);

            var positiveEvenEntity = new TestEntity { Value = 4, Name = "Bob" };      // Satisfies first part
            var negativeOddEntityWithA = new TestEntity { Value = -3, Name = "Alice" }; // Satisfies second part
            var positiveOddEntity = new TestEntity { Value = 3, Name = "Bob" };       // Satisfies neither
            var negativeEvenEntity = new TestEntity { Value = -2, Name = "Bob" };     // Satisfies neither

            // Act & Assert
            complexSpec.IsSatisfiedBy(positiveEvenEntity).Should().BeTrue();
            complexSpec.IsSatisfiedBy(negativeOddEntityWithA).Should().BeTrue();
            complexSpec.IsSatisfiedBy(positiveOddEntity).Should().BeFalse();
            complexSpec.IsSatisfiedBy(negativeEvenEntity).Should().BeFalse();
        }

        [Fact]
        public void Not_Of_And_Should_Work_Correctly()
        {
            // Arrange
            var positiveSpec = new PositiveValueSpecification();
            var evenSpec = new EvenValueSpecification();

            // NOT (Positive AND Even) = (NOT Positive) OR (NOT Even)
            var notAndSpec = positiveSpec.And(evenSpec).Not();

            var positiveEvenEntity = new TestEntity { Value = 4 };  // Positive AND Even
            var positiveOddEntity = new TestEntity { Value = 3 };   // Positive but NOT Even
            var negativeEvenEntity = new TestEntity { Value = -2 }; // NOT Positive but Even
            var negativeOddEntity = new TestEntity { Value = -3 };  // NOT Positive AND NOT Even

            // Act & Assert
            notAndSpec.IsSatisfiedBy(positiveEvenEntity).Should().BeFalse(); // Only case where both are true
            notAndSpec.IsSatisfiedBy(positiveOddEntity).Should().BeTrue();   // NOT Even
            notAndSpec.IsSatisfiedBy(negativeEvenEntity).Should().BeTrue();  // NOT Positive
            notAndSpec.IsSatisfiedBy(negativeOddEntity).Should().BeTrue();   // NOT Positive AND NOT Even
        }

        [Fact]
        public void Chained_Specifications_Should_Work()
        {
            // Arrange
            var positiveSpec = new PositiveValueSpecification();
            var evenSpec = new EvenValueSpecification();
            var nameSpec = new NameStartsWithASpecification();

            // Positive AND Even AND NameStartsWithA
            var chainedSpec = positiveSpec.And(evenSpec).And(nameSpec);

            var allConditionsEntity = new TestEntity { Value = 4, Name = "Alice" };   // All conditions
            var missingNameEntity = new TestEntity { Value = 4, Name = "Bob" };       // Missing name condition
            var missingEvenEntity = new TestEntity { Value = 3, Name = "Alice" };     // Missing even condition
            var missingPositiveEntity = new TestEntity { Value = -4, Name = "Alice" }; // Missing positive condition

            // Act & Assert
            chainedSpec.IsSatisfiedBy(allConditionsEntity).Should().BeTrue();
            chainedSpec.IsSatisfiedBy(missingNameEntity).Should().BeFalse();
            chainedSpec.IsSatisfiedBy(missingEvenEntity).Should().BeFalse();
            chainedSpec.IsSatisfiedBy(missingPositiveEntity).Should().BeFalse();
        }
    }
}
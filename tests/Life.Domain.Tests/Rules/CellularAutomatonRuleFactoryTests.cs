using FluentAssertions;
using Life.Domain.Rules;
using Life.Domain.Rules.Standard;
using Life.Domain.Rules.Custom;

namespace Life.Domain.Tests.Rules
{
    public class CellularAutomatonRuleFactoryTests
    {
        [Fact]
        public void CreateRules_Should_Create_Conway_Rules()
        {
            // Act
            var rules = CellularAutomatonRuleFactory.CreateRules("conway");

            // Assert
            rules.Should().BeOfType<ConwaysGameOfLifeRules>();
            rules.RuleSetId.Should().Contain("Conway");
        }

        [Fact]
        public void CreateRules_Should_Create_HighLife_Rules()
        {
            // Act
            var rules = CellularAutomatonRuleFactory.CreateRules("highlife");

            // Assert
            rules.Should().BeOfType<HighLifeRules>();
            rules.RuleSetId.Should().Contain("HighLife");
        }

        [Fact]
        public void CreateRules_Should_Create_DayNight_Rules()
        {
            // Act
            var rules = CellularAutomatonRuleFactory.CreateRules("daynight");

            // Assert
            rules.Should().BeOfType<DayAndNightRules>();
            rules.RuleSetId.Should().Contain("DayNight");
        }

        [Fact]
        public void CreateRules_Should_Create_Seeds_Rules()
        {
            // Act
            var rules = CellularAutomatonRuleFactory.CreateRules("seeds");

            // Assert
            rules.Should().BeOfType<SeedsRules>();
            rules.RuleSetId.Should().Contain("Seeds");
        }

        [Fact]
        public void CreateRules_Should_Be_Case_Insensitive()
        {
            // Act & Assert
            var conwayLower = CellularAutomatonRuleFactory.CreateRules("conway");
            var conwayUpper = CellularAutomatonRuleFactory.CreateRules("CONWAY");
            var conwayMixed = CellularAutomatonRuleFactory.CreateRules("Conway");

            conwayLower.Should().BeOfType<ConwaysGameOfLifeRules>();
            conwayUpper.Should().BeOfType<ConwaysGameOfLifeRules>();
            conwayMixed.Should().BeOfType<ConwaysGameOfLifeRules>();
        }

        [Fact]
        public void CreateRules_Should_Throw_For_Unknown_Rule()
        {
            // Act & Assert
            var act = () => CellularAutomatonRuleFactory.CreateRules("unknown");
            
            act.Should().Throw<ArgumentException>()
                .WithMessage("Unknown rule set: unknown*");
        }

        [Fact]
        public void CreateRules_Should_Throw_For_Null_Or_Empty_Name()
        {
            // Act & Assert
            var actNull = () => CellularAutomatonRuleFactory.CreateRules(null!);
            var actEmpty = () => CellularAutomatonRuleFactory.CreateRules("");
            var actWhitespace = () => CellularAutomatonRuleFactory.CreateRules("   ");

            actNull.Should().Throw<ArgumentException>();
            actEmpty.Should().Throw<ArgumentException>();
            actWhitespace.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CreateConwaysRules_Should_Return_Conway_Rules()
        {
            // Act
            var rules = CellularAutomatonRuleFactory.CreateConwaysRules();

            // Assert
            rules.Should().BeOfType<ConwaysGameOfLifeRules>();
            rules.RuleSetId.Should().Contain("Conway");
        }

        [Fact]
        public void CreateCustomRules_Should_Create_Custom_Rules()
        {
            // Arrange
            var birthCounts = new[] { 3, 6 };
            var survivalCounts = new[] { 2, 3 };

            // Act
            var rules = CellularAutomatonRuleFactory.CreateCustomRules(birthCounts, survivalCounts);

            // Assert
            rules.Should().BeOfType<CustomRules>();
            rules.RuleSetId.Should().Contain("B36");
            rules.RuleSetId.Should().Contain("S23");
        }

        [Fact]
        public void CreateCustomRules_Should_Accept_Custom_Id_And_Description()
        {
            // Arrange
            var birthCounts = new[] { 1 };
            var survivalCounts = new[] { 1, 2 };
            var customId = "MyCustomRule";
            var description = "My custom description";

            // Act
            var rules = CellularAutomatonRuleFactory.CreateCustomRules(birthCounts, survivalCounts, customId, description);

            // Assert
            rules.RuleSetId.Should().Be(customId);
            rules.GetRuleDescription().Should().Be(description);
        }

        [Fact]
        public void CreateFromNotation_Should_Parse_Conway_Notation()
        {
            // Act
            var rules = CellularAutomatonRuleFactory.CreateFromNotation("B3/S23");

            // Assert
            rules.Should().BeOfType<CustomRules>();
            rules.RuleSetId.Should().Contain("B3");
            rules.RuleSetId.Should().Contain("S23");
        }

        [Fact]
        public void CreateFromNotation_Should_Parse_HighLife_Notation()
        {
            // Act
            var rules = CellularAutomatonRuleFactory.CreateFromNotation("B36/S23");

            // Assert
            rules.Should().BeOfType<CustomRules>();
            rules.RuleSetId.Should().Contain("B36");
            rules.RuleSetId.Should().Contain("S23");
        }

        [Fact]
        public void CreateFromNotation_Should_Parse_Complex_Notation()
        {
            // Act
            var rules = CellularAutomatonRuleFactory.CreateFromNotation("B3678/S34678");

            // Assert
            rules.Should().BeOfType<CustomRules>();
            rules.RuleSetId.Should().Contain("B3678");
            rules.RuleSetId.Should().Contain("S34678");
        }

        [Fact]
        public void CreateFromNotation_Should_Handle_Empty_Survival()
        {
            // Act
            var rules = CellularAutomatonRuleFactory.CreateFromNotation("B2/S");

            // Assert
            rules.Should().BeOfType<CustomRules>();
            rules.RuleSetId.Should().Contain("B2");
            rules.RuleSetId.Should().Contain("S");
        }

        [Fact]
        public void CreateFromNotation_Should_Handle_Empty_Birth()
        {
            // Act
            var rules = CellularAutomatonRuleFactory.CreateFromNotation("B/S23");

            // Assert
            rules.Should().BeOfType<CustomRules>();
            rules.RuleSetId.Should().Contain("B");
            rules.RuleSetId.Should().Contain("S23");
        }

        [Fact]
        public void CreateFromNotation_Should_Be_Case_Insensitive()
        {
            // Act
            var rules1 = CellularAutomatonRuleFactory.CreateFromNotation("b3/s23");
            var rules2 = CellularAutomatonRuleFactory.CreateFromNotation("B3/S23");

            // Assert
            rules1.Should().BeOfType<CustomRules>();
            rules2.Should().BeOfType<CustomRules>();
            rules1.RuleSetId.Should().Be(rules2.RuleSetId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("B3")]
        [InlineData("S23")]
        [InlineData("B3S23")]
        [InlineData("B3/")]
        [InlineData("/S23")]
        [InlineData("3/S23")]
        [InlineData("B3/23")]
        public void CreateFromNotation_Should_Throw_For_Invalid_Notation(string invalidNotation)
        {
            // Act & Assert
            var act = () => CellularAutomatonRuleFactory.CreateFromNotation(invalidNotation);
            
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetAvailableRules_Should_Return_All_Predefined_Rules()
        {
            // Act
            var availableRules = CellularAutomatonRuleFactory.GetAvailableRules().ToList();

            // Assert
            availableRules.Should().Contain("conway");
            availableRules.Should().Contain("highlife");
            availableRules.Should().Contain("daynight");
            availableRules.Should().Contain("seeds");
            availableRules.Should().HaveCountGreaterThanOrEqualTo(4);
        }

        [Fact]
        public void GetAvailableRules_Should_Return_Sorted_List()
        {
            // Act
            var availableRules = CellularAutomatonRuleFactory.GetAvailableRules().ToList();

            // Assert
            availableRules.Should().BeInAscendingOrder();
        }

        [Fact]
        public void GetAvailableRuleInfo_Should_Return_Rule_Information()
        {
            // Act
            var ruleInfo = CellularAutomatonRuleFactory.GetAvailableRuleInfo().ToList();

            // Assert
            ruleInfo.Should().NotBeEmpty();
            ruleInfo.Should().Contain(info => info.Name == "conway");
            
            var conwayInfo = ruleInfo.First(info => info.Name == "conway");
            conwayInfo.Description.Should().NotBeNullOrEmpty();
            conwayInfo.Description.Should().Contain("Conway");
        }

        [Fact]
        public void RegisterCustomRule_Should_Add_New_Rule()
        {
            // Arrange
            var customRuleName = "testcustom";
            Func<ICellularAutomatonRules> ruleCreator = () => new ConwaysGameOfLifeRules();

            try
            {
                // Act
                CellularAutomatonRuleFactory.RegisterCustomRule(customRuleName, ruleCreator);

                // Assert
                var availableRules = CellularAutomatonRuleFactory.GetAvailableRules();
                availableRules.Should().Contain(customRuleName);
                
                var createdRule = CellularAutomatonRuleFactory.CreateRules(customRuleName);
                createdRule.Should().BeOfType<ConwaysGameOfLifeRules>();
            }
            finally
            {
                // Cleanup - Note: In real tests, you'd want a more robust cleanup mechanism
                // This is a limitation of the static factory approach
            }
        }

        [Fact]
        public void RegisterCustomRule_Should_Throw_For_Null_Name()
        {
            // Act & Assert
            var act = () => CellularAutomatonRuleFactory.RegisterCustomRule(null!, () => new ConwaysGameOfLifeRules());
            
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void RegisterCustomRule_Should_Throw_For_Null_Creator()
        {
            // Act & Assert
            var act = () => CellularAutomatonRuleFactory.RegisterCustomRule("test", null!);
            
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
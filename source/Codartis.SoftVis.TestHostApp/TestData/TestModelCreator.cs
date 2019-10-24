using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal static class TestModelCreator
    {
        internal static void Create(ITestModelService testModelService)
        {
            var modelBuilder = new TestModelBuilder(testModelService);

            modelBuilder

                .AddClass("W1")
                .AddClass("W2")
                .AddClass("W3")
                .AddProperty("W1", "P1", "W2")
                .AddProperty("W1", "P2_WithLongName", "W2")
                .AddProperty("W2", "P3", "W3")
                .AddProperty("W3", "P4", "W1")
                .EndGroup()

                // Connector goes out of the rect union of source and target (on the right side)
                .AddInterface("R1")
                .AddInterface("R2", "R1")
                .AddInterface("R4")
                .AddInterface("R5", "R4")
                .AddInterface("R6", "R2")
                .AddBase("R6", "R4")
                .EndGroup()

                // Connector goes out of the rect union of source and target (on the left side)
                .AddInterface("L1")
                .AddInterface("L2", "L1")
                .AddInterface("L0", "L1")
                .AddInterface("L4")
                .AddInterface("L5", "L4")
                .AddBase("L0", "L5")
                .EndGroup()

                // Single node
                .AddClass("1", isAbstract: true)
                .EndGroup()

                // Single connector
                .AddClass("3", "1")
                .EndGroup()

                // Siblings added (left, right, middle)
                .AddClass("2", "1")
                .AddClass("5", "1")
                .AddClass("4", "1")
                .AddClass("z1azgd uzgwzdu", "1")
                .AddClass("z2wedwnebbiw", "1")
                .AddClass("z3sahbahs,.sjd.wed", "1")
                .AddClass("z4", "1")
                .AddClass("z5", "1")
                .AddClass("z6", "1")
                .AddClass("z7", "1")
                .AddClass("z8", "1")
                .AddClass("z11wedhwbehhwebfqweuvufvwuvftw", "1")
                .AddClass("z21", "1")
                .AddClass("z3134456674566723456475634t4556", "1")
                .AddClass("z41", "1")
                .AddClass("z51", "1")
                .AddClass("z61", "1")
                .AddClass("z71", "1")
                .AddClass("z123456", "1")
                .EndGroup()

                // Tree moves under parent
                .AddClass("0")
                .AddBase("1", "0")
                .EndGroup()

                // Placing child with no siblings
                .AddClass("10")
                .AddInterface("11")
                .AddInterface("12", "11")
                .AddClass("13", "10")
                .EndGroup()

                // Dummy vertex added
                .AddInterface("14")
                .AddInterface("15")
                .AddBase("14", "15")
                .EndGroup()

                // Dummy vertex removed
                .AddInterface("16")
                .AddBase("15", "16")
                .EndGroup()

                // Redundant edge removed
                .AddInterface("20")
                .AddBase("12", "20")
                .AddBase("20", "11")
                .AddInterface("21-sdhfiwgzgwqe")
                .AddBase("21-sdhfiwgzgwqe", "11")
                .AddInterface("22-wzeuzwe")
                .AddBase("22-wzeuzwe", "11")
                .AddBase("14", "11")
                .EndGroup()

                // Path with 2 new dummy vertices
                .AddInterface("17")
                .AddBase("14", "17")
                .EndGroup()

                // Regression test: layer-assignment of dummy vertices should be finished before positioning, 
                // otherwise it causes an infinite cycle in the layout logic
                .AddInterface("A1")
                .AddInterface("A2", "A1")
                .AddInterface("A3", "A2")
                .AddInterface("A4", "A2")
                .AddInterface("A5", "A2")
                .AddInterface("A6", "A5")
                .AddClass("C1")
                .AddClass("C2", "C1")
                .AddClass("C3", "C1")
                .AddImplements("C1", "A3")
                .AddClass("C0")
                .AddBase("C1", "C0")
                .EndGroup()

                .AddImplements("C1", "A1")
                .AddImplements("C1", "A2")
                .AddImplements("C1", "A4")
                .AddImplements("C1", "A5")
                .AddImplements("C1", "A6")

                // Gap removal
                .AddClass("G1")
                .AddClass("G2", "G1")
                .AddClass("G3", "G1")
                .AddClass("G4", "G3")
                .EndGroup()

                // Overlap removal
                .AddClass("O1")
                .AddClass("O2", "O1")
                .AddInterface("O3")
                .AddInterface("O4", "O3")
                .AddInterface("O5", "O3")
                .AddInterface("O6", "O4")
                .AddInterface("O7", "O5")
                .AddInterface("O8", "O5")
                .AddInterface("O9", "O7")
                .AddInterface("O10", "O8")
                .AddImplements("O2", "O6")
                .AddImplements("O2", "O9")
                .AddImplements("O2", "O10")

                ;
        }
    }
}

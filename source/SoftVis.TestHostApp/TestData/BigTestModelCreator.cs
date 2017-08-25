using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal static class BigTestModelCreator
    {
        internal static TestModelStore Create(int childCount, int levels)
        {
            var initialModel = new TestModel();
            var modelStore = new TestModelStore(initialModel);
            var modelBuilder = new TestModelBuilder(modelStore);

            var rootName = "0";
            modelBuilder.AddClass(rootName);
            CreateChildren(modelBuilder, rootName, childCount, 1, levels);

            return modelStore;
        }

        private static void CreateChildren(TestModelBuilder modelBuilder, string parentName, 
            int childCount, int currentLevel, int maxLevel)
        {
            if (currentLevel == maxLevel)
                return;

            for (var i = 0; i < childCount; i++)
            {
                var nodeName = $"{parentName}-{i}";
                modelBuilder.AddClass(nodeName, parentName);
                CreateChildren(modelBuilder, nodeName, childCount, currentLevel + 1, maxLevel);
            }
        }
    }
}

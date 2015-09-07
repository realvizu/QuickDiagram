using System;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A UmlModel is a special UmlPackage, the model root.
    /// </summary>
    public class UmlModel : UmlPackage
    {
        public override string Name
        {
            get { return "ModelRoot"; }
            set { throw new InvalidOperationException(); }
        }
    }
}

using Codartis.SoftVis.VisualStudioIntegration.App;
using Microsoft.VisualStudio.Shell.Interop;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Translates from Microsoft.VisualStudio.Shell.Interop.VSFRAMEMODE and VSFRAMEMODE2 
    /// to Codartis.SoftVis.VisualStudioIntegration.App.FrameMode.
    /// </summary>
    public static class FrameModeTranslator
    {
        public static FrameMode Translate(object nativeFrameMode)
        {
            var frameMode = FromVsframemode((VSFRAMEMODE)nativeFrameMode);
            if (frameMode != FrameMode.Unknown)
                return frameMode;

            return FromVsframemode2((VSFRAMEMODE2) nativeFrameMode);
        }

        private static FrameMode FromVsframemode(VSFRAMEMODE frameMode)
        {
            switch (frameMode)
            {
                case VSFRAMEMODE.VSFM_Dock:
                    return FrameMode.Docked;
                case VSFRAMEMODE.VSFM_Float:
                case VSFRAMEMODE.VSFM_FloatOnly:
                    return FrameMode.Floating;
                case VSFRAMEMODE.VSFM_MdiChild:
                    return FrameMode.MdiChild;
                default:
                    return FrameMode.Unknown;
            }
        }

        private static FrameMode FromVsframemode2(VSFRAMEMODE2 frameMode)
        {
            switch (frameMode)
            {
                case VSFRAMEMODE2.VSFM_AutoHide:
                    return FrameMode.AutoHide;
                default:
                    return FrameMode.Unknown;
            }
        }
    }
}
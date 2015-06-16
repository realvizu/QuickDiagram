using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    internal class TranslateChangedEventArgs : EventArgs
    {
        public Vector NewTranslate { get; private set; }

        public TranslateChangedEventArgs(Vector newTranslate)
        {
            NewTranslate = newTranslate;
        }
    }
}

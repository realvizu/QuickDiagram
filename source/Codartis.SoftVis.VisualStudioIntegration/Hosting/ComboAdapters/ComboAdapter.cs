using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Codartis.SoftVis.VisualStudioIntegration.App.Selectors;
using Microsoft.VisualStudio.Shell;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting.ComboAdapters
{
    /// <summary>
    /// Abstract base class for adapters that translate between an ISelector and a VS ComboBox.
    /// </summary>
    /// <typeparam name="T">The type of selector's items.</typeparam>
    internal abstract class ComboAdapter<T> : IComboAdapter
    {
        protected readonly ISelector<T> Selector;

        protected ComboAdapter(ISelector<T> selector)
        {
            Selector = selector;
        }

        public void GetItemsCommandHandler(object sender, EventArgs e)
        {
            var comboItems = GetComboItems();
            FillCombo((OleMenuCmdEventArgs)e, comboItems);
        }

        public void ComboCommandHandler(object sender, EventArgs e)
        {
            var oleMenuCmdEventArgs = (OleMenuCmdEventArgs)e;

            switch (GetComboCommandType(oleMenuCmdEventArgs))
            {
                case ComboCommandType.CurrentItemRequested:
                    var selectedItem = GetSelectedItem();
                    SetCurrentComboItem(oleMenuCmdEventArgs, selectedItem);
                    break;

                case ComboCommandType.SelectedItemChanged:
                    var selectedString = GetSelectedComboItem(oleMenuCmdEventArgs);
                    OnSelectedItemChanged(selectedString);
                    break;
            }
        }

        protected abstract IEnumerable<string> GetComboItems();
        protected abstract string GetSelectedItem();
        protected abstract void OnSelectedItemChanged(string item);

        private static void FillCombo(OleMenuCmdEventArgs oleMenuCmdEventArgs, IEnumerable<string> items)
        {
            var outValue = oleMenuCmdEventArgs.OutValue;
            if (outValue != IntPtr.Zero)
                Marshal.GetNativeVariantForObject(items, outValue);
        }

        private static ComboCommandType GetComboCommandType(EventArgs e)
        {
            var oleMenuCmdEventArgs = (OleMenuCmdEventArgs)e;

            if (IsCurrentValueRequested(oleMenuCmdEventArgs))
                return ComboCommandType.CurrentItemRequested;

            if (IsSelectedValueModified(oleMenuCmdEventArgs))
                return ComboCommandType.SelectedItemChanged;

            return ComboCommandType.Unknown;
        }

        private static bool IsCurrentValueRequested(OleMenuCmdEventArgs oleMenuCmdEventArgs)
        {
            return oleMenuCmdEventArgs.OutValue != IntPtr.Zero;
        }

        private static bool IsSelectedValueModified(OleMenuCmdEventArgs oleMenuCmdEventArgs)
        {
            return oleMenuCmdEventArgs.InValue != null;
        }

        private static void SetCurrentComboItem(OleMenuCmdEventArgs oleMenuCmdEventArgs, string item)
        {
            Marshal.GetNativeVariantForObject(item, oleMenuCmdEventArgs.OutValue);
        }

        private static string GetSelectedComboItem(OleMenuCmdEventArgs oleMenuCmdEventArgs)
        {
            return Convert.ToString(oleMenuCmdEventArgs.InValue);
        }
    }
}

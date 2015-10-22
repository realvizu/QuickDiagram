using System;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking.Implementation
{
    /// <summary>
    /// An action of a layout logic run.
    /// </summary>
    internal class LayoutAction : ILayoutAction
    {
        public string Action { get; }
        public double? Amount { get; }

        public LayoutAction(string action, double? amount = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            Action = action;
            Amount = amount;
        }

        public virtual string SubjectName => string.Empty;

        private bool Equals(LayoutAction other)
        {
            return string.Equals(Action, other.Action) && Amount.Equals(other.Amount);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LayoutAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Action != null ? Action.GetHashCode() : 0)*397) ^ Amount.GetHashCode();
            }
        }

        public static bool operator ==(LayoutAction left, LayoutAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LayoutAction left, LayoutAction right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            var amountString = Amount == null ? string.Empty : $"{Amount}";
            var separator = string.IsNullOrEmpty(SubjectName) || string.IsNullOrEmpty(amountString) ? "" : ",";
            return $"{Action} ({SubjectName}{separator}{amountString})";
        }
    }
}
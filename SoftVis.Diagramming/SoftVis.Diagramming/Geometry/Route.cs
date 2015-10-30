using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// A route is a series of points in 2D space.
    /// </summary>
    /// <remarks>
    /// The route makes sure that all consecutive points are different.
    /// If the same point is added consecutively multiple times then it gets added only once.
    /// </remarks>
    public class Route : IEnumerable<Point2D>
    {
        private readonly List<Point2D> _routePoints;

        public Route(IEnumerable<Point2D> routePoints = null)
        {
            _routePoints = new List<Point2D>();
            Add(routePoints);
        }

        public IEnumerator<Point2D> GetEnumerator()
        {
            return _routePoints.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Point2D? point)
        {
            if (point.HasValue && IsNewPointInRoute(point.Value))
                _routePoints.Add(point.Value);
        }

        public void Add(IEnumerable<Point2D> points)
        {
            if (points == null)
                return;

            foreach (var point in points)
                Add(point);
        }

        private bool IsNewPointInRoute(Point2D point)
        {
            return !_routePoints.Any() || point != _routePoints.Last();
        }

        protected bool Equals(Route other)
        {
            return _routePoints != null && _routePoints.SequenceEqual(other._routePoints);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Route) obj);
        }

        public override int GetHashCode()
        {
            return (_routePoints != null ? _routePoints.GetHashCode() : 0);
        }

        public static bool operator ==(Route left, Route right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Route left, Route right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"[{_routePoints.ToDelimitedString("->")}]";
        }
    }
}

namespace Codartis.Util
{
    /// <summary>
    /// The result of a GetOrAdd-type operation.
    /// </summary>
    /// <typeparam name="T">The type of the result (found or added) object.</typeparam>
    public struct GetOrAddResult<T>
    {
        public T Result { get; }
        public GetOrAddAction Action { get; }

        public GetOrAddResult(T result, GetOrAddAction action)
        {
            Result = result;
            Action = action;
        }

        public bool IsGet => Action == GetOrAddAction.Get;
        public bool IsAdd => Action == GetOrAddAction.Add;
    }
}

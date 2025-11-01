namespace KarenKrill.Utilities
{
    public static class UnityObjectExtensions
    {
        /// <summary>
        /// Determines whether an object is null/destroyed or not
        /// </summary>
        /// <remarks>
        /// Returns <see langword="false"/> if the object is queued for destroying but has not yet been destroyed.
        /// </remarks>
        public static bool IsNullOrDestroyed(this UnityEngine.Object unityObject)
        {
            return ReferenceEquals(unityObject, null) || unityObject == null;
        }
    }
}

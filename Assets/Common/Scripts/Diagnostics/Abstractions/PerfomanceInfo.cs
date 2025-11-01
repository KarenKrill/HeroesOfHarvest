namespace KarenKrill.Diagnostics.Abstractions
{
    public readonly struct PerfomanceInfo
    {
        public float Fps { get; }
        public float FpsAverage { get; }
        public float FpsMin { get; }
        public float FpsMax { get; }

        public PerfomanceInfo(float fps, float fpsAverage, float fpsMin, float fpsMax)
        {
            Fps = fps;
            FpsAverage = fpsAverage;
            FpsMin = fpsMin;
            FpsMax = fpsMax;
        }
    }
}

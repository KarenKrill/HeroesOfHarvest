using KarenKrill.UniCore.Diagnostics.Abstractions;
using KarenKrill.UniCore.UI.Presenters.Abstractions;
using KarenKrill.UniCore.UI.Views.Abstractions;

namespace HeroesOfHarvest.UI.Presenters
{
    using Views.Abstractions;

    public class DiagnosticInfoPresenter : PresenterBase<IDiagnosticInfoView>, IPresenter<IDiagnosticInfoView>
    {
        public DiagnosticInfoPresenter(IViewFactory viewFactory,
            IPresenterNavigator navigator,
            IDiagnosticsProvider diagnosticsProvider) : base(viewFactory, navigator)
        {
            _diagnosticsProvider = diagnosticsProvider;
        }
        protected override void Subscribe()
        {
            OnPerfomanceInfoChanged(_diagnosticsProvider.PerfomanceInfo);
            _diagnosticsProvider.PerfomanceInfoChanged += OnPerfomanceInfoChanged;
        }
        protected override void Unsubscribe()
        {
            _diagnosticsProvider.PerfomanceInfoChanged -= OnPerfomanceInfoChanged;
        }

        private readonly IDiagnosticsProvider _diagnosticsProvider;
        private void OnPerfomanceInfoChanged(PerfomanceInfo perfomanceInfo)
        {
            View.FpsText = $"Fps: {perfomanceInfo.FpsAverage:0.0}";
        }
    }
}
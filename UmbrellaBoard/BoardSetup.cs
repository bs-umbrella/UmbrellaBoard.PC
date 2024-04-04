using System;
using Zenject;
using SiraUtil.Logging;

namespace UmbrellaBoard
{
    internal class BoardSetup : IInitializable, IDisposable
    {
        [Inject]
        private MainFlowCoordinator _mainFlowCoordinator;
        [Inject]
        private BoardViewController _boardView;
        [Inject]
        private SiraLog _log;
        public void Initialize()
        {
            _log.Info("Initializing board values");
            _mainFlowCoordinator._providedRightScreenViewController = _boardView;
            _mainFlowCoordinator._rightScreenViewController = _boardView;
        }

        public void Dispose() => _mainFlowCoordinator._providedRightScreenViewController = null;
    }
}

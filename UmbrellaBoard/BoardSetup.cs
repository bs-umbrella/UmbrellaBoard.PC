using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zenject;

namespace UmbrellaBoard
{
    internal class BoardSetup : IInitializable, IDisposable
    {
        private MainFlowCoordinator _mainFlowCoordinator;
        private BoardViewController _boardView;

        [Inject]
        private void Construct(MainFlowCoordinator mainFlowCoordinator, BoardViewController boardView)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _boardView = boardView;
        }

        public void Initialize()
        {
            _mainFlowCoordinator._providedRightScreenViewController = _boardView;
            _mainFlowCoordinator._rightScreenViewController = _boardView;
        }

        public void Dispose() => _mainFlowCoordinator._providedRightScreenViewController = null;
    }
}

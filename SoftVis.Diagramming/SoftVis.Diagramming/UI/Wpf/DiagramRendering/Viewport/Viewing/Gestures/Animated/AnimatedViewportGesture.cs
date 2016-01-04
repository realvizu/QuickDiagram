using System;
using System.Collections.Generic;
using System.Windows;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Viewport.Viewing.Gestures.Animated
{
    /// <summary>
    /// Smooths out the viewport manipulation commands by sending intermediate commands.
    /// </summary>
    internal class AnimatedViewportGesture : IViewportGesture
    {
        public event ViewportCommandHandler ViewportCommand;

        private readonly IViewportGesture _originalGesture;
        private readonly Duration _animationDuration;
        private readonly List<ViewportCommandAnimatorBase> _commandAnimators = new List<ViewportCommandAnimatorBase>();

        internal AnimatedViewportGesture(IViewportGesture originalGesture, TimeSpan animationTimeSpan)
        {
            _originalGesture = originalGesture;
            _originalGesture.ViewportCommand += OnViewportCommand;
            _animationDuration = new Duration(animationTimeSpan);
        }

        public IDiagramViewport DiagramViewport => _originalGesture.DiagramViewport;

        private void OnViewportCommand(object sender, ViewportCommandBase command)
        {
            var commandAnimator = CreateCommandAnimator(command);
            commandAnimator.ViewportCommand += ViewportCommand;
            commandAnimator.BeginAnimation();
            _commandAnimators.Add(commandAnimator);
        }

        private ViewportCommandAnimatorBase CreateCommandAnimator(ViewportCommandBase command)
        {
            if (command is ZoomViewportCommand)
                return new ZoomViewportCommandAnimator(_originalGesture, _animationDuration, 
                    command as ZoomViewportCommand);

            if (command is ResizeViewportCommand)
                return new ResizeViewportCommandAnimator(_originalGesture, _animationDuration, 
                    command as ResizeViewportCommand);

            if (command is MoveViewportCenterInDiagramSpaceCommand)
                return new MoveViewportCenterInDiagramSpaceCommandAnimator(_originalGesture, _animationDuration, 
                    command as MoveViewportCenterInDiagramSpaceCommand);

            if (command is ZoomViewportWithCenterInScreenSpaceCommand)
                return new ZoomViewportWithCenterInScreenSpaceCommandAnimator(_originalGesture, _animationDuration, 
                    command as ZoomViewportWithCenterInScreenSpaceCommand);

            throw new Exception($"Unexpected command type {command.GetType()}");
        }
    }
}

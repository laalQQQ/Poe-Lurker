﻿//-----------------------------------------------------------------------
// <copyright file="StashTabGridViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Services;

    /// <summary>
    /// Represents the stash tab grid.
    /// #MagicNumbersLand.
    /// </summary>
    internal class StashTabGridViewModel : PoeOverlayBase, IDisposable
    {
        #region Fields

        private static readonly int DefaultSize = 637;
        private static readonly int DefaultTabHeight = 1119;

        private static readonly int DefaultLeftMargin = 17;
        private static readonly int DefaultTopMargin = 154;

        private StashTabService _service;

        private int _top;
        private int _left;
        private bool _isRegularTab;
        private bool _isInFolder;
        private bool _isVisible;
        private string _currentTabName;
        private PoeWindowInformation _currentWindowInformation;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="StashTabGridViewModel" /> class.
        /// </summary>
        /// <param name="stashTabService">The stash tab service.</param>
        /// <param name="windowManager">The window manage.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="settingsService">the settings service.</param>
        public StashTabGridViewModel(StashTabService stashTabService, IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._isRegularTab = true;
            this._service = stashTabService;
            this._service.NewMarkerRequested += this.Service_NewMarkerRequested;
            this._service.CloseRequested += this.Service_CloseRequested;
        }

        #region Properties

        /// <summary>
        /// Gets Top.
        /// </summary>
        public int Top
        {
            get
            {
                return this._top;
            }

            private set
            {
                this._top = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets Left.
        /// </summary>
        public int Left
        {
            get
            {
                return this._left;
            }

            private set
            {
                this._left = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsVisible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return this._isVisible;
            }

            private set
            {
                this._isVisible = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsRegularTab.
        /// </summary>
        public bool IsRegularTab
        {
            get
            {
                return this._isRegularTab;
            }

            private set
            {
                this._isRegularTab = value;
                this.NotifyOfPropertyChange();
                this.NotifyOfPropertyChange(() => this.IsQuadTab);
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsQuadTab.
        /// </summary>
        public bool IsQuadTab => !this.IsRegularTab;

        /// <summary>
        /// Gets a value indicating whether the tab is in a folder.
        /// </summary>
        public bool IsInFolder
        {
            get
            {
                return this._isInFolder;
            }

            private set
            {
                this._isInFolder = value;
                this.NotifyOfPropertyChange();
                this.NotifyOfPropertyChange(() => this.IsNotInFolder);
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsNotInFolder.
        /// </summary>
        public bool IsNotInFolder => !this.IsInFolder;

        #endregion

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Toggle the tab type.
        /// </summary>
        public void ToggleTabType()
        {
            this.IsRegularTab = !this.IsRegularTab;

            if (this.IsQuadTab)
            {
                this._service.AddQuadTab(this._currentTabName);
            }
            else
            {
                this._service.RemoveQuadTab(this._currentTabName);
            }
        }

        /// <summary>
        /// Toggle the tab type.
        /// </summary>
        public void ToggleInFolder()
        {
            this.IsInFolder = !this.IsInFolder;
            this._service.AddOrUpdateTab(new StashTab()
            {
                Name = this._currentTabName,
                InFolder = this.IsInFolder,
                TabType = this.IsRegularTab ? StashTabType.Regular : StashTabType.Quad,
            });

            this.SetWindowPosition();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._service.NewMarkerRequested -= this.Service_NewMarkerRequested;
                this._service.CloseRequested -= this.Service_CloseRequested;
            }
        }

        /// <summary>
        /// Will place the overlay.
        /// </summary>
        /// <param name="windowInformation">The window information.</param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            this._currentWindowInformation = windowInformation;

            // When Poe Lurker is updated we save the settings before the view are loaded
            if (this.View == null)
            {
                return;
            }

            Execute.OnUIThread(() =>
            {
                var size = DefaultSize * windowInformation.Height / DefaultTabHeight;
                var leftMargin = DefaultLeftMargin * windowInformation.Height / DefaultTabHeight;
                var topMargin = DefaultTopMargin * windowInformation.Height / DefaultTabHeight;

                var top = windowInformation.Position.Top + topMargin - Margin;
                if (this.IsInFolder)
                {
                    top += 42 * windowInformation.Height / DefaultTabHeight;
                }

                // 50 is the footer
                this.View.Height = this.ApplyScalingY(size + 50);
                this.View.Width = this.ApplyScalingX(size);
                this.View.Left = this.ApplyScalingX(windowInformation.Position.Left + leftMargin);
                this.View.Top = this.ApplyScalingY(top);
            });
        }

        private void SetWindowPosition() => this.SetWindowPosition(this._currentWindowInformation);

        private void Service_NewMarkerRequested(object sender, StashTabLocation e)
        {
            this._currentTabName = e.Name;
            this.Left = e.Left - 1;
            this.Top = e.Top - 1;
            this.IsRegularTab = e.StashTabType == StashTabType.Regular;
            this.IsVisible = true;
        }

        private void Service_CloseRequested(object sender, System.EventArgs e)
        {
            this.IsVisible = false;
        }

        #endregion
    }
}
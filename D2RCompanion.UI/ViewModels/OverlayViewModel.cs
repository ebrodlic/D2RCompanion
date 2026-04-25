// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using D2RCompanion.Core.Traderie.Domain;
using D2RCompanion.UI.Messages;
using D2RCompanion.ViewModels;
using Microsoft.Extensions.Logging;

namespace D2RCompanion.UI.ViewModels
{
    public partial class OverlayViewModel :
        ObservableObject
    {
        private readonly HomeViewModel _homeViewModel;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly PriceCheckViewModel _priceCheckViewModel;

        private readonly ILogger _logger;

        [ObservableProperty]
        public object currentViewModel;

        public OverlayViewModel(
            HomeViewModel home,
            SettingsViewModel settings,
            PriceCheckViewModel priceCheck,
            ILogger<OverlayViewModel> logger
            )
        {
            _homeViewModel = home;
            _settingsViewModel = settings;
            _priceCheckViewModel = priceCheck;

            _logger = logger;

            SetMessageHandler();
            ResetView(); // Default = Home
        }

        private void SetMessageHandler()
        {
            WeakReferenceMessenger.Default.Register<NavigationRequestMessage>(this, (r, m) =>
            {
                SetView(m.ToView);
            });
        }

        public void ResetView()
        {
            CurrentViewModel = _homeViewModel;
        }

        private void SetView(OverlayContentView view = OverlayContentView.Home)
        {
            CurrentViewModel = view switch
            {
                OverlayContentView.Home => _homeViewModel,
                OverlayContentView.Settings => _settingsViewModel,
                OverlayContentView.PriceCheck => _priceCheckViewModel,
                _ => _homeViewModel
            };

            _logger.LogDebug("Switched to view: {View}", view);
        }

        //public void Receive(NavigationRequestMessage message)
        //{
        //    _logger.LogDebug("Navigation message received: " + message.ToView);

        //    switch (message.ToView)
        //    {
        //        case OverlayContentView.Settings:
        //            CurrentViewModel = _settingsViewModel;
        //            break;
        //        case OverlayContentView.PriceCheck:
        //            CurrentViewModel = _priceCheckViewModel;
        //            break;
        //    }
        //}
    }

    public enum OverlayContentView
    {
        Home,
        Settings,
        PriceCheck
    }
}

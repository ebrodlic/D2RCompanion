// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using D2RCompanion.UI.Messages;

namespace D2RCompanion.UI.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [RelayCommand]
        public void ShowHome()
        {

        }
        [RelayCommand]
        public void ShowSettings() 
        {
            WeakReferenceMessenger.Default.Send(
                new NavigationRequestMessage(OverlayContentView.Settings)
            );
        }

        [RelayCommand]
        public void ShowPriceCheck()
        {
            WeakReferenceMessenger.Default.Send(
                new NavigationRequestMessage(OverlayContentView.PriceCheck)
            );
        }

        [RelayCommand]
        public void ShowTraderie()
        {
            WeakReferenceMessenger.Default.Send(
                new TraderieVisibilityRequestMessage() { Toggle = true }
            );
        }
    }
}

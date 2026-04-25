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
    public partial class PriceCheckViewModel : ObservableObject
    {

        [RelayCommand]
        public void Close()
        {
            WeakReferenceMessenger.Default.Send(
                new NavigationRequestMessage(OverlayContentView.Home));

            //close overlay
            WeakReferenceMessenger.Default.Send(
                new OverlayVisibilityRequestMessage(false)
            );


            //WeakReferenceMessenger.Default.Send(
            //    new NavigationRequestMessage
            //    {
            //        FromView = OverlayContentView.Settings,
            //        ToView = OverlayContentView.Home,
            //    }
            //);


        }
    }
}

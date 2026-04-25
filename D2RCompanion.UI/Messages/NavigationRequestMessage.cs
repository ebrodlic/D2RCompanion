// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using D2RCompanion.UI.ViewModels;

namespace D2RCompanion.UI.Messages
{
    public class NavigationRequestMessage
    {
        public OverlayContentView ToView { get; set; }

        public NavigationRequestMessage(OverlayContentView view)
        {
            ToView = view;
        }
    }
}

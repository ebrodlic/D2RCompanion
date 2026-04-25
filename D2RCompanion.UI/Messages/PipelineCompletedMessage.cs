// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using D2RCompanion.UI.Services;

namespace D2RCompanion.UI.Messages
{
    public class PipelineCompletedMessage
    {
        public PipelineResult Result { get; set; }

        public PipelineCompletedMessage(PipelineResult result)
        {
            Result = result;
        }
    }
}

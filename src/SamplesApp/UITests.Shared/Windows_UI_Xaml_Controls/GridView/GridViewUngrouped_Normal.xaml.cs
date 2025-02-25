﻿using Uno.UI.Samples.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using SamplesApp.Windows_UI_Xaml_Controls.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GenericApp.Views.Content.UITests.GridView
{
    [Sample("GridView", ViewModelType = typeof(ListViewGroupedViewModel), IgnoreInSnapshotTests = true)]
    public sealed partial class GridViewUngrouped_Normal : UserControl
    {
        public GridViewUngrouped_Normal()
        {
            this.InitializeComponent();
        }
    }
}

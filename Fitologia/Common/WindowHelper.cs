﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fitologia.Common
{
    public class WindowHelper
    {
        private Page _page;

        public WindowHelper(Page page)
        {
            _page = page;
            _page.Loaded += page_Loaded;
            _page.Unloaded += page_Unloaded;
        }

        public IEnumerable<WindowState> States { get; set; }

        private void page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Window_SizeChanged;
            DetermineState(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
        }

        private void page_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Window_SizeChanged;
        }

        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            DetermineState(e.Size.Width, e.Size.Height, true);
        }

        private void DetermineState(double width, double height, bool transitions = false)
        {
            var state = States.First(x => x.MatchCriterium(width, height));
            VisualStateManager.GoToState(_page, state.State, transitions);
        }
    }

    public class WindowState
    {
        public string State { get; set; }

        public Func<double, double, bool> MatchCriterium { get; set; }
    }
}

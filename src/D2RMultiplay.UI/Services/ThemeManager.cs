using System;
using System.Linq;
using System.Windows;

namespace D2RMultiplay.UI.Services
{
    public static class ThemeManager
    {
        public static void ApplyTheme(string themeName)
        {
            var app = Application.Current;
            if (app == null) return;

            // Find existing theme dictionary
            var existingTheme = app.Resources.MergedDictionaries.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("/Themes/"));

            if (existingTheme != null)
            {
                app.Resources.MergedDictionaries.Remove(existingTheme);
            }

            // Add new theme dictionary
            var newThemeSource = new Uri($"/D2RMultiplay.UI;component/Themes/{themeName}Theme.xaml", UriKind.Relative);
            app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = newThemeSource });
        }
    }
}

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Deployment.Application;
using System.Reflection;
using System.Windows;

namespace HtmlAgilityPackTestBed
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppCenter.Start("ae59173a-2296-4306-b03d-4db3a680d5af",
                   typeof(Analytics), typeof(Crashes));

#if DEBUG
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Running in DEBUG version " + getRunningVersion().ToString());
#else
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Running in RELEASE version " + getRunningVersion().ToString());
#endif
        }

        private Version getRunningVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
    }
}

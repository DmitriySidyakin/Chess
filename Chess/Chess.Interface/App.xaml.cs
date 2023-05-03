using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Chess.Interface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public bool ForceSoftwareRendering
        {
            get
            {
                int renderingTier = (System.Windows.Media.RenderCapability.Tier >> 16);
                return renderingTier == 0;
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            if (ForceSoftwareRendering)
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        }
    }
}

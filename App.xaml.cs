using System.Configuration;
using System.Data;
using System.Windows;

namespace DotNetPad
{
    public partial class App : Application
    {
        //
        // Application state variables
        //
        public static bool AutoSave = false;
        public static double FontSizeInSettings;
        public static string Choice = "Cancel";
    }
}

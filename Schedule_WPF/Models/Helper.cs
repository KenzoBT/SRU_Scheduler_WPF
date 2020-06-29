using System.Reflection;
using System.Windows;

namespace Schedule_WPF.Models
{
    class Helper
    {
        public static void CloseUniqueWindow<T>()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            foreach (Window w in Application.Current.Windows)
            {
                if (w.GetType().Assembly == currentAssembly && w is T)
                {
                    w.Close();
                    break;
                }
            }
        }
    }
}

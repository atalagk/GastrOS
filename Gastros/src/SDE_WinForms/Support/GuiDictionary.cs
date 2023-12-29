using System.Resources;

namespace GastrOs.Sde.Views.WinForms.Support
{
    public static class GuiDictionary
    {
        private static ResourceManager guiStrings;

        static GuiDictionary()
        {
            //TODO handle exceptions!
            guiStrings = new ResourceManager("GastrOs.Sde.Views.WinForms.Support.LocalisedGuiStrings",
                                             typeof (GuiDictionary).Assembly);
        }

        public static string Lookup(string key)
        {
            return guiStrings.GetString(key);
        }
    }
}

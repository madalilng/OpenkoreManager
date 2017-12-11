using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace openkore_manager
{

    public class ConfigKeyViewModel : BaseViewModel
    {
        public string ConfigKey { get; set; }
        public string Value { get; set; }
        public int index { get; set; }

        public override string ToString()
        {
            return ConfigKey + " " + Value;
        }

        public ConfigKeyViewModel(string key , string val , int i)
        {
            ConfigKey = key;
            Value = val;
            index = i;
        }
    }

    public class AddonViewModel : BaseViewModel
    {
        public string AddonKey { get; set; }
        public bool isActive { get; set; }
        public bool isMacro { get; set; }
        public int index { get; set; }

        public AddonViewModel( string key , bool v ,int i, bool macro = false)
        {
            AddonKey = key;
            isActive = v;
            isMacro = macro;
            index = i;
        }
    }
    public class AddonListViewModel : BaseViewModel
    {
        public AddonListViewModel(string name, params AddonViewModel[] AddonVM)
        {
            Title = name;
            Addonitem = new ObservableCollection<AddonViewModel>(AddonVM);
        }

        public string Title { get; set; }
        public ObservableCollection<AddonViewModel> Addonitem { get; set; } = new ObservableCollection<AddonViewModel>();

    }
}

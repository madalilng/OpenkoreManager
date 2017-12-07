namespace openkore_manager
{
    public class ConfigViewModel : BaseViewModel
    {

    }

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
}

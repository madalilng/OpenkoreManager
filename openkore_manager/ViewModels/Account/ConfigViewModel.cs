using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

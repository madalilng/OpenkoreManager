using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openkore_manager
{
    public class ContentViewModel :BaseViewModel
    {
        
        public account Content { get; set; }
        public string Header {
            get
            {
                return Content.ViewModel.BotName;
            }
        }
        public ContentViewModel( string name )
        {
            Content = new account(name);
        }
    }
}

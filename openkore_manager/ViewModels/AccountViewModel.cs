using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openkore_manager
{
    public class AccountViewModel : BaseViewModel
    {
        public string BotName { get; set; }
        public bool isBusy { get; set; }
        public string Control {
            get
            {
                return "Bots/" + BotName;
            }
        }
        public List<ConfigKeyViewModel> Configs { get; set; } = new List<ConfigKeyViewModel>();

        private List<string> CommonConfig { get; } = new List<string> { "username","password","char", "autoResponseOnHeal", "pauseCharServer", "pauseMapServer", "attackAuto","route_randomWalk","allowedMaps","allowedMaps_reaction","attackAuto_inLockOnly","dcPause","dcOnServerClose","dcOnServerShutDown","partyAuto","partyAutoShare","partyAutoShareItem","partyAutoShareItemDiv","route_randomWalk","teleportAuto_idle","teleportAuto_useSkill","lockMap" };
        public async void InitConfig()
        {
            await RunCommandAsync(() => isBusy, async () =>
            {
                await Task.Factory.StartNew(() => {
                    int index = 0;
                    string line;
                    System.IO.StreamReader file = new System.IO.StreamReader(Control + "/config.txt");
                    bool inBracket = false;
                    while ( (line = file.ReadLine()) != null )
                    {
                        if (!line.Contains("#") && !string.IsNullOrEmpty(line) )
                        {
                            if (!line.Contains("{") && !line.Contains("}") && !inBracket)
                            {
                                var key = line.Split(' ');
                                if( CommonConfig.Contains(key[0])) { 
                                    try
                                    {
                                        Configs.Add(new ConfigKeyViewModel() { ConfigKey = key[0], Value = key[1], index = index });
                                    }
                                    catch
                                    {
                                        Configs.Add(new ConfigKeyViewModel() { ConfigKey = key[0], Value = "", index = index });
                                    }
                                }
                                //System.Console.WriteLine(line);
                            }
                            else if (line.Contains("{"))
                            {
                                inBracket = true;
                            }
                            else if (line.Contains("}"))
                            {
                                inBracket = false;
                            }
                        }
                        index++;
                    }
                });
            });
        }
    }
}

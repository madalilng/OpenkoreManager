using System;
using System.Collections.Generic;
using System.IO;
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

        public async void EditConfigAsync( object sender, System.ComponentModel.PropertyChangedEventArgs e )
        {
            await Task.Factory.StartNew(() =>
            {
                var s = sender as ConfigKeyViewModel;
                StringBuilder sbText = new StringBuilder();

                using (var reader = new System.IO.StreamReader(Control + "/config.txt"))
                {
                    string line;
                    int counter = 0;
                    while ((line = reader.ReadLine()) != null)
                    {

                        if (s.index == counter)
                        {
                            sbText.AppendLine(s.ToString());
                        }
                        else
                        {
                            sbText.AppendLine(line);
                        }
                        counter++;
                    }
                }

                using (var writer = new System.IO.StreamWriter(Control + "/config.txt"))
                {
                    writer.Write(sbText.ToString());
                }

            });
        }
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
                                        var conf = new ConfigKeyViewModel(key[0], key[1], index);
                                        conf.PropertyChanged += EditConfigAsync;
                                        Configs.Add(conf);
                                    }
                                    catch
                                    {
                                        var conf = new ConfigKeyViewModel(key[0], "", index);
                                        conf.PropertyChanged += EditConfigAsync;
                                        Configs.Add(conf);
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

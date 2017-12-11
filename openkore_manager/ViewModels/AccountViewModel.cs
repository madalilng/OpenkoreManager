using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace openkore_manager
{
    public class AccountViewModel : BaseViewModel
    {
        public string BotName { get; set; }
        public string oldBotname { get; set; }
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Mainconfig;
        public ObservableCollection<AddonListViewModel> Addons { get; set; }
        public bool isBusy { get; set; }
        public bool ConfigChanged { get; set; }
        public string Control {
            get
            {
                return "Bots/" + BotName;
            }
        }


        public List<ConfigKeyViewModel> Configs { get; set; } = new List<ConfigKeyViewModel>();

        private List<string> CommonConfig { get; } = new List<string> { "username","password","char", "autoResponseOnHeal", "pauseCharServer", "pauseMapServer", "attackAuto","route_randomWalk","allowedMaps","allowedMaps_reaction","attackAuto_inLockOnly","dcPause","dcOnServerClose","dcOnServerShutDown","partyAuto","partyAutoShare","partyAutoShareItem","partyAutoShareItemDiv","route_randomWalk","teleportAuto_idle","teleportAuto_useSkill","lockMap" };

        public ICommand Refresh { get; set; }
        public ICommand MainConfigBtn { get; set; }
        public ICommand OtherBtn { get; set; }
        public ICommand DeleteBtn { get; set; }
        public ICommand DuplicateBtn { get; set; }
        public AccountViewModel()
        {

            Refresh = new RelayCommand(RefreshAsync);
            MainConfigBtn = new RelayCommand(MainConfigBtnAct);
            OtherBtn = new RelayCommand(OtherBtnAct);
            DeleteBtn = new RelayCommand(DeleteBtnActAsync);
            DuplicateBtn = new RelayCommand(DuplicateBtnActAsync);
        }

        public async void DuplicateBtnActAsync()
        {
            await RunCommandAsync(() => ConfigChanged, async () =>
            {
                int counter = 0;
                foreach (var item in Directory.GetDirectories(@"Bots"))
                {
                    counter++;
                }
                string nName = BotName + (counter + 1).ToString();
                Directory.CreateDirectory(@"Bots\" + nName);
                foreach (var file in Directory.GetFiles(Control))
                    File.Copy(file, Path.Combine(@"Bots\" + nName, Path.GetFileName(file)));
            });
        }

        public async void DeleteBtnActAsync()
        {
            await RunCommandAsync(() => ConfigChanged, async () =>
            {
                foreach (var file in Directory.GetFiles(Control))
                    File.Delete(file);
                Directory.Delete(Control);
            });
        }

        private void MainConfigBtnAct()
        {
            CurrentPage = ApplicationPage.Mainconfig;
            InitConfig();
        }

        private void OtherBtnAct()
        {
            CurrentPage = ApplicationPage.Other;
            InitConfig();
        }

        private void RefreshAsync()
        {
            InitConfig();
            Console.WriteLine("refresh");
        }

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


        public async void EditAddonAsync(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                var s = sender as AddonViewModel;
                StringBuilder sbText = new StringBuilder();
                if (!s.isMacro)
                {
                    using (var reader = new System.IO.StreamReader(Control + "/eventMacros.txt"))
                    {
                        string line;
                        int counter = 0;
                        while ((line = reader.ReadLine()) != null)
                        {

                            if (s.index == counter)
                            {
                                if (s.isActive)
                                    sbText.AppendLine(line.Replace('#', '!'));
                                else
                                    sbText.AppendLine(line.Replace('!', '#'));
                            }
                            else
                            {
                                sbText.AppendLine(line);
                            }
                            counter++;
                        }
                    }

                    using (var writer = new System.IO.StreamWriter(Control + "/eventMacros.txt"))
                    {
                        writer.Write(sbText.ToString());
                    }
                }
                else
                {
                    using (var reader = new System.IO.StreamReader(Control + "/macros.txt"))
                    {
                        string line;
                        int counter = 0;
                        while ((line = reader.ReadLine()) != null)
                        {

                            if (s.index == counter)
                            {
                                if (s.isActive)
                                    sbText.AppendLine(line.Replace('#', '!'));
                                else
                                    sbText.AppendLine(line.Replace('!', '#'));
                            }
                            else
                            {
                                sbText.AppendLine(line);
                            }
                            counter++;
                        }
                    }

                    using (var writer = new System.IO.StreamWriter(Control + "/macros.txt"))
                    {
                        writer.Write(sbText.ToString());
                    }
                }
            });
        }


        public async void InitConfig()
        {
            await RunCommandAsync(() => isBusy, async () =>
            {
                await Task.Factory.StartNew(() => {
                    try
                    {
                        if (!File.Exists(Control + "/eventMacros.txt"))
                        {
                            StringBuilder EventMacro = new StringBuilder();
                            StringBuilder Macro = new StringBuilder();
                            foreach (var l in Directory.GetDirectories(@"scripts\add-ons"))
                            {
                                if (!l.Contains("utilities"))
                                {
                                    EventMacro.AppendLine("#~" + l.Split('\\')[l.Split('\\').Count() - 1]);
                                }
                                foreach (var f in Directory.GetFiles(l))
                                {
                                    EventMacro.AppendLine(@"#include ..\..\" + f);
                                }

                                foreach (var lx in Directory.GetDirectories(l))
                                {
                                    if (lx.Contains("macros"))
                                    {
                                        Macro.AppendLine("#~" + lx.Split('\\')[lx.Split('\\').Count() - 1]);
                                        foreach (var fx in Directory.GetFiles(lx))
                                        {
                                            Macro.AppendLine(@"#include ..\..\" + fx);
                                        }
                                    }
                                    else
                                    {
                                        EventMacro.AppendLine("#~" + lx.Split('\\')[lx.Split('\\').Count() - 1]);
                                        foreach (var fx in Directory.GetFiles(lx))
                                        {
                                            EventMacro.AppendLine(@"#include ..\..\" + fx);
                                        }
                                    }

                                }
                            }
                            Console.WriteLine(Macro.ToString());
                            using (var writer = new System.IO.StreamWriter(Control + "/eventMacros.txt"))
                            {
                                writer.Write(EventMacro.ToString());
                            }

                            using (var writer = new System.IO.StreamWriter(Control + "/macros.txt"))
                            {
                                writer.Write(Macro.ToString());
                            }
                        }

                        Addons = new ObservableCollection<AddonListViewModel>();
                        System.IO.StreamReader emacro = new System.IO.StreamReader(Control + "/eventMacros.txt");
                        string Macroline;
                        int ecounter = 0;
                        while ((Macroline = emacro.ReadLine()) != null)
                        {
                            if (Macroline.Contains("#~"))
                            {
                                var title = Macroline.Split('~');
                                Addons.Add(new AddonListViewModel(title[1]));
                            }
                            else if ((Macroline.Contains("#") || Macroline.Contains("!")) && !Macroline.Contains("~"))
                            {
                                var x = Macroline.Split('\\');

                                var eventmacros = new AddonViewModel(x[x.Count() - 1].Split('.')[0], Macroline.Contains('#') ? false : true, ecounter, false);
                                eventmacros.PropertyChanged += EditAddonAsync;
                                Addons[Addons.Count() - 1].Addonitem.Add(eventmacros);
                            }
                            ecounter++;
                        }
                        int counter = 0;
                        System.IO.StreamReader macro = new System.IO.StreamReader(Control + "/macros.txt");
                        while ((Macroline = macro.ReadLine()) != null)
                        {
                            if (Macroline.Contains("#~"))
                            {
                                var title = Macroline.Split('~');
                                Addons.Add(new AddonListViewModel(title[1]));
                            }
                            else if ((Macroline.Contains("#") || Macroline.Contains("!")) && !Macroline.Contains("~"))
                            {
                                var x = Macroline.Split('\\');

                                var eventmacros = new AddonViewModel(x[x.Count() - 1].Split('.')[0], Macroline.Contains('#') ? false : true, counter, true);
                                eventmacros.PropertyChanged += EditAddonAsync;
                                Addons[Addons.Count() - 1].Addonitem.Add(eventmacros);
                            }
                            counter++;
                        }
                    }
                    catch (Exception) { }

                    Configs = new List<ConfigKeyViewModel>();
                    int index = 0;
                    string line;
                    System.IO.StreamReader file = new System.IO.StreamReader(Control + "/config.txt");
                    bool inBracket = false;
                    while ( (line = file.ReadLine()) != null )
                    {
                        if (!line.Contains("#") && !string.IsNullOrEmpty(line))
                        {
                            if (!line.Contains("{") && !line.Contains("}") && !inBracket)
                            {
                                var key = line.Split(' ');
                                if (CommonConfig.Contains(key[0]))
                                {
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


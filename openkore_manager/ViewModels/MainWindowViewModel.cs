using LibGit2Sharp;
using LibGit2Sharp.Handlers;
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
    public class MainWindowViewModel : BaseViewModel
    {
        public bool isBusy { get; set; }
        public bool SnackBarVisibility { get; set; }
        public string SnackBarMessage { get; set; }
        public ObservableCollection<ContentViewModel> Bots { get; set; } = new ObservableCollection<ContentViewModel>();
        public int SelectedIndex { get; set; }

        public ICommand AddNewBot { get; set; }

        public void DisplayMessage( string message)
        {
            SnackBarMessage = message;
            SnackBarVisibility = true;
        }

        public void HideMessage()
        {
            SnackBarVisibility = false;
        }

        public MainWindowViewModel()
        {
            AddNewBot = new RelayCommand(AddNewBotAsync);
            //update();
            LoadBots();
        }

        private async void AddNewBotAsync()
        {
            await RunCommandAsync(() => isBusy, async () =>
            {
                Directory.CreateDirectory(@"Bots\New" + (Bots.Count + 1));
                foreach (var file in Directory.GetFiles(@"openkore\control"))
                    File.Copy(file, Path.Combine(@"Bots\New" + (Bots.Count + 1), Path.GetFileName(file)));
                var newbot = new ContentViewModel("New" + (Bots.Count + 1));
                newbot.Content.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
                Bots.Add(newbot);
                SelectedIndex = Bots.Count - 1;
            });
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var s = sender as AccountViewModel;
            if("ConfigChanged" == e.PropertyName) 
            {
                if(!s.ConfigChanged)
                    LoadBots();
            }
            if ( e.PropertyName == "BotName")
            {
                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    string newname = s.BotName.Replace(' ', '_');
                    Directory.CreateDirectory(@"Bots\" + newname);
                    foreach (var file in Directory.GetFiles(@"Bots\" + s.oldBotname))
                        File.Copy(file, Path.Combine(@"Bots\" + newname, Path.GetFileName(file)));
                    foreach (var file in Directory.GetFiles(@"Bots\" + s.oldBotname))
                        File.Delete(file);
                    Directory.Delete(@"Bots\" + s.oldBotname);
                }));
                LoadBots();
            }
        }

        private void LoadBots()
        {
            Bots = new ObservableCollection<ContentViewModel>();
            DirectoryInfo directory = new DirectoryInfo("Bots");
            DirectoryInfo[] directories = directory.GetDirectories();
            foreach (var item in directories)
            {
                var bot = new ContentViewModel(item.Name);
                bot.Content.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
                Bots.Add(bot);
            }
        }

        private void update()
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(async () =>
            {
                isBusy = true;
                if (!Directory.Exists("openkore"))
                {
                    DisplayMessage("Cloning https://github.com/OpenKore/openkore");
                    Repository.Clone("https://github.com/OpenKore/openkore", "openkore");
                }
                else
                {
                    DisplayMessage("Updating Openkore");
                    using (var repo = new Repository("openkore"))
                    {
                        PullOptions options = new PullOptions();
                        Commands.Pull(repo, new Signature("madalilng", "madalilng@gmail.com", new DateTimeOffset(DateTime.Now)), options);
                    }
                }

                if (!Directory.Exists("scripts"))
                {
                    DisplayMessage("Cloning https://github.com/iamtonysoft/legit-repo");
                    Repository.Clone("https://github.com/iamtonysoft/legit-repo", "scripts");
                    DisplayMessage("Done");
                    HideMessage();
                }
                else
                {
                    DisplayMessage("Updating Scripts");
                    using (var repo = new Repository("scripts"))
                    {
                        PullOptions options = new PullOptions();
                        Commands.Pull(repo, new Signature("madalilng", "madalilng@gmail.com", new DateTimeOffset(DateTime.Now)), options);
                    }
                    DisplayMessage("Done");
                    HideMessage();
                }

                isBusy = false;
            }));
        }
    }
}


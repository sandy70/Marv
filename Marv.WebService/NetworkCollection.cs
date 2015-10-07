using System.Collections.ObjectModel;
using System.IO;
using Marv.Common;

namespace Marv.WebService
{
    public class NetworkCollection : NotifyPropertyChanged
    {
        private ObservableCollection<Network> networks = new ObservableCollection<Network>();
        private Network selectedNetwork;

        public ObservableCollection<Network> Networks
        {
            get { return this.networks; }

            set
            {
                this.networks = value;
                this.RaisePropertyChanged();
            }
        }

        public Network SelectedNetwork
        {
            get { return this.selectedNetwork; }

            set
            {
                this.selectedNetwork = value;
                this.RaisePropertyChanged();
            }
        }

        public static NetworkCollection Read(string dirPath)
        {
            var networkCollection = new NetworkCollection();

            foreach (var fileName in Directory.GetFiles(dirPath, "*.net"))
            {
                networkCollection.Networks.Add(Network.Read(fileName));
            }

            return networkCollection;
        }
    }
}
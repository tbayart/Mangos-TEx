using Framework.MVVM;

namespace MangosModels
{
    public class Item : NotificationObject
    {
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(() => Id);
            }
        }
        private int _id;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }
        private string _name;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }
        private string _description;

        public string LocalizedName
        {
            get { return _localizedName; }
            set
            {
                _localizedName = value;
                RaisePropertyChanged(() => LocalizedName);
            }
        }
        private string _localizedName;

        public string LocalizedDescription
        {
            get { return _localizedDescription; }
            set
            {
                _localizedDescription = value;
                RaisePropertyChanged(() => LocalizedDescription);
            }
        }
        private string _localizedDescription;

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                RaisePropertyChanged(() => Error);
            }
        }
        private string _error;
    }
}

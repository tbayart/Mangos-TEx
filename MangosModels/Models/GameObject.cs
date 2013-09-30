using System.Collections.Generic;
using Framework.MVVM;
using MangosModels.DataTypes;
using MangosModels.ModelBase;

namespace MangosModels
{
    public class GameObject : NotificationObject
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

        public GameObjectType GameObjectType { get { return (GameObjectType)Type; } }
        public int Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged(() => Type);
                RaisePropertyChanged(() => GameObjectType);
            }
        }
        private int _type;

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

        public IEnumerable<GameObjectData> RelatedData
        {
            get { return _relatedData; }
            set
            {
                _relatedData = value;
                RaisePropertyChanged(() => RelatedData);
            }
        }
        private IEnumerable<GameObjectData> _relatedData;

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

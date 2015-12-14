using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;

namespace MangosTEx.Models
{
    public class WowApiLocalizedData : ObservableObject
    {
        #region Properties
        public int ItemId
        {
            get { return _itemId; }
            set
            {
                _itemId = value;
                RaisePropertyChanged(() => ItemId);
            }
        }
        private int _itemId;

        public string Query
        {
            get { return _query; }
            set
            {
                _query = value;
                RaisePropertyChanged(() => Query);
            }
        }
        private string _query;

        public string Data
        {
            get { return _data; }
            set
            {
                _data = value;
                RaisePropertyChanged(() => Data);
            }
        }
        private string _data;

        public LocalizationStatus Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }
        private LocalizationStatus _status;

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
        #endregion Properties

        #region Methods
        protected LocalizationStatus GetStatus()
        {
            if (string.IsNullOrEmpty(Query) == true)
                return LocalizationStatus.Unprocessed;

            if (string.IsNullOrEmpty(Data) == true)
                return LocalizationStatus.NotEqual;

            return LocalizationStatus.Equal;
        }

        private void UpdateStatusInternal()
        {
            var status = LocalizationStatus.Error;
            // check for reported error and if none, get status
            if (string.IsNullOrEmpty(Error))
                status = GetStatus();
            // update status
            Status = status;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName != GetPropertyName(() => Status))
                UpdateStatusInternal();
        }
        #endregion Methods
    }
}

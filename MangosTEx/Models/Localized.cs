using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;

namespace MangosTEx.Models
{
    public abstract class Localized<DBENTITY, TENTITY> : ObservableObject
    {
        #region Properties
        public DBENTITY DatabaseEntity
        {
            get { return _databaseEntity; }
            set
            {
                _databaseEntity = value;
                RaisePropertyChanged(() => DatabaseEntity);
            }
        }
        private DBENTITY _databaseEntity;

        public TENTITY TranslatedEntity
        {
            get { return _translatedEntity; }
            set
            {
                _translatedEntity = value;
                RaisePropertyChanged(() => TranslatedEntity);
            }
        }
        private TENTITY _translatedEntity;

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
        protected abstract LocalizationStatus GetStatus();

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

    public enum LocalizationStatus
    {
        Untranslated,
        Equal,
        NotEqual,
        Error,
    }
}

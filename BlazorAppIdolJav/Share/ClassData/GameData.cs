using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
    [DataContract(Name = "ActressData", Namespace = "")]

    public class GameData
    {
        [DataMember(Order = 1)]
        public virtual String Id
        {
            get;
            set;
        }
        [DataMember(Order = 2)]
        public virtual String Name
        {
            get;
            set;
        }
        [DataMember(Order = 3)]
        public virtual String Country
        {
            get;
            set;
        }
        [DataMember(Order = 4)]
        public virtual string? ImageName
        {
            get;
            set;
        }
        [DataMember(Order = 5)]
        public virtual DateTime CreateDate
        {
            get;
            set;
        }
        [DataMember(Order = 6)]
        public virtual DateTime ReleaseDate
        {
            get;
            set;
        }
        [DataMember(Order = 7)]
        public virtual string Status
        {
            get;
            set;
        }
        [DataMember(Order = 8)]
        public virtual string SoldStatus
        {
            get;
            set;
        }
        [DataMember(Order = 9)]
        public virtual int Price
        {
            get;
            set;
        }
        [DataMember(Order = 10)]
        public virtual string Unit
        {
            get;
            set;
        }
        [DataMember(Order = 11)]
        public virtual string Description
        {
            get;
            set;
        }
        [DataMember(Order = 12)]
        public virtual string SystemSupport
        {
            get;
            set;
        }
        [DataMember(Order = 13)]
        public virtual string GameCompanyId
        {
            get;
            set;
        }
        [DataMember(Order = 14)]
        public virtual string? ImagePath
        {
            get;
            set;
        }
    }
}

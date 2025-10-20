using System.Runtime.Serialization;

namespace BlazorAppIdolJav.Share.ClassData
{
    [DataContract(Name = "ActressData", Namespace = "")]

    public class ActressData
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
        public virtual int Age
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
        public virtual DateTime? DateDebut
        {
            get;
            set;
        }
        [DataMember(Order = 7)]
        public virtual string Gender
        {
            get;
            set;
        }
        [DataMember(Order = 8)]
        public virtual string CompanyId
        {
            get;
            set;
        }
        [DataMember(Order = 9)]
        public virtual int ButtSize
        {
            get;
            set;
        }
        [DataMember(Order = 10)]
        public virtual int WaistSize
        {
            get;
            set;
        }
        [DataMember(Order = 11)]
        public virtual int ChestSize
        {
            get;
            set;
        }
        [DataMember(Order = 12)]
        public virtual string NickName
        {
            get;
            set;
        }
        [DataMember(Order = 13)]
        public virtual string ImageName
        {
            get;
            set;
        }
        [DataMember(Order = 14)]
        public virtual string ImagePath
        {
            get;
            set;
        }
    }
}

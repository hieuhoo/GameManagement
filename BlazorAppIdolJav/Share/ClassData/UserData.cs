using System.Runtime.Serialization;

namespace BlazorAppIdolJav.Share.ClassData
{
    public class UserData
    {
        [DataMember(Order = 1)]
        public virtual String Id
        {
            get;
            set;
        }
        [DataMember(Order = 2)]
        public virtual String UserName
        {
            get;
            set;
        }
        [DataMember(Order = 3)]
        public virtual String PassWord
        {
            get;
            set;
        }
        [DataMember(Order = 4)]
        public virtual int? PhoneNumber
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
        public virtual String Email
        {
            get;
            set;
        }
        [DataMember(Order = 7)]
        public virtual String Name
        {
            get;
            set;
        }
        [DataMember(Order = 8)]
        public virtual int? QuantityLoginCount
        {
            get;
            set;
        }
    }
}

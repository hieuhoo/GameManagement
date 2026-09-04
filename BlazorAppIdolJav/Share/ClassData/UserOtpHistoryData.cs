using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
    public class UserOtpHistoryData
    {
        [DataMember(Order = 1)]
        public virtual String Id
        {
            get;
            set;
        }
        [DataMember(Order = 2)]
        public virtual String UserId
        {
            get;
            set;
        }
        [DataMember(Order = 3)]
        public virtual String OtpType
        {
            get;
            set;
        }
        [DataMember(Order = 4)]
        public virtual DateTime CreateDate
        {
            get;
            set;
        }
        [DataMember(Order = 5)]
        public virtual String Email
        {
            get;
            set;
        }
        [DataMember(Order = 6)]
        public virtual String OtpCode
        {
            get;
            set;
        }
        [DataMember(Order = 7)]
        public virtual string OtpCodeHash
        {
            get;
            set;
        }
        [DataMember(Order = 8)]
        public virtual DateTime ExpiredDate
        {
            get;
            set;
        }
        [DataMember(Order = 9)]
        public virtual bool IsUsed
        {
            get;
            set;
        }
    }
}

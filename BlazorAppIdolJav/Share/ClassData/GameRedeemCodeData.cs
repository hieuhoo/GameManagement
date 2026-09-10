using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
    public class GameRedeemCodeData
    {
        [DataMember(Order = 1)]
        public virtual String Id
        {
            get;
            set;
        }
        [DataMember(Order = 2)]
        public virtual String Code
        {
            get;
            set;
        }
        [DataMember(Order = 3)]
        public virtual int Value
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
        public virtual DateTime ExpiredDate
        {
            get;
            set;
        }
        [DataMember(Order = 6)]
        public virtual DateTime? RedeemedDate
        {
            get;
            set;
        }
        [DataMember(Order = 7)]
        public virtual String RedeemedBy
        {
            get;
            set;
        }
        [DataMember(Order = 8)]
        public virtual String CreateBy
        {
            get;
            set;
        }
        [DataMember(Order = 9)]
        public virtual String Status
        {
            get;
            set;
        }
        [DataMember(Order = 10)]
        public virtual String? BatchId
        {
            get;
            set;
        }
        [DataMember(Order = 11)]
        public virtual String GenerateType
        {
            get;
            set;
        }
    }
}

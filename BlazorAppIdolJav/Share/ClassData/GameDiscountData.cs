using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
    public class GameDiscountData
    {
        [DataMember(Order = 1)]
        public virtual String Id
        {
            get;
            set;
        }
        [DataMember(Order = 2)]
        public virtual String GameId
        {
            get;
            set;
        }
        [DataMember(Order = 3)]
        public virtual int DiscountPercent
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
        public virtual DateTime StartDate
        {
            get;
            set;
        }
        [DataMember(Order = 6)]
        public virtual DateTime EndDate
        {
            get;
            set;
        }
        [DataMember(Order = 7)]
        public virtual String Status
        {
            get;
            set;
        }
    }
}

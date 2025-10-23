using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
    public class GameTypeData
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
        public virtual String Code
        {
            get;
            set;
        }
        [DataMember(Order = 4)]
        public virtual String Status
        {
            get;
            set;
        }
        [DataMember(Order = 5)]
        public virtual String Description
        {
            get;
            set;
        }
        [DataMember(Order = 6)]
        public virtual DateTime CreateDate
        {
            get;
            set;
        }
    }
}

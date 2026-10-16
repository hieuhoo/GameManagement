using Microsoft.AspNetCore.Components;
using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
	public class UserNotificationData
	{
		[DataMember(Order = 1)]
		public virtual String Id
		{
			get;
			set;
		}
		[DataMember(Order = 2)]
		public virtual String ReceiveUserId
		{
			get;
			set;
		}
		[DataMember(Order = 3)]
		public virtual string ActorUserId
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
		public virtual DateTime? ReadAtTime
		{
			get;
			set;
		}
		[DataMember(Order = 6)]
		public virtual bool IsRead
		{
			get;
			set;
		}
		[DataMember(Order = 7)]
		public virtual string Type
		{
			get;
			set;
		}
		[DataMember(Order = 8)]
		public virtual string TargetId
		{
			get;
			set;
		}
		[DataMember(Order = 9)]
		public virtual string ContentView
		{
			get;
			set;
		}
		[DataMember(Order = 10)]
		public virtual string ActorUserName
		{
			get;
			set;
		}
		[DataMember(Order = 11)]
		public virtual string ActorDisplayName
		{
			get;
			set;
		}
		[DataMember(Order = 12)]
		public virtual string GameName
		{
			get;
			set;
		}
		[DataMember(Order = 13)]
		public virtual string GameId
		{
			get;
			set;
		}
	}
}

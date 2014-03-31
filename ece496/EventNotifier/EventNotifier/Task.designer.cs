﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EventNotifier
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="PM")]
	public partial class TaskDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertTask(Task instance);
    partial void UpdateTask(Task instance);
    partial void DeleteTask(Task instance);
    #endregion
		
		public TaskDataContext() : 
				base(global::EventNotifier.Properties.Settings.Default.PMConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public TaskDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TaskDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TaskDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TaskDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Task> Tasks
		{
			get
			{
				return this.GetTable<Task>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Tasks")]
	public partial class Task : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _TaskID;
		
		private string _TaskName;
		
		private string _TaskDescription;
		
		private System.Nullable<System.DateTime> _TaskStartTime;
		
		private System.DateTime _TaskDeadline;
		
		private System.Nullable<int> _FKTeamID;
		
		private System.Nullable<int> _Status;
		
		private System.Nullable<int> _FK_AssigneeID;
		
		private System.Nullable<bool> _alerted;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnTaskIDChanging(int value);
    partial void OnTaskIDChanged();
    partial void OnTaskNameChanging(string value);
    partial void OnTaskNameChanged();
    partial void OnTaskDescriptionChanging(string value);
    partial void OnTaskDescriptionChanged();
    partial void OnTaskStartTimeChanging(System.Nullable<System.DateTime> value);
    partial void OnTaskStartTimeChanged();
    partial void OnTaskDeadlineChanging(System.DateTime value);
    partial void OnTaskDeadlineChanged();
    partial void OnFKTeamIDChanging(System.Nullable<int> value);
    partial void OnFKTeamIDChanged();
    partial void OnStatusChanging(System.Nullable<int> value);
    partial void OnStatusChanged();
    partial void OnFK_AssigneeIDChanging(System.Nullable<int> value);
    partial void OnFK_AssigneeIDChanged();
    partial void OnalertedChanging(System.Nullable<bool> value);
    partial void OnalertedChanged();
    #endregion
		
		public Task()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TaskID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int TaskID
		{
			get
			{
				return this._TaskID;
			}
			set
			{
				if ((this._TaskID != value))
				{
					this.OnTaskIDChanging(value);
					this.SendPropertyChanging();
					this._TaskID = value;
					this.SendPropertyChanged("TaskID");
					this.OnTaskIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TaskName", DbType="VarChar(128)")]
		public string TaskName
		{
			get
			{
				return this._TaskName;
			}
			set
			{
				if ((this._TaskName != value))
				{
					this.OnTaskNameChanging(value);
					this.SendPropertyChanging();
					this._TaskName = value;
					this.SendPropertyChanged("TaskName");
					this.OnTaskNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TaskDescription", DbType="Text", UpdateCheck=UpdateCheck.Never)]
		public string TaskDescription
		{
			get
			{
				return this._TaskDescription;
			}
			set
			{
				if ((this._TaskDescription != value))
				{
					this.OnTaskDescriptionChanging(value);
					this.SendPropertyChanging();
					this._TaskDescription = value;
					this.SendPropertyChanged("TaskDescription");
					this.OnTaskDescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TaskStartTime", DbType="DateTime")]
		public System.Nullable<System.DateTime> TaskStartTime
		{
			get
			{
				return this._TaskStartTime;
			}
			set
			{
				if ((this._TaskStartTime != value))
				{
					this.OnTaskStartTimeChanging(value);
					this.SendPropertyChanging();
					this._TaskStartTime = value;
					this.SendPropertyChanged("TaskStartTime");
					this.OnTaskStartTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TaskDeadline", DbType="DateTime NOT NULL")]
		public System.DateTime TaskDeadline
		{
			get
			{
				return this._TaskDeadline;
			}
			set
			{
				if ((this._TaskDeadline != value))
				{
					this.OnTaskDeadlineChanging(value);
					this.SendPropertyChanging();
					this._TaskDeadline = value;
					this.SendPropertyChanged("TaskDeadline");
					this.OnTaskDeadlineChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FKTeamID", DbType="Int")]
		public System.Nullable<int> FKTeamID
		{
			get
			{
				return this._FKTeamID;
			}
			set
			{
				if ((this._FKTeamID != value))
				{
					this.OnFKTeamIDChanging(value);
					this.SendPropertyChanging();
					this._FKTeamID = value;
					this.SendPropertyChanged("FKTeamID");
					this.OnFKTeamIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Status", DbType="Int")]
		public System.Nullable<int> Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				if ((this._Status != value))
				{
					this.OnStatusChanging(value);
					this.SendPropertyChanging();
					this._Status = value;
					this.SendPropertyChanged("Status");
					this.OnStatusChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FK_AssigneeID", DbType="Int")]
		public System.Nullable<int> FK_AssigneeID
		{
			get
			{
				return this._FK_AssigneeID;
			}
			set
			{
				if ((this._FK_AssigneeID != value))
				{
					this.OnFK_AssigneeIDChanging(value);
					this.SendPropertyChanging();
					this._FK_AssigneeID = value;
					this.SendPropertyChanged("FK_AssigneeID");
					this.OnFK_AssigneeIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_alerted", DbType="Bit")]
		public System.Nullable<bool> alerted
		{
			get
			{
				return this._alerted;
			}
			set
			{
				if ((this._alerted != value))
				{
					this.OnalertedChanging(value);
					this.SendPropertyChanging();
					this._alerted = value;
					this.SendPropertyChanged("alerted");
					this.OnalertedChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591

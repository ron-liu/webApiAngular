using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using CommonDomain.Persistence.EventStore;
using LeaveManager.Api.Domain;
using Ninject;
using Dapper;

namespace LeaveManager.Api.Infrastructure
{
	public interface IDbConecxt
	{
		string DbContext { get; set; }
	}

	public class ReadModelDbContext : IDbConecxt {
		
		public string DbContext { get; set; }

		public IDbConnection GetConnection()
		{
			return new SqlConnection(ConfigurationManager.ConnectionStrings[DbContext].ConnectionString);
		}
	}

	public class LeaveReadModelRepository : IReadModel
	{
		[Inject] private ReadModelDbContext ReadModelDbContext { get; set; }
		[Inject] private IEventSteamRepository<Leave> EventStore { get; set; }

		public void SyncLeave(Guid leaveId)
		{
			var leave = EventStore.Get(leaveId);
			using (var conn = ReadModelDbContext.GetConnection())
			{
				conn.Execute(@"
					begin tran
					delete LeaveReadModel where Id = @Id
					insert into 
					LeaveReadModel(Id, Applicant, Evaluator, StartDate, EndDate, ApplicantComment, EvaluatorComment, Reason, Status, AppliedOn, EvaluatedOn, WorkingDays) 
					values(@Id, @Applicant, @Evaluator, @StartDate, @EndDate, @ApplicantComment, @EvaluatorComment, @Reason, @Status, @AppliedOn, @EvaluatedOn, @WorkingDays) 
					commit tran
				", leave);
			}
		}

		public IEnumerable<Leave> GetLeavesByUserName(string applicant)
		{
			using (var conn = ReadModelDbContext.GetConnection())
			{
				return conn.Query<Leave>("select * from LeaveReadModel where Applicant = @Applicant order by AppliedOn desc", new {Applicant = applicant});
			}
		}

		public Leave GetLeaveById(Guid leaveId)
		{
			using (var conn = ReadModelDbContext.GetConnection())
			{
				return conn.Query<Leave>("select * from LeaveReadModel where Id = @leaveId ", new { leaveId }).FirstOrDefault();
			}
		}

		public Leave GetMyLeaveById(Guid leaveId, string userName)
		{
			using (var conn = ReadModelDbContext.GetConnection())
			{
				return conn.Query<Leave>("select * from LeaveReadModel where Id = @leaveId and Applicant=@userName", new { leaveId, userName }).FirstOrDefault();
			}
		}

		public void Init()
		{
			using (var conn = ReadModelDbContext.GetConnection())
				conn.Execute(@"
					if not exists (select * from sys.tables where name='leaveReadModel')
					begin
					create table leaveReadModel (
						Id uniqueidentifier  primary key,
						Applicant varchar(200),
						Evaluator varchar(200),
						StartDate datetime2,
						EndDate datetime2,
						Reason int,
						Status int,
						AppliedOn datetime2,
						EvaluatedOn datetime2,
						ApplicantComment varchar(1024),
						EvaluatorComment varchar(1024),
						WorkingDays int
					)
					end
				");
		}

		public IEnumerable<Leave> ListLeavesToEvaluate()
		{
			using (var conn = ReadModelDbContext.GetConnection())
			{
				return conn.Query<Leave>("select * from LeaveReadModel where Status = @Status order by AppliedOn desc", new {Status= Leave.StatusEnum.Applied} );
			}
		}

		public IEnumerable<Leave> OverlappedLeaves(DateTime startDate, DateTime endDate)
		{
			using (var conn = ReadModelDbContext.GetConnection())
			{
				return conn.Query<Leave>("select * from LeaveReadModel where Status = @Status and @endDate >= StartDate and @startDate <= EndDate order by AppliedOn desc", new { Status = Leave.StatusEnum.Approved, startDate, endDate });
			}
		}
	}
}
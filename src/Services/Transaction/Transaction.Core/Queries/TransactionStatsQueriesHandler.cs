﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Core.Dtos;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using System.Linq;

namespace Transaction.Core.Queries
{
    class TransactionStatsQueriesHandler : IRequestHandler<TransactionStatsOverTodayQuery, TransactionTodayStatsDto>,
        IRequestHandler<TransactionStatsOverMonthQuery, List<TransactionMonthlyStatsDto>>,
        IRequestHandler<TransactionStatsOverYearQuery, List<int>>
    {
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;

        private IRepository<Guid, ObjectReceiving> _receivingsRepo;

        public TransactionStatsQueriesHandler(IRepository<Guid, ObjectRegistration> registrationsRepo, IRepository<Guid, ObjectReceiving> receivingsRepo)
        {
            _registrationsRepo = registrationsRepo;
            _receivingsRepo = receivingsRepo;
        }

        public async Task<TransactionTodayStatsDto> Handle(TransactionStatsOverTodayQuery request, CancellationToken cancellationToken)
        {
            // This will resolve to MM/DD/YYYY 00:00:00
            var startDate = DateTime.UtcNow.Date;

            // This will resolve to MM/DD+1/YYYY 00:00:00
            var endDate = DateTime.UtcNow.Date.AddDays(1);

            var transesHourly = (from r in _receivingsRepo.Table
                                 where r.ReceivedAtUtc >= startDate && r.ReceivedAtUtc <= endDate
                                 group r by new
                                 {
                                     r.ReceivedAtUtc.Date,
                                     r.ReceivedAtUtc.Hour,
                                 } into g
                                 select new
                                 {
                                     Count = g.Count(),
                                     Date = g.Key.Date,
                                     Hour = g.Key.Hour
                                 });
            var transesHourlyFormated = transesHourly.Select(r => new
            {
                r.Count,
                DateTime = new DateTime(r.Date.Year, r.Date.Month, r.Date.Day, r.Hour, 0, 0)
            }).ToList();

            var hours = Enumerable.Range(0, 24).Select(offset =>
            {

                var dateTime = startDate.AddHours(offset);
                return dateTime;
            }).ToList();

            hours.ForEach(hour =>
            {
                if (!transesHourlyFormated.Any(t => t.DateTime == hour))
                {
                    transesHourlyFormated.Add(new
                    {
                        Count = 0,
                        DateTime = hour
                    });
                }
            });

            transesHourlyFormated = transesHourlyFormated.OrderBy(t => t.DateTime).ToList();
            var stats = transesHourlyFormated.Select(t => t.Count).ToList();

            var transactions = from r in _registrationsRepo.Table
                               where r.ShouldReturnItAfter.HasValue && r.Status == ObjectRegistrationStatus.OK
                               select r;
            var late = transactions
                .AsEnumerable()
                .Count(r => r.ObjectReceiving != null &&
                    r.ObjectReceiving.ObjectReturning != null &&
                    (r.ObjectReceiving.ReceivedAtUtc + r.ShouldReturnItAfter.Value) > r.ObjectReceiving.ObjectReturning.ReturnedAtUtc.AddMinutes(30));
            var notReturned = transactions.Count(r => r.ObjectReceiving != null && r.ObjectReceiving.ObjectReturning == null);
            var onTime = transactions
                .AsEnumerable()
                .Count(r => r.ObjectReceiving != null &&
                    r.ObjectReceiving.ObjectReturning != null &&
                    r.ObjectReceiving.ReceivedAtUtc.Add(r.ShouldReturnItAfter.Value) <= r.ObjectReceiving.ObjectReturning.ReturnedAtUtc.AddMinutes(30));

            return new TransactionTodayStatsDto
            {
                LateReturn = late,
                NotReturnedYet = notReturned,
                OnTimeReturn = onTime,
                TransactionsOverToday = stats
            };
        }

        public async Task<List<TransactionMonthlyStatsDto>> Handle(TransactionStatsOverMonthQuery request, CancellationToken cancellationToken)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-31);
            var endDate = DateTime.UtcNow.Date;

            var transes = (from r in _receivingsRepo.Table
                           where
                              r.ReceivedAtUtc.Date >= startDate && r.ReceivedAtUtc.Date <= endDate
                           group r by r.ReceivedAtUtc.Date into g
                           orderby g.Key
                           select new TransactionMonthlyStatsDto
                           {
                               Count = g.Count(),
                               Day = g.Key
                           }).ToList();
            var days = Enumerable.Range(0, 31).Select(offset => endDate.AddDays(-offset)).ToList();

            days.ForEach(day =>
            {
                if (!transes.Any(t => t.Day.Date == day.Date))
                {
                    transes.Add(new TransactionMonthlyStatsDto
                    {
                        Count = 0,
                        Day = day.Date,
                    });
                }
            });

            return transes.OrderBy(t => t.Day.Date).ToList();
        }

        public async Task<List<int>> Handle(TransactionStatsOverYearQuery request, CancellationToken cancellationToken)
        {
            // end Date 6/4/ 2020
            // start date 1/1/2019
            // the stats will
            // CurrentYear : 1/1/20, 1/2/20, 1/3/20, 1/4/20, 1/5/20 = 0, 1/6/20 = 0, 1/7/20 = 0, ...
            var startDate = DateTime.UtcNow.AddMonths(-DateTime.UtcNow.Month + 1).AddDays(-DateTime.UtcNow.Day + 1).Date;
            var endDate = DateTime.UtcNow.Date.AddDays(1);

            var transesMonthly = (from r in _receivingsRepo.Table
                                  where
                                     r.ReceivedAtUtc >= startDate && r.ReceivedAtUtc <= endDate
                                  group r by new
                                  {
                                      r.ReceivedAtUtc.Year,
                                      r.ReceivedAtUtc.Month
                                  } into g
                                  select new
                                  {
                                      Count = g.Count(),
                                      MonthYear = new DateTime(g.Key.Year, g.Key.Month, 1)
                                  }).ToList();

            var months = Enumerable.Range(0, 12).Select(offset => startDate.AddMonths(offset)).ToList();

            months.ForEach(month =>
            {
                if (!transesMonthly.Any(u => u.MonthYear.Year == month.Year && u.MonthYear.Month == month.Month))
                {
                    transesMonthly.Add(new
                    {
                        Count = 0,
                        MonthYear = month,
                    });
                }
            });

            return transesMonthly.Select(m => m.Count).ToList();
        }

    }
}

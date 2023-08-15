using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using RMALMS.Configuration;
using RMALMS.Constants;
using RMALMS.DomainServices;
using System;
using System.Threading.Tasks;

namespace RMALMS.BackgroundWorkers
{
    public class FinishCoursesBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {

        private readonly IQuizManager _quizManager;
        private readonly ITestAttemptManager _testAttemptManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ISettingManager settingManager;
        public FinishCoursesBackgroundWorker(
            AbpTimer timer,
            IQuizManager quizManager,
            ITestAttemptManager testAttemptManager,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager
            ) : base(timer)
        {
            this.settingManager = settingManager;
            Timer.Period = 1000;
            _quizManager = quizManager;
            _testAttemptManager = testAttemptManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            _ = FinishTest();
        }

        private async Task FinishTest()
        {
            if (Timer.Period == 1000)
            {
                Timer.Period = (int)TimeSpan.FromMinutes(int.Parse(settingManager.GetSettingValue(AppSettingNames.TimeScanFinishCourse))).TotalMilliseconds;
                return;
            }
            try
            {
                var now = DateTime.UtcNow;
              /*  using (_unitOfWorkManager.Current.DisableFilter(new string[] { AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant }))
                {
                    var allTestAttempt = await _testAttemptManager.GetAllTestingTestAttempt();
                    foreach (var item in allTestAttempt)
                    {
                        if (item.CreationTime.AddMinutes((double)item.TimeLimit) < now)
                        {
                            await _quizManager.ProcessScore(item.StudentId, item.TestAttemptId, item.PageType);
                        }
                    }
                }*/
            }
            catch (Exception ex)
            {
                Logger.Error("FinishTestBackGroundWorker-Error: " + ex.Message);
            }
        }
    }
}

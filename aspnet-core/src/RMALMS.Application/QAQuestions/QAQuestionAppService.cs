using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.Authorization.Users;
using RMALMS.Entities;
using RMALMS.Extension;
using RMALMS.IoC;
using RMALMS.Paging;
using RMALMS.QAQuestions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RMALMS.QAQuestionSignalR.SignalR;
using Microsoft.AspNetCore.SignalR;
using Abp.Application.Services;
using RMALMS.Courses.Dto;

namespace RMALMS.QAQuestions
{
    [AbpAuthorize]
    public class QAQuestionAppService : CrudApplicationBaseService<QAQuestion, QAQuestionDto, Guid, PagedResultRequestDto, QAQuestionCreateInput, QAQuestionUpdateInput>, IQAQuestionAppService
    {
        private readonly IWorkScope _ws;
        private readonly IHubContext<QnAHub> _hubQnAContext;
        public QAQuestionAppService(IRepository<Entities.QAQuestion, Guid> respository,
            IHubContext<QnAHub> hubChart,
            IHubContext<QnAHub> hubContext,
            IWorkScope ws) : base(respository)
        {
            _ws = ws;
            _hubQnAContext = hubContext;
        }
        #region for student        
        public async Task<QAQuestionOut> CreateQAQuestion(QAQuestionCreateInput input)
        {
            var item = new Entities.QAQuestion
            {
                //TenantId = input.TenantId,
                Title = input.Title,
                Content = input.Content,
                CourseInstanceId = input.CourseInstanceId
            };
            item.Id = await _ws.InsertAndGetIdAsync(item);

            var currentId = AbpSession.UserId;
            var user = new User();
            if (currentId.HasValue)
            {
                user = await _ws.GetRepo<User, long>().GetAsync(currentId.Value);
            }

            //return ObjectMapper.Map<QAQuestionOut>(item);
            var result = new QAQuestionOut
            {
                Id = item.Id,
                Title = input.Title,
                Content = input.Content,
                CreationTime = Uitls.DateTimeUtils.GetNow(),
                UserId = user.Id,
                ImageCover = user.Avatar,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.EmailAddress
            };
            await _hubQnAContext.Clients.All.SendAsync("CreateQAQuestion", result);
            return result;
        }


        public async override Task<QAQuestionDto> Update(QAQuestionUpdateInput input)
        {
            var item = await Repository.GetAsync(input.Id);
            ObjectMapper.Map(input, item);
            await Repository.UpdateAsync(item);
            return ObjectMapper.Map<QAQuestionDto>(item);
        }

        public async Task<QAAnswersDto> CreateQAAnswer(QAAnswerInput input)
        {

            var item = new QAAnswer
            {
                QuestionId = input.QuestionId,
                Content = input.Content,
                ResponseParentId = input.ResponseParentId,

            };
            item.Id = await _ws.InsertAndGetIdAsync(item);

            var currentId = AbpSession.UserId;
            var user = new User();
            if (currentId.HasValue)
            {
                user = await _ws.GetRepo<User, long>().GetAsync(currentId.Value);
            }
            var result = new QAAnswersDto
            {
                Id = item.Id,
                QuestionId = input.QuestionId,
                Content = input.Content,

                CreationTime = Uitls.DateTimeUtils.GetNow(),
                UserId = user.Id,
                ImageCover = user.Avatar,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.EmailAddress,
                PId = item.ResponseParentId
            };

            await _hubQnAContext.Clients.All.SendAsync("qaquestion", result);
            return result;
        }

        public async Task<QAAnswersDto> UpdateQAAnswer(IdContentInput input)
        {
            var item = await _ws.GetRepo<QAAnswer>().GetAsync(input.Id);
            item.Content = input.Content;
            await _ws.GetRepo<QAAnswer>().UpdateAsync(item);
            return new QAAnswersDto
            {
                Id = item.Id,
            };
        }

        [HttpPost]
        public async Task<GridResult<QAQuestionOut>> GetQAQuestionAnswer(GridQAAnswerParam input)
        {
            var currentId = AbpSession.UserId;
            var qFollowerQuestion = _ws.GetRepo<UserFollowQAQuestion>().GetAll().Where(u => u.UserId == currentId).Select(m => m.FollowId).Distinct();

            var qQnA = from question in _ws.GetRepo<Entities.QAQuestion, Guid>().GetAll().Where(q => q.CourseInstanceId == input.courseInstanceId)
                       join qFQ in qFollowerQuestion
                       on question.Id equals qFQ into qFQs

                       join aw in _ws.GetRepo<QAAnswer, Guid>().GetAll().Where(m => m.ResponseParentId == null)
                       on question.Id equals aw.QuestionId into aws

                       join user in _ws.GetRepo<User, long>().GetAll()
                       on question.CreatorUserId equals user.Id

                       select new QAQuestionOut
                       {
                           Id = question.Id,
                           Title = question.Title,
                           Content = question.Content,
                           CreationTime = question.CreationTime,
                           Responses = aws.Count(),
                           IsFollow = qFQs.Count() > 0 ? true : false,
                           UserId = user.Id,
                           ImageCover = user.Avatar,
                           UserName = user.UserName,
                           FullName = user.FullName,
                           Email = user.EmailAddress
                       };
            if (input.IsResponse)
            {
                qQnA = qQnA.Where(m => m.Responses == 0);
            }
            if (input.IsFollower)
            {
                qQnA = qQnA.Where(m => m.IsFollow);
            }
            if (input.Sort == "date_asc")
            {
                qQnA = qQnA.OrderByDescending(m => m.CreationTime);
            }
            if (input.Sort == "date_desc")
            {
                qQnA = qQnA.OrderBy(m => m.CreationTime);
            }
            return await qQnA.GetGridResult(qQnA, input.input);

        }


        private async Task<bool> AddOrUpdateTeacherViewDiscussions(Guid Id, string type)
        {
            var currentId = AbpSession.UserId;
            if (!currentId.HasValue)
            {
                return false;
            }
            var exist = await _ws.GetRepo<TeacherViewDiscussion, Guid>().GetAll().IgnoreQueryFilters().AnyAsync(m => m.QAId == Id && m.CreatorUserId == currentId && m.QAType == type);
            if (exist)
            {
                return true;
            }
            var teacherView = new TeacherViewDiscussion
            {
                CreatorUserId = currentId,
                QAId = Id,
                QAType = type,
            };
            await _ws.GetRepo<TeacherViewDiscussion>().InsertAsync(teacherView);
            return true;
        }

        public async Task<ListResultDto<QAAnswersDto>> GetQAAnswerByQAQuestionId(Guid QAQuestionId)
        {
            var currentId = AbpSession.UserId;
            var qFollowerQuestion = _ws.GetRepo<UserFollowQAQuestion>().GetAll().Where(u => u.UserId == currentId).Select(m => m.FollowId).Distinct();

            var qAnswer_child = from aw in _ws.GetRepo<QAAnswer, Guid>().GetAll().Where(m => m.ResponseParentId != null)
                                join user in _ws.GetRepo<User, long>().GetAll()
                                on aw.CreatorUserId equals user.Id
                                select new QAAnswersDto
                                {
                                    Content = aw.Content,
                                    Id = aw.Id,
                                    QuestionId = aw.QuestionId,
                                    CreationTime = aw.CreationTime,
                                    UserId = user.Id,
                                    ImageCover = user.Avatar,
                                    UserName = user.UserName,
                                    FullName = user.FullName,
                                    Email = user.EmailAddress,
                                    PId = aw.ResponseParentId,
                                };

            var qQAAnswer = from answer in _ws.GetRepo<QAAnswer, Guid>().GetAll().Where(m => m.QuestionId == QAQuestionId && m.ResponseParentId == null)
                            join qFQ in qFollowerQuestion
                            on answer.Id equals qFQ into qFQs

                            join child in qAnswer_child
                            on answer.Id equals child.PId into chids

                            join user in _ws.GetRepo<User, long>().GetAll()
                            on answer.CreatorUserId equals user.Id


                            select new QAAnswersDto
                            {
                                Content = answer.Content,
                                Id = answer.Id,
                                CreationTime = answer.CreationTime,
                                IsFollow = qFQs.Count() > 0 ? true : false,
                                UserId = user.Id,
                                ImageCover = user.Avatar,
                                UserName = user.UserName,
                                FullName = user.FullName,
                                Email = user.EmailAddress,
                                Answers = chids,
                                NumberAnswer = chids.Count(),
                            };
            // Update viewed to TeacherViewDiscussions
            if (currentId.HasValue)
            {
                await AddOrUpdateTeacherViewDiscussions(QAQuestionId, nameof(QAQuestion));
                foreach (var answer in qQAAnswer)
                {
                    await AddOrUpdateTeacherViewDiscussions(answer.Id, nameof(QAAnswer));
                }
            }
            var result = await qQAAnswer.ToListAsync();
            return new ListResultDto<QAAnswersDto>(result);
        }

        public async Task<Boolean> AddUserFollowQA(IdContentInput input)
        {
            var currentId = AbpSession.UserId;
            if (!currentId.HasValue)
            {
                return false;
            }
            var exist = await _ws.GetRepo<UserFollowQAQuestion>().GetAll().IgnoreQueryFilters().AnyAsync(m => m.UserId == currentId && m.FollowId == input.Id);
            if (exist)
            {
                return false;
            }
            UserFollowQAQuestion ufQAQuestion = new UserFollowQAQuestion();
            {
                ufQAQuestion.UserId = currentId.Value;
                ufQAQuestion.FollowId = input.Id;
                ufQAQuestion.FollowType = input.Content;
                ufQAQuestion.TenantId = AbpSession.TenantId;
            };
            await _ws.GetRepo<UserFollowQAQuestion>().InsertAsync(ufQAQuestion);
            return true;
        }
        [HttpPost]
        public async Task<Boolean> DeleteUserFollowQA(IdContentInput input)
        {
            var currentId = AbpSession.UserId;
            if (!currentId.HasValue)
            {
                return false;
            }
            var item = await _ws.GetRepo<UserFollowQAQuestion>().GetAll().IgnoreQueryFilters().FirstOrDefaultAsync(m => m.UserId == currentId && m.FollowId == input.Id);
            if (item != null)
            {
                await _ws.GetRepo<UserFollowQAQuestion>().DeleteAsync(item);
                return true;
            }
            else
                return false;
        }



        #endregion for student
        #region for admin

        [HttpPost]
        public async Task<GridResult<DiscussionOut>> GetsDiscussionByCourseInstanceIdPagging(GridDiscussionParam input)
        {

            var qAnswer_child = from aw in _ws.GetRepo<QAAnswer, Guid>().GetAll().Where(m => m.ResponseParentId != null)
                                join user in _ws.GetRepo<User, long>().GetAll()
                                on aw.CreatorUserId equals user.Id
                                select new QAAnswersDto
                                {
                                    Content = aw.Content,
                                    Id = aw.Id,
                                    QuestionId = aw.QuestionId,
                                    CreationTime = aw.CreationTime,
                                    UserId = user.Id,
                                    ImageCover = user.Avatar,
                                    UserName = user.UserName,
                                    FullName = user.FullName,
                                    Email = user.EmailAddress,
                                    PId = aw.ResponseParentId,
                                };
            var qAnswer = from aw in _ws.GetRepo<QAAnswer, Guid>().GetAll().Where(m => m.ResponseParentId == null)
                          join user in _ws.GetRepo<User, long>().GetAll()
                          on aw.CreatorUserId equals user.Id

                          join child in qAnswer_child
                          on aw.Id equals child.PId into chids

                          select new QAAnswersDto
                          {
                              Content = aw.Content,
                              Id = aw.Id,
                              QuestionId = aw.QuestionId,
                              CreationTime = aw.CreationTime,
                              UserId = user.Id,
                              ImageCover = user.Avatar,
                              UserName = user.UserName,
                              FullName = user.FullName,
                              Email = user.EmailAddress,
                              Answers = chids.Select(p => new QAAnswersDto
                              {
                                  Content = p.Content,
                                  Id = p.Id,
                                  QuestionId = p.QuestionId,
                                  CreationTime = p.CreationTime,
                                  UserId = p.UserId,
                                  ImageCover = p.ImageCover,
                                  UserName = p.UserName,
                                  FullName = p.FullName,
                                  Email = p.Email,
                              }),
                          };

            var tcvDiscussion = _ws.GetRepo<TeacherViewDiscussion, Guid>().GetAll().Where(m => m.CreatorUserId == AbpSession.UserId);

            var qQnA = from question in _ws.GetRepo<QAQuestion, Guid>().GetAll().Where(q => q.CourseInstanceId == input.courseInstanceId)

                       join aw in qAnswer
                       on question.Id equals aw.QuestionId into aws

                       join user in _ws.GetRepo<User, long>().GetAll()
                       on question.CreatorUserId equals user.Id

                       let _reponse = aws.Where(m => m.UserId != AbpSession.UserId).Select(m => m.Id).Except(tcvDiscussion.Where(m => m.QAType == nameof(QAAnswer)).Select(p => p.QAId))
                       let _teacherView = tcvDiscussion.Where(m => m.QAId == question.Id)

                       select new DiscussionOut
                       {
                           Id = question.Id,
                           Title = question.Title,
                           Content = question.Content,
                           CreationTime = question.CreationTime,
                           QAAnswers = aws.Select(m => new QAAnswersDto
                           {
                               Content = m.Content,
                               Id = m.Id,
                               CreationTime = m.CreationTime,
                               UserId = m.UserId,
                               ImageCover = m.ImageCover,
                               UserName = m.UserName,
                               FullName = m.FullName,
                               Email = m.Email,
                               Answers = m.Answers,
                               NumberAnswer = m.Answers.Count(),

                           }),
                           Responses = _reponse.Count(), // aws.Count(),
                           IsNew = _teacherView.Count() > 0 ? false : true,

                           UserId = user.Id,
                           ImageCover = user.Avatar,
                           UserName = user.UserName,
                           FullName = user.FullName,
                           Email = user.EmailAddress
                       };

            qQnA = qQnA.OrderByDescending(m => m.CreationTime);
            return await qQnA.GetGridResult(qQnA, input.input);
        }

        public async Task<bool> DeleteQAQuestion(IdContentInput input)
        {
            // Remove all answer of the question
            var qAnswer = _ws.GetRepo<QAAnswer, Guid>().GetAll().Where(m => m.QuestionId == input.Id);
            foreach (var item in qAnswer)
            {
                await _ws.GetRepo<QAAnswer, Guid>().DeleteAsync(item);
            }
            await _ws.GetRepo<QAQuestion, Guid>().DeleteAsync(input.Id);
            return true;
        }
        [HttpPost]
        public async Task<bool> DeleteQAAnswer(IdContentInput input)
        {
            var ans = await _ws.GetRepo<QAAnswer, Guid>().GetAsync(input.Id); // Check exist
            await _ws.GetRepo<QAAnswer, Guid>().DeleteAsync(ans);
            await _hubQnAContext.Clients.All.SendAsync("delete_qaanswer", input.Id);
            return true;
        }

        [HttpPost]
        public async Task<bool> DeleteFAQQuestion(IdContentInput input)
        {
            await _ws.GetRepo<FAQQuestion, Guid>().DeleteAsync(input.Id);
            return true;
        }

        public async Task<ListResultDto<FAQQuestionDto>> GetsFAQQuestionByCourseId(IdContentInput input)
        {
            var qFAQ = from fQ in _ws.GetRepo<FAQQuestion, Guid>().GetAll().Where(m => m.CourseId == input.Id)

                       select new FAQQuestionDto
                       {
                           Id = fQ.Id,
                           Content = fQ.Content,
                           Title = fQ.Title,
                           SequenceOrder = fQ.SequenceOrder,
                           CreationTime = fQ.CreationTime,
                       };
            if (!string.IsNullOrEmpty(input.Content))
            {
                var txtSearch = input.Content.Trim().ToLower();
                qFAQ = qFAQ.Where(m => m.Title.ToLower().Contains(txtSearch) || m.Content.ToLower().Contains(txtSearch));
            }
            var result = await qFAQ.OrderBy(m => m.SequenceOrder).ThenByDescending(m => m.CreationTime).ToListAsync();
            return new ListResultDto<FAQQuestionDto>(result);
        }
        public async Task<ListResultDto<FAQQnADto>> GetsFAQQnAByCourseId(Guid CourseId)
        {
            var qFAQ = from fQ in _ws.GetRepo<FAQQuestion, Guid>().GetAll().Where(m => m.CourseId == CourseId)
                       join fA in _ws.GetRepo<FAQAnswer, Guid>().GetAll()
                       on fQ.Id equals fA.QuestionId into faqAns
                       select new FAQQnADto
                       {
                           Id = fQ.Id,
                           Content = fQ.Content,
                           Title = fQ.Title,
                           SequenceOrder = fQ.SequenceOrder,
                           FAQAnswers = faqAns.Select(p => new FAQAnswerDto
                           {
                               Id = p.Id,
                               Content = p.Content,
                               SequenceOrder = p.SequenceOrderId,
                           }).OrderBy(m => m.SequenceOrder),
                           //CreationTime = fQ.CreationTime,
                       };
            var result = await qFAQ.OrderBy(m => m.SequenceOrder).ToListAsync();
            return new ListResultDto<FAQQnADto>(result);
        }

        public async Task<FAQQuestionDto> CreateFAQQuestion(FAQQuestionInput input)
        {
            var item = new FAQQuestion
            {
                CourseId = input.CourseId,
                Title = input.Title,
                Content = input.Content,
                SequenceOrder = 1
            };
            item.Id = await _ws.InsertAndGetIdAsync(item);
            return new FAQQuestionDto
            {
                Id = item.Id,
                Content = input.Content
            };

        }

        public async Task<FAQQuestionDto> UpdateFAQQuestion(FAQQuestionInput input)
        {
            var item = await _ws.GetRepo<FAQQuestion>().GetAsync(input.CourseId);
            item.Content = input.Content;
            item.Title = input.Title;
            await _ws.GetRepo<FAQQuestion>().UpdateAsync(item);
            return new FAQQuestionDto
            {
                Id = item.Id,
            };

        }

        public async Task<FAQAnswerDto> CreateFAQAnswer(FAQAnswerInput input)
        {
            var item = new FAQAnswer
            {
                QuestionId = input.QuestionId,
                Content = input.Content,
                SequenceOrderId = 1,

            };
            item.Id = await _ws.InsertAndGetIdAsync(item);
            return new FAQAnswerDto
            {
                Content = input.Content,
                QuestionId = input.QuestionId,
            };
        }

        public async Task<Boolean> UpdateFAQQuestionSequenceOrder(IEnumerable<FAQSequenceOrderInput> inputs)
        {
            foreach (var input in inputs)
            {
                var item = await _ws.GetRepo<FAQQuestion>().GetAsync(input.Id);
                item.SequenceOrder = input.SequenceOrder;
                await _ws.GetRepo<FAQQuestion>().UpdateAsync(item);
            }
            return true;
        }

        #endregion for admin


    }
}

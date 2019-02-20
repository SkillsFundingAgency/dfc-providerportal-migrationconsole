
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Dfc.CourseDirectory.Common;
using Dfc.CourseDirectory.Common.Interfaces;
using Dfc.CourseDirectory.Models.Interfaces.Courses;
using Dfc.CourseDirectory.Services.Interfaces.CourseService;
using Newtonsoft.Json;
using Dfc.CourseDirectory.Models.Models.Courses;
using System.Net;

namespace Dfc.CourseDirectory.Services.CourseService
{
    public class CourseService : ICourseService
    {
        private readonly ILogger<CourseService> _logger;
        private readonly HttpClient _httpClient;
        private readonly Uri _addCourseUri;
        private readonly Uri _deleteCoursesByUKPRNUri;

        public CourseService(
            ILogger<CourseService> logger,
            HttpClient httpClient,
            IOptions<CourseServiceSettings> settings)
        {
            Throw.IfNull(logger, nameof(logger));
            Throw.IfNull(httpClient, nameof(httpClient));
            Throw.IfNull(settings, nameof(settings));

            _logger = logger;
            _httpClient = httpClient;

            _addCourseUri = settings.Value.ToAddCourseUri();
            _deleteCoursesByUKPRNUri = settings.Value.ToDeleteCoursesByUKPRNUri();
        }

        public async Task<IResult<ICourse>> AddCourseAsync(ICourse course)
        {
            _logger.LogMethodEnter();
            Throw.IfNull(course, nameof(course));

            try
            {
                _logger.LogInformationObject("Course add object.", course);
                _logger.LogInformationObject("Course add URI", _addCourseUri);

                var courseJson = JsonConvert.SerializeObject(course);

                var content = new StringContent(courseJson, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_addCourseUri, content);

                _logger.LogHttpResponseMessage("Course add service http response", response);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    _logger.LogInformationObject("Course add service json response", json);


                    var courseResult = JsonConvert.DeserializeObject<Course>(json);


                    return Result.Ok<ICourse>(courseResult);
                }
                else if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    return Result.Fail<ICourse>("Course add service unsuccessful http response - TooManyRequests");
                }
                else
                {
                    return Result.Fail<ICourse>("Course add service unsuccessful http response - ResponseStatusCode: " + response.StatusCode);
                }
            }
            catch (HttpRequestException hre)
            {
                _logger.LogException("Course add service http request error", hre);
                return Result.Fail<ICourse>("Course add service http request error.");
            }
            catch (Exception e)
            {
                _logger.LogException("Course add service unknown error.", e);

                return Result.Fail<ICourse>("Course add service unknown error.");
            }
            finally
            {
                _logger.LogMethodExit();
            }
        }


        public async Task<IResult<List<string>>> DeleteCoursesByUKPRNAsync(IDeleteCoursesByUKPRNCriteria criteria)
        {
            Throw.IfNull(criteria, nameof(criteria));
            Throw.IfLessThan(0, criteria.UKPRN.Value, nameof(criteria.UKPRN.Value));
            _logger.LogMethodEnter();

            try
            {
                _logger.LogInformationObject("Delete Courses By UKPRN criteria", criteria);
                _logger.LogInformationObject("Delete Courses By UKPRN URI", _deleteCoursesByUKPRNUri);

                if (!criteria.UKPRN.HasValue)
                    return Result.Fail<List<string>>("Delete Courses By UKPRN - unknown UKRLP");

                var response = await _httpClient.GetAsync(new Uri(_deleteCoursesByUKPRNUri.AbsoluteUri + "&UKPRN=" + criteria.UKPRN));
                _logger.LogHttpResponseMessage("Delete Courses By UKPRN service http response", response);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    if (!json.StartsWith("["))
                        json = "[" + json + "]";

                    _logger.LogInformationObject("Delete Courses By UKPRN json response", json);
                    List<string> messagesList = JsonConvert.DeserializeObject<List<string>>(json);

                    return Result.Ok<List<string>>(messagesList);
                }
                else
                {
                    return Result.Fail<List<string>>("Delete Courses By UKPRN service unsuccessful http response");
                }

            }
            catch (HttpRequestException hre)
            {
                _logger.LogException("Delete Courses By UKPRN service http request error", hre);
                return Result.Fail<List<string>>("Delete Courses By UKPRN service http request error.");

            }
            catch (Exception e)
            {
                _logger.LogException("Delete Courses By UKPRN service unknown error.", e);
                return Result.Fail<List<string>>("Delete Courses By UKPRN service unknown error.");

            }
            finally
            {
                _logger.LogMethodExit();
            }
        }

    }

    internal static class CourseServiceSettingsExtensions
    {
        internal static Uri ToAddCourseUri(this ICourseServiceSettings extendee)
        {
            return new Uri($"{extendee.ApiUrl + "AddCourse?code=" + extendee.ApiKey}");
        }
        internal static Uri ToDeleteCoursesByUKPRNUri(this ICourseServiceSettings extendee)
        {
            return new Uri($"{extendee.ApiUrl + "DeleteCoursesByUKPRN?code=" + extendee.ApiKey}");
        }
    }
}

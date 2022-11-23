using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using QuizService.Model;
using Xunit;
using Xunit.Abstractions;

namespace QuizService.Tests;

public class QuizzesControllerTest
{
    const string QuizApiEndPoint = "/api/quizzes/";

    private readonly ITestOutputHelper _output;

    public QuizzesControllerTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task PostNewQuizAddsQuiz()
    {
        var quiz = new QuizCreateModel("Test title");
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
                content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }
    }

    [Fact]
    public async Task AQuizExistGetReturnsQuiz()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 1;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
            var quiz = JsonConvert.DeserializeObject<QuizResponseModel>(await response.Content.ReadAsStringAsync());
            Assert.Equal(quizId, quiz.Id);
            Assert.Equal("My first quiz", quiz.Title);
        }
    }

    [Fact]
    public async Task AQuizDoesNotExistGetFails()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]
        
    public async Task AQuizDoesNotExists_WhenPostingAQuestion_ReturnsNotFound()
    {
        const string QuizApiEndPoint = "/api/quizzes/999/questions";

        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var question = new QuestionCreateModel("The answer to everything is what?");
            var content = new StringContent(JsonConvert.SerializeObject(question));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"),content);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Theory]
    [InlineData(new int[] { 1, 5, 7, 10 }, 4)]
    [InlineData(new int[] { 1, 4, 7, 10 }, 3)]
    [InlineData(new int[] { 2, 5, 7, 9 }, 2)]
    [InlineData(new int[] { 0, 5, 6, 9 }, 1)]
    [InlineData(new int[] { 0, 3, 6, 8 }, 0)]
    public async Task QuizTaken_WhenAnsweredCorrectly_ReturnsScore(int[] userAnswers, int expectedScore)
    {
        const string QuizApiEndPoint = "/api/quizzes/";

        List<int> answers = new();

        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();

            foreach (var quizId in new int[] {1,2})
            {
                var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var quiz = await response.Content.ReadFromJsonAsync<QuizResponseModel>();

                // simulate displaying
                foreach (var item in quiz.Questions)
                {
                    _output.WriteLine(item.Text);

                    foreach (var answer in item.Answers)
                    {
                        _output.WriteLine(answer.Text);
                    }

                    answers.Add(item.CorrectAnswerId);
                }
            }

            Assert.Equal(answers.Count, userAnswers.Length);

            // simulate answering and scoring
            int score = 0;

            for (int i = 0; i < answers.Count; i++)
            {
                score += userAnswers[i] == answers[i] ? 1 : 0;

                _output.WriteLine($"Correct answer:{answers[i]} User gave an answer: {userAnswers[i]}");
            }

            Assert.Equal(expectedScore, score);
        }
    }
}
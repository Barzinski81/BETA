using RestSharpServices;
using System.Net;
using System.Reflection.Emit;
using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using NUnit.Framework.Internal;
using RestSharpServices.Models;
using System;

namespace TestGitHubApi
{
    public class TestGitHubApi
    {
        private GitHubApiClient client;
        private readonly string repo = "test-nakov-repo";
        private static long lastCreatedIssueNumber;
        private static long lastCreatedCommentId;

        [SetUp]
        public void Setup()
        {            
            client = new GitHubApiClient("https://api.github.com/repos/testnakov/", "username", "token");
        }


        [Test, Order (1)]
        public void Test_GetAllIssuesFromARepo()
        {
          
            var issues = client.GetAllIssues(repo);

            Assert.That(issues, Has.Count.GreaterThan(1), "There should be more than one issue.");

            foreach (var issue in issues)
            {
                Assert.That(issue.Id, Is.GreaterThan(0), "Issue ID should be greater than 0");
                Assert.That(issue.Number, Is.GreaterThan(0), "Issue Number should be greater than 0");
                Assert.That(issue.Title, Is.Not.Empty, "Issue Title should not be empty");
            }
        }

        [Test, Order (2)]
        public void Test_GetIssueByValidNumber()
        {
            int issueNumber = 1;
            var issue = client.GetIssueByNumber(repo, issueNumber);

            Assert.That(issue, Is.Not.Null, "The response should contain issue data.");
            Assert.That(issue.Id, Is.GreaterThan(0), "The issue ID should be a positive long.");
            Assert.That(issue.Number, Is.EqualTo(issueNumber), "The issue number should match the requested number.");
        }
        
        [Test, Order (3)]
        public void Test_GetAllLabelsForIssue()
        {
            int issueNumber = 6;
            var labels = client.GetAllLabelsForIssue(repo, issueNumber);

            Assert.That(labels.Count, Is.GreaterThan(0));

            foreach(var label in labels)
            {
                Assert.That(label.Id, Is.GreaterThan(0), "Label ID should be greater than 0.");
                Assert.That(label.Name, Is.Not.Empty, "Label Name should not be empty.");

                Console.WriteLine($"Label: {label.Id} - Name: {label.Name}");
            }
        }

        [Test, Order (4)]
        public void Test_GetAllCommentsForIssue()
        {
            int issueNumber = 6;
            var comments = client.GetAllCommentsForIssue(repo,issueNumber);

            Assert.That(comments.Count, Is.GreaterThan(0));

            foreach (var comment in comments)
            {
                Assert.That(comment.Id, Is.GreaterThan(0), "Commend ID should be greater that 0.");
                Assert.That(comment.Body, Is.Not.Empty, "Comment body should not be empty");

                Console.WriteLine($"Comment: {comment.Id} - Body: {comment.Body}");
            }
        }

        [Test, Order(5)]
        public void Test_CreateGitHubIssue()
        {
            string expectedTitle = "Create Your Own Title";
            string body = "Give Some Description";
            var issue = client.CreateIssue(repo, expectedTitle, body);

            Assert.Multiple(() =>
            {
                Assert.That(issue.Id, Is.GreaterThan(0));
                Assert.That(issue.Number, Is.GreaterThan(0));
                Assert.That(issue.Title, Is.Not.Empty);
                Assert.That(issue.Title, Is.EqualTo(expectedTitle));
            });

            Console.WriteLine(issue.Number);
            lastCreatedIssueNumber = issue.Number;
        }

        [Test, Order (6)]
        public void Test_CreateCommentOnGitHubIssue()
        {
            long issueNumber = lastCreatedIssueNumber;
            string expectedBody = "Let me see";

            var comment = client.CreateCommentOnGitHubIssue(repo, issueNumber, expectedBody);
            Assert.That(comment.Body, Is.EqualTo(expectedBody));
            Console.WriteLine(comment.Id);
            lastCreatedCommentId = comment.Id;
        }

        [Test, Order (7)]
        public void Test_GetCommentById()
        {
            long commentId = lastCreatedCommentId;
            string expectedBody = "Let me see";

            Comment comment = client.GetCommentById(repo, commentId);

            Assert.That(comment, Is.Not.Null, "Expect to retrieve a comment that is not null.");
            Assert.That(comment.Id, Is.EqualTo(commentId), "The retrieved and request comment IDs should match");
            Assert.That(comment.Body, Is.EqualTo(expectedBody), "The retrieved comment body should match the expected body.");
        }
         

        [Test, Order (8)]
        public void Test_EditCommentOnGitHubIssue()
        {
            long commentId = lastCreatedCommentId;
            string newBody = "Updated text on the comment.";

            var updatedComment = client.EditCommentOnGitHubIssue (repo, commentId, newBody);

            Assert.That(updatedComment, Is.Not.Null, "The updated comment should not be null.");
            Assert.That(updatedComment.Id, Is.EqualTo(commentId), "The retrieved comment IDs should match the original comment ID.");
            Assert.That(updatedComment.Body, Is.EqualTo(newBody), "The updated comment text should match the new body text.");

        }

        [Test, Order (9)]
        public void Test_DeleteCommentOnGitHubIssue()
        {
            long commentId = lastCreatedCommentId;

            bool result = client.DeleteCommentOnGitHubIssue(repo, commentId);

            Assert.That(result, Is.True, "The comment should be successfully deleted.");
        }


    }
}


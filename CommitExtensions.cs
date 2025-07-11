using System;
using LibGit2Sharp;

namespace SemanticRelease.NotesGenerator
{
    public static class CommitExtensions
    {
        public static string ToDescription(this Commit commit, Repository repo)
        {
            var shortHash = commit.Sha[..7];
            // TODO: Eventually add scope
            var message = commit.Message.Split('\n')[0].Split(':')[1].Trim();
            var commitUrl = repo.ToCommitUrl(commit.Sha);
            return $"* {message} - {commit.Committer.When.Date:yyyy MMMM dd} - ([{shortHash}]({commitUrl}))";
        }

        public static string ToCommitUrl(this Repository repo, string commitHash)
        {
            var origin = repo.Network.Remotes["origin"];
            if (origin == null)
            {
                throw new InvalidOperationException($"Repository {repo.Info.Path} does not have an origin");
            }
            
            var remoteUrl = origin.Url;
            if (!remoteUrl.Contains("github.com"))
            {
                throw new InvalidOperationException("Non GitHub repositories are not supported");
            }

            var cleanedUrl = remoteUrl
                .Replace(".git", "")
                .Replace("git@github.com:", "https://github.com/")
                .Replace("https://github.com/", "https://github.com/");
            
            return $"{cleanedUrl}/commit/{commitHash}";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using SemanticRelease.Abstractions;

namespace SemanticRelease.NotesGenerator
{
    public class NotesGenerator : ISemanticPlugin
    {
        private readonly Dictionary<string, string> _commitTypes = new Dictionary<string, string>
        {
            { "!", "Breaking Changes" },
            { "feat", "Features" },
            { "fix", "Bug Fixes" },
            { "docs", "Documentation" },
            { "refactor", "Code Refactoring" },
            { "perf", "Performance Improvements" },
            { "test", "Tests" },
            { "chore", "Chores" },
        };

        public void Register(SemanticLifecycle lifecycle)
        {
            lifecycle.OnGenerateNotes(GenerateNotesAsync);
        }

        private Task GenerateNotesAsync(ReleaseContext context)
        {
            Console.WriteLine("Beginning step 'generateNotes' for plugin 'NotesGenerator'");
            if (!(context.PluginData["commits"] is List<Commit> commits) || commits.Count == 0)
            {
                Console.WriteLine("No commits found");
                Console.WriteLine("Ending step 'generateNotes' for plugin 'NotesGenerator'");
                return Task.CompletedTask;
            }

            var typedCommits = new Dictionary<string, List<Commit>>();

            foreach (var commit in commits)
            {
                var containsBreaking = commit.Message.Contains("BREAKING CHANGE") ||
                                       commit.Message.Split('\n')[0].Split(':')[0].EndsWith('!');
                if (containsBreaking)
                {
                    if (!typedCommits.TryGetValue("Breaking Changes", out var breakingChangeList))
                    {
                        breakingChangeList = new List<Commit>();
                        typedCommits.Add("Breaking Changes", breakingChangeList);
                    }

                    breakingChangeList.Add(commit);
                    continue;
                }

                var type = commit.Message.Split('\n')[0].Split(':')[0].ToLower();
                if (type.Contains('('))
                    type = type.Split('(')[0];
                var sectionHeader = _commitTypes.GetValueOrDefault(type);
                if (sectionHeader == null)
                    continue;
                if (!typedCommits.TryGetValue(sectionHeader, out var list))
                {
                    list = new List<Commit>();
                    typedCommits.Add(sectionHeader, list);
                }

                list.Add(commit);
            }

            var repo = new Repository(context.WorkingDirectory);
            var remoteUrl = repo.Network.Remotes["origin"];
            if (remoteUrl == null)
                throw new InvalidOperationException("Repository does not have an origin");
            var cleanedUrl = remoteUrl.Url
                .Replace(".git", "")
                .Replace("git@github.com:", "https://github.com/")
                .Replace("https://github.com/", "https://github.com/");
            var notes = "";
            var versionString =
                $"([{context.PluginData["nextVersion"] ?? throw new InvalidOperationException("Cannot generate notes without next version")}]({cleanedUrl}/compare/{((string)context.PluginData["lastTag"] == "0.0.0" ? commits[0].Sha[..7] : context.PluginData["lastTag"])}...{context.PluginData["nextVersion"] ?? throw new InvalidOperationException("Cannot generate notes without next version")})) ({DateTime.Now:MM/dd/yyyy})";
            notes += $"## {versionString}\n\n";
            foreach (var (sectionHeader, commitList) in typedCommits)
            {
                if (sectionHeader.IsNullOrEmpty() || commitList.Count == 0)
                {
                    continue;
                }

                notes += $"### {sectionHeader}\n\n";
                var section = commitList.Select(c => c.ToDescription(repo))
                    .Aggregate((a, b) => $"{a}\n{b}");
                notes += $"{section}\n\n";
            }
            
            Console.WriteLine(notes);
            Console.WriteLine("Ending step 'generateNotes' for plugin 'NotesGenerator'");
            
            context.PluginData["releaseNotesMD"] = notes;
            
            return Task.CompletedTask;
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using LibGit2Sharp;
using SemanticRelease.Abstractions;

namespace SemanticRelease.NotesGenerator
{
    public class NotesGenerator : ISemanticPlugin
    {
        private readonly Dictionary<string, string> _commitTypes = new Dictionary<string, string>
        {
            {"!", "Breaking Changes"},
            {"feat", "Features"},
            {"fix", "Bug Fixes"},
            {"docs", "Documentation"},
            {"refactor", "Code Refactoring"},
            {"perf", "Performance Improvements"},
            {"test", "Tests"},
            {"chore", "Chores"},
        };
        public void Register(SemanticLifecycle lifecycle)
        {
            lifecycle.OnGenerateNotes(GenerateNotesAsync);
        }
        
        private async Task GenerateNotesAsync(ReleaseContext context)
        {
            if (!(context.PluginData["commits"] is List<Commit> commits) || commits.Count == 0)
            {
                return;
            }

            var typedCommits = new Dictionary<string, List<Commit>>();

            foreach (var commit in commits)
            {
                var containsBreaking = commit.Message.Contains("BREAKING CHANGE") || commit.Message.Split('\n')[0].Split(':')[0].EndsWith('!');
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
            
            
        }
    }
}
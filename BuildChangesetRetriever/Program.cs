using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildChangesetRetriever
{
    class Program
    {
        static void Main(string[] args)
        {
            LogArguments(args);

            string repositorySourceFolder = Path.GetFullPath(args.ElementAt(0));
            string lastCommitid = args.ElementAt(1);
            ChangesetOperation operation = (ChangesetOperation)Enum.Parse(typeof(ChangesetOperation), args.ElementAt(2));

            Console.WriteLine("Repository source folder: " + repositorySourceFolder);
            Console.WriteLine("Last commit identifier:   " + lastCommitid);
            Console.WriteLine("Changeset operation:      " + operation);

            using (Repository repository = new Repository(repositorySourceFolder))
            {
                Commit currentCommit = repository.Commits.QueryBy(new CommitFilter()).FirstOrDefault(c => c.Sha == lastCommitid);
                if (null == currentCommit)
                {
                    throw new ArgumentException(string.Format("Could not retrieve the commit with identifier '{0}'", lastCommitid));
                }

                LogCommit(currentCommit, "Current commit");

                TreeChanges changeLog = null;

                switch (operation)
                {
                    case ChangesetOperation.DifferenceWithLastCommit:
                        changeLog = PerformDifferenceWithLastCommit(repository, currentCommit);
                        break;

                    case ChangesetOperation.DifferenceWithMaster:
                        changeLog = PerformDifferenceWithMaster(repository, currentCommit);
                        break;

                    case ChangesetOperation.DifferenceWithLastRelease:
                        changeLog = PerformDifferenceWithLastRelease(repository, currentCommit);
                        break;
                }

                LogChanges(changeLog);
            }
        }

        private static TreeChanges PerformDifferenceWithLastCommit(Repository repository, Commit currentCommit)
        {
            Commit preceedingCommit = repository.Commits.QueryBy(new CommitFilter() { SortBy = CommitSortStrategies.Reverse })
                                                     .GetPrecedingElement<Commit>(() => currentCommit);

            LogCommit(preceedingCommit, "Preceding commit");

            TreeChanges changeLog = repository.Diff.Compare<TreeChanges>(preceedingCommit.Tree, DiffTargets.Index);

            return changeLog;
        }

        private static TreeChanges PerformDifferenceWithMaster(Repository repository, Commit currentCommit)
        {
            throw new NotImplementedException();
        }

        private static TreeChanges PerformDifferenceWithLastRelease(Repository repository, Commit currentCommit)
        {
            throw new NotImplementedException();
        }

        private static void LogChanges(TreeChanges changeLog)
        {
            string s = "";
            Console.WriteLine("File Added: " + s.JoinWithPlaceholder(", ", "0", changeLog.Added.Select(x => x.Path).ToArray()));
            Console.WriteLine("File Copied: " + string.Join(", ", "0", changeLog.Copied.Select(x => x.Path)));
            Console.WriteLine("File Deleted: " + string.Join(", ", "0", changeLog.Deleted.Select(x => x.Path)));
            Console.WriteLine("File Modified: " + string.Join(", ", "0", changeLog.Modified.Select(x => x.Path)));
            Console.WriteLine("File Renamed: " + string.Join(", ", "0", changeLog.Renamed.Select(x => x.Path)));
            Console.WriteLine("File Unmodified: " + string.Join(", ", "0", changeLog.Unmodified.Select(x => x.Path)));
        }

        private static void LogCommit(Commit currentCommit, string commitDescriptor)
        {
            Console.WriteLine(string.Concat(commitDescriptor, " id: ", currentCommit.Id));
            Console.WriteLine(string.Concat(commitDescriptor, " author: ", currentCommit.Author));
            Console.WriteLine(string.Concat(commitDescriptor, " message: ", currentCommit.Message));
        }

        private static void LogArguments(string[] args)
        {
            if (args == null)
            {
                Console.WriteLine("args is null"); // Check for null array
            }
            else
            {
                Console.Write("args length is ");
                Console.WriteLine(args.Length); // Write array length
                for (int i = 0; i < args.Length; i++) // Loop through array
                {
                    string argument = args[i];
                    Console.Write("args index ");
                    Console.Write(i); // Write index
                    Console.Write(" is [");
                    Console.Write(argument); // Write string
                    Console.WriteLine("]");
                }
            }
        }
    }
}

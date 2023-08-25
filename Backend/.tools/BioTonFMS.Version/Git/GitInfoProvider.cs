namespace BioTonFMS.Version.Git
{
    using System;

    /// <summary>
    /// Provides Mercurial information for a particular file path, by executing and scraping
    /// information from the hg.exe command-line program.
    /// </summary>
    public class GitInfoProvider : SourceControlInfoProvider
    {
        private int? _revisionNumber;
        private string _revisionId;
        private bool? _isWorkingCopyDirty;
        private string _branch;
        private string _tags;
        private string _lastTag;
        private string _revision;

        public override string SourceControlName => "Git";

        public string LastTag => !string.IsNullOrWhiteSpace(_lastTag) ? _lastTag : (_lastTag = GetLastTag());

#pragma warning disable RECS0154 // Parameter is never used
        public static object GetLongRevisionId(GitInfoProvider instance)
#pragma warning restore RECS0154 // Parameter is never used
        {
#pragma warning disable RECS0083 // Shows NotImplementedException throws in the quick task bar
            throw new NotImplementedException();
#pragma warning restore RECS0083 // Shows NotImplementedException throws in the quick task bar
        }

        public virtual int GetRevisionNumber()
        {
            if (_revisionNumber == null)
            {
                InitRevision();
            }
            
            return _revisionNumber ?? default(int);
        }

        public virtual string GetRevisionId()
        {
            if (_revisionId == null)
            {
                InitRevision();
            }
            
            return _revisionId;
        }

        public virtual bool IsWorkingCopyDirty()
        {
            if (_isWorkingCopyDirty == null)
            {
                ExecuteCommand(
                    "git",
                    "diff-index --quiet HEAD",
                    (exitCode, error) =>
                        {
                            switch (exitCode)
                            {
                                case 0:
                                    _isWorkingCopyDirty = false;
                                    return false;
                                case 1:
                                    _isWorkingCopyDirty = true;
                                    return false;
                        }

                    return true;
                    });
            }
            
            return _isWorkingCopyDirty ?? default(bool);
        }

        public virtual string GetTags()
        {
            return _tags ?? (_tags = ExecuteCommand("git", "tag"));
        }

        public virtual string GetBranch()
        {
            if (!string.IsNullOrWhiteSpace(_branch))
            {
                return _branch;
            }

            _branch = ExecuteCommand(
                "git",
                /*"symbolic-ref --short HEAD"*/
                /*"rev-parse --abbrev-ref HEAD"*/
                "describe --all");

            return _branch;
        }

        public virtual string GetLastTag()
        {
            return _lastTag ?? (_lastTag = base.ExecuteCommand("git", "describe")[0].Split('-')[0]);
        }

        public virtual string GetShortRevisonNumber()
        {
            // string args = "rev-list " + LastTag + "..HEAD --count --no-merges";
            // Выводится количество коммитов, начинающихся с символа "#" - номера задачи в Редмайне
            string args = "rev-list " + LastTag + "..HEAD --count --pretty=format:\"%cd %s\" --grep=\"^#\" --remove-empty";

            return _revision ?? (_revision = ExecuteCommand("git", args));
        }

        public virtual string GetRevisionVersion()
        {
            var describtionSplit = base.ExecuteCommand("git", "describe")[0].Split('-');
            if (describtionSplit.Length > 1)
            { 
                return describtionSplit[1];
            }
            else
            {
                return "0"; // это первый комит версии
            }
        }

        public virtual string GetShortRevisionId()
        {
            // первые 8 символов хэша коммита
            return GetRevisionId().Substring(0, 8);
        }

        protected new string ExecuteCommand(string fileName, string arguments)
        {
            var result = base.ExecuteCommand(fileName, arguments);
            return result?.Count > 0 ? result[0] : string.Empty;
        }

        private void InitRevision()
        {
            ExecuteCommand(
                "git",
                "rev-list HEAD",
                output =>
                    {
                        if (_revisionId == null)
                        {
                            _revisionId = output;
                            _revisionNumber = 1;
                        }
                        else
                        {
                            _revisionNumber += 1;
                        }
                    },
                null);
        }
    }
}
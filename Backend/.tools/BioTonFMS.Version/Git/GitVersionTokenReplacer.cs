namespace BioTonFMS.Version.Git
{
    /// <summary>
    /// Replaces tokens in a string with information from a <c>GitInfoProvider</c>.
    /// </summary>
    public class GitVersionTokenReplacer : VersionTokenReplacer
    {
        public GitVersionTokenReplacer(GitInfoProvider infoProvider)
        {
            AddToken("REVNUM", infoProvider.GetShortRevisonNumber);
            AddToken("REVNUM_MOD", x => (infoProvider.GetRevisionNumber() % x).ToString());
            AddToken("REVNUM_DIV", x => (infoProvider.GetRevisionNumber() / x).ToString());
            AddToken("REVID", infoProvider.GetShortRevisionId);
            AddToken("DIRTY", () => infoProvider.IsWorkingCopyDirty() ? "1" : "0");
            AddToken("BRANCH", infoProvider.GetBranch);
            AddToken("TAGS", infoProvider.GetTags);
            AddToken("LASTTAG", infoProvider.GetLastTag);
            AddToken("REV_VER", infoProvider.GetRevisionVersion);
        }
    }
}
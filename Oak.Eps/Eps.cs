using Common.Server;
using Common.Server.Auth;
using Oak.Db;

namespace Oak.Eps;

public static class OakEps
{
    private static IReadOnlyList<IEp>? _eps;
    public static IReadOnlyList<IEp> Eps
    {
        get
        {
            if (_eps == null)
            {
                var eps =
                    (List<IEp>)
                        new CommonEps<OakDb>(
                            5,
                            true,
                            5,
                            OrgEps.AuthOnActivation,
                            OrgEps.AuthOnDelete,
                            OrgEps.AuthValidateFcmTopic
                        ).Eps;
                eps.AddRange(OrgEps.Eps);
                eps.AddRange(OrgMemberEps.Eps);
                eps.AddRange(ProjectEps.Eps);
                eps.AddRange(ProjectMemberEps.Eps);
                eps.AddRange(TaskEps.Eps);
                eps.AddRange(TimerEps.Eps);
                eps.AddRange(VItemEps.Eps);
                eps.AddRange(FileEps.Eps);
                eps.AddRange(CommentEps.Eps);
                _eps = eps;
            }
            return _eps;
        }
    }
}

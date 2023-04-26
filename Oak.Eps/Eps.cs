﻿using Common.Server;
using Common.Server.Auth;
using Oak.Db;
using Task = System.Threading.Tasks.Task;

namespace Oak.Eps;

public static class OakEps
{
    private static IReadOnlyList<IRpcEndpoint>? _eps;
    public static IReadOnlyList<IRpcEndpoint> Eps
    {
        get
        {
            if (_eps == null)
            {
                var eps = (List<IRpcEndpoint>)new AuthEps<OakDb>(5, OrgEps.AuthOnDelete).Eps;
                eps.AddRange(OrgEps.Eps);
                eps.AddRange(OrgMemberEps.Eps);
                eps.AddRange(ProjectEps.Eps);
                _eps = eps;
            }
            return _eps;
        }
    }
}

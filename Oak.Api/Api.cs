using System.Collections;
using Common.Shared;
using Common.Shared.Auth;
using Newtonsoft.Json;
using Oak.Api.Org;
using Oak.Api.OrgMember;
using Oak.Api.Project;

namespace Oak.Api;

public class MinMaxBaseException : Exception
{
    public MinMaxBaseException(string msg) : base(msg)
    {
    }
}

public class NullMinMaxValuesException : MinMaxBaseException
{
    public NullMinMaxValuesException() : base("invalid min max args, both are null")
    {
    }
}

public class InvalidMinMaxValuesException : MinMaxBaseException
{
    public string Min { get; }
    public string Max { get; }

    public InvalidMinMaxValuesException(string min, string max) : base(
        $"invalid min max args min: {min} must not be larger than max: {max}")
    {
        Min = min;
        Max = max;
    }
}
public record MinMax<T> where T : IComparable<T>
{
    public T? Min { get; }
    public T? Max { get; }
    
    [JsonConstructor]
    public MinMax(T? min, T? max)
    {
        if (min == null && max == null)
        {
            throw new NullMinMaxValuesException();
        }
        if (min != null && max != null && min.CompareTo(max) > 0)
        {
            throw new InvalidMinMaxValuesException(min.ToString().NotNull(), max.ToString().NotNull());
        }

        Min = min;
        Max = max;
    }
}
public interface IApi : Common.Shared.Auth.IApi
{
    public IOrgApi Org { get; }
    public IOrgMemberApi OrgMember { get; }
    public IProjectApi Project { get; }
}

public class Api : IApi
{
    public Api(IRpcClient client)
    {
        Auth = new AuthApi(client);
        Org = new OrgApi(client);
        OrgMember = new OrgMemberApi(client);
        Project = new ProjectApi(client);
    }

    public IAuthApi Auth { get; }
    public IOrgApi Org { get; }
    public IOrgMemberApi OrgMember { get; }
    public IProjectApi Project { get; }
}

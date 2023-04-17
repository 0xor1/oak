﻿using Common.Shared;

namespace Oak.Api.Org;

public interface IOrgApi
{
    private static IOrgApi? _inst;
    static IOrgApi Init() => _inst ??= new OrgApi();
    
    public Rpc<Create, Org> Create { get; }
    public Rpc<Get, IReadOnlyList<Org>> Get { get; }
    public Rpc<Update, Org> Update { get; }
    public Rpc<Delete, Nothing> Delete { get; }
}
public class OrgApi: IOrgApi
{
    public Rpc<Create, Org> Create { get; } = new ("/org/create");
    public Rpc<Get, IReadOnlyList<Org>> Get { get; } = new ("/org/get");
    public Rpc<Update, Org> Update { get; } = new ("/org/update");
    public Rpc<Delete, Nothing> Delete { get; } = new ("/org/delete");
}

public record Org(string Id, string Name, DateTime CreatedOn);
public record Create(string Name, string OwnerMemberName);
public record Get(OrgOrderBy OrderBy = OrgOrderBy.Name, bool Asc = true);
public record Update(string Id, string NewName);
public record Delete(string Id);

public enum OrgOrderBy
{
    Name,
    CreatedOn
}
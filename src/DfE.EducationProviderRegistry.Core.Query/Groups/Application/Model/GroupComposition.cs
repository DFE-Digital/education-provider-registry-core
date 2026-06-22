namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupComposition
{
    public GroupComposition(
        IEnumerable<Academy>? academies = null,
        IEnumerable<Member>? members = null,
        IEnumerable<Trustee>? trustees = null)
    {
        Academies = academies?.ToList() ?? [];
        Members = members?.ToList() ?? [];
        Trustees = trustees?.ToList() ?? [];
    }

    public IReadOnlyCollection<Academy> Academies { get; }
    public IReadOnlyCollection<Member> Members { get; }
    public IReadOnlyCollection<Trustee> Trustees { get; }

    public bool Equals(GroupComposition? other)
    {
        if (other is null)
        {
            return false;
        }

        return Academies.SequenceEqual(other.Academies)
            && Members.SequenceEqual(other.Members)
            && Trustees.SequenceEqual(other.Trustees);
    }

    public override int GetHashCode()
    {
        HashCode hash = new();

        foreach (Academy academy in Academies)
        {
            hash.Add(academy);
        }

        foreach (Member member in Members)
        {
            hash.Add(member);
        }


        foreach (Trustee trustee in Trustees)
        {
            hash.Add(trustee);
        }

        return hash.ToHashCode();
    }
}

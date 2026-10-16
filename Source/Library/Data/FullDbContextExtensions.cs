using Library.Entity;

namespace Library.Data;

internal static class FullDbContextExtensions
{
    /// <summary>
    /// Copie les valeurs de <paramref name="updated"/> vers l'entité suivie <paramref name="existing"/>.
    /// SetValues ne copie que les propriétés scalaires : le type détenu Active doit être recopié
    /// explicitement. Audit n'est volontairement pas copié (géré par FullDbContext).
    /// </summary>
    internal static void UpdateFrom<T>(this FullDbContext dbContext, T existing, T updated)
        where T : PrimaryKey
    {
        dbContext.Entry(existing).CurrentValues.SetValues(updated);

        if (updated.Active != null)
        {
            existing.Active ??= new Active();
            existing.Active.From = updated.Active.From;
            existing.Active.Until = updated.Active.Until;
        }
    }
}

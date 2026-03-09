using SportReservation.Data;
using SportReservation.Models;

namespace SportReservation.Services;

public class UserService(AppDbContext db)
{
    public async Task Update(User user, UserPatchDto patch)
    {
        if (patch.Email != null)
        {
            user.Email = patch.Email;
        }

        if (patch.FullName != null)
        {
            user.FullName = patch.FullName;
        }

        db.Update(user);
        await db.SaveChangesAsync();
    }
}